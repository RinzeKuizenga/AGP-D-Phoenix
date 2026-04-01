using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class Compass_Controller : MonoBehaviour
{
    #region Serialized Fields

    [Header("UI Reference")]
    [SerializeField, Tooltip("UIDocument that contains the compass layout.")]
    private UIDocument ui_document;

    [Header("Compass Settings")]
    [SerializeField, Tooltip("Visible width of the compass bar in pixels (synced from USS --compass-width).")]
    private float compass_width = 800f;

    [SerializeField, Tooltip("Degrees of the world visible at once. Lower = more zoomed in.")]
    private float compass_fov = 150f;

    [Header("Waypoint Settings")]
    [SerializeField, Tooltip("Color used to highlight the active (next) waypoint.")]
    private Color active_marker_color = new Color(1f, 0.78f, 0.2f, 1f);

    [Header("Editor Preview")]
    [SerializeField, Tooltip("Simulated heading in edit mode."), Range(0f, 360f)]
    private float editor_preview_heading;

    [Header("Player Reference")]
    [SerializeField, Tooltip("Player transform used for distance calculations. Falls back to main camera.")]
    private Transform player_transform;

    [Header("Markers (ordered destinations)")]
    [SerializeField, Tooltip("Ordered list of waypoints. The first unvisited entry is the active destination.")]
    private List<Compass_Marker_Data> waypoints = new List<Compass_Marker_Data>();

    #endregion

    #region Private State

    private VisualElement compass_strip;
    private VisualElement compass_markers;
    private readonly List<Compass_Marker> marker_instances = new List<Compass_Marker>();
    private Transform cam_transform;
    private float pixels_per_degree;
    private float strip_offset;
    private float half_fov;
    private int active_waypoint_index;

    private static readonly (string label, float angle)[] cardinals =
    {
        ("N",  0f),   ("NE", 45f),  ("E",  90f),  ("SE", 135f),
        ("S",  180f), ("SW", 225f), ("W",  270f), ("NW", 315f)
    };

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        recalculate_metrics();
    }

    private void OnEnable()
    {
        recalculate_metrics();

        if (ui_document == null)
            ui_document = GetComponent<UIDocument>();

        if (ui_document != null && ui_document.rootVisualElement != null)
            ui_document.rootVisualElement.RegisterCallback<GeometryChangedEvent>(on_geometry_changed);

#if UNITY_EDITOR
        EditorApplication.update += editor_update;
#endif
    }

    private void OnDisable()
    {
        if (ui_document != null && ui_document.rootVisualElement != null)
            ui_document.rootVisualElement.UnregisterCallback<GeometryChangedEvent>(on_geometry_changed);

#if UNITY_EDITOR
        EditorApplication.update -= editor_update;
#endif
    }

    private void Update()
    {
        ensure_initialized();

        if (compass_strip == null)
            return;

        if (compass_strip.childCount == 0)
        {
            recalculate_metrics();
            generate_compass_ticks();
            rebuild_marker_ui();
        }

        update_strip_position();
        update_markers();
    }

#if UNITY_EDITOR
    private void editor_update()
    {
        if (Application.isPlaying) return;
        if (ui_document == null) ui_document = GetComponent<UIDocument>();
        if (ui_document == null || ui_document.rootVisualElement == null) return;

        VisualElement root = ui_document.rootVisualElement;
        if (root.childCount == 0) return;

        VisualElement current_strip = root.Q<VisualElement>("compass-strip");

        bool needs_init = compass_strip == null
                       || current_strip != compass_strip
                       || (current_strip != null && current_strip.childCount == 0);

        if (needs_init && current_strip != null)
        {
            initialize_compass();
            rebuild_marker_ui();
        }

        if (compass_strip != null)
        {
            update_strip_position();
            update_markers();
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (compass_strip == null || compass_markers == null)
            ensure_initialized();

        if (compass_strip != null)
        {
            recalculate_metrics();
            update_strip_position();
            rebuild_marker_ui();
        }
    }
#endif

    #endregion

    #region Initialization

    private void recalculate_metrics()
    {
        if (Camera.main != null)
            cam_transform = Camera.main.transform;

        pixels_per_degree = compass_width / compass_fov;
        half_fov = compass_fov * 0.5f;
        strip_offset = 180f * pixels_per_degree;
    }

    private void ensure_initialized()
    {
        if (compass_strip != null && compass_markers != null)
            return;

        if (ui_document == null)
            ui_document = GetComponent<UIDocument>();
        if (ui_document == null || ui_document.rootVisualElement == null)
            return;

        VisualElement root = ui_document.rootVisualElement;
        if (root.childCount == 0)
            return;

        compass_strip = root.Q<VisualElement>("compass-strip");
        compass_markers = root.Q<VisualElement>("compass-markers");

        if (compass_strip != null && compass_strip.childCount == 0)
        {
            sync_width_from_wrapper(root);
            recalculate_metrics();
            generate_compass_ticks();
            rebuild_marker_ui();
        }
    }

    private void on_geometry_changed(GeometryChangedEvent evt)
    {
        VisualElement root = ui_document.rootVisualElement;
        VisualElement new_strip = root.Q<VisualElement>("compass-strip");

        bool needs_init = compass_strip == null
                       || new_strip != compass_strip
                       || (compass_strip != null && compass_strip.childCount == 0);

        if (needs_init)
        {
            initialize_compass();
            rebuild_marker_ui();
        }
    }

    private void initialize_compass()
    {
        VisualElement root = ui_document.rootVisualElement;
        compass_strip = root.Q<VisualElement>("compass-strip");
        compass_markers = root.Q<VisualElement>("compass-markers");

        if (compass_strip == null)
        {
            Debug.LogError("Compass_Controller: 'compass-strip' element not found in UXML.");
            return;
        }

        sync_width_from_wrapper(root);
        recalculate_metrics();
        generate_compass_ticks();
    }

    private void sync_width_from_wrapper(VisualElement root)
    {
        VisualElement wrapper = root.Q<VisualElement>("compass-wrapper");
        if (wrapper != null && wrapper.resolvedStyle.width > 0)
        {
            compass_width = wrapper.resolvedStyle.width;
            pixels_per_degree = compass_width / compass_fov;
            strip_offset = 180f * pixels_per_degree;
        }
    }

    #endregion

    #region Compass Strip Generation

    private void generate_compass_ticks()
    {
        compass_strip.Clear();

        float strip_width = 720f * pixels_per_degree;
        compass_strip.style.width = strip_width;

        for (int deg = -180; deg <= 540; deg += 5)
        {
            float x = (deg + 180f) * pixels_per_degree;
            compass_strip.Add(create_tick(deg, x));

            int norm = normalize_angle(deg);
            if (deg % 15 == 0 && !is_cardinal(norm))
                compass_strip.Add(create_degree_label(norm, x));
        }

        foreach ((string text, float angle) in cardinals)
        {
            add_cardinal_label(text, angle);
            add_cardinal_label(text, angle - 360f);
            add_cardinal_label(text, angle + 360f);
        }
    }

    private VisualElement create_tick(int degree, float x)
    {
        VisualElement tick = new VisualElement();
        tick.AddToClassList("compass-tick");

        int norm = normalize_angle(degree);
        if (is_cardinal(norm))
            tick.AddToClassList("compass-tick-large");
        else if (norm % 15 == 0)
            tick.AddToClassList("compass-tick-medium");
        else
            tick.AddToClassList("compass-tick-small");

        tick.style.left = x;
        tick.pickingMode = PickingMode.Ignore;
        return tick;
    }

    private Label create_degree_label(int display_degree, float x)
    {
        Label label = new Label(display_degree.ToString());
        label.AddToClassList("compass-degree");
        label.style.left = x;
        label.pickingMode = PickingMode.Ignore;
        return label;
    }

    private void add_cardinal_label(string text, float angle)
    {
        if (angle < -180f || angle > 540f) return;

        float x = (angle + 180f) * pixels_per_degree;
        Label label = new Label(text);
        label.AddToClassList("compass-label");
        label.AddToClassList("compass-label-cardinal");
        label.style.left = x;
        label.pickingMode = PickingMode.Ignore;
        compass_strip.Add(label);
    }

    private bool is_cardinal(int degree)
    {
        return degree % 45 == 0 && degree >= 0 && degree < 360;
    }

    private int normalize_angle(int degree)
    {
        int n = degree % 360;
        return n < 0 ? n + 360 : n;
    }

    #endregion

    #region Strip Rotation

    private void update_strip_position()
    {
        if (compass_strip == null) return;

        float yaw = get_current_yaw();
        compass_strip.style.left = -yaw * pixels_per_degree - strip_offset + compass_width * 0.5f;
    }

    private float get_current_yaw()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return editor_preview_heading;
#endif
        if (cam_transform == null) return 0f;

        Vector3 fwd = cam_transform.forward;
        float yaw = Mathf.Atan2(fwd.x, fwd.z) * Mathf.Rad2Deg;
        return yaw < 0f ? yaw + 360f : yaw;
    }

    #endregion

    #region Waypoint Progression (Public API)

    /// <summary>Returns the total number of waypoints.</summary>
    public int get_waypoint_count() => waypoints.Count;

    /// <summary>Returns the index of the current active waypoint.</summary>
    public int get_active_waypoint_index() => active_waypoint_index;

    /// <summary>Returns true if all waypoints have been reached.</summary>
    public bool all_waypoints_reached() => active_waypoint_index >= waypoints.Count;

    /// <summary>Returns the data of the current active waypoint, or null if all are reached.</summary>
    public Compass_Marker_Data get_active_waypoint_data()
    {
        if (active_waypoint_index >= waypoints.Count) return null;
        return waypoints[active_waypoint_index];
    }

    /// <summary>Returns waypoint data at a specific index, or null if out of range.</summary>
    public Compass_Marker_Data get_waypoint_data(int index)
    {
        if (index < 0 || index >= waypoints.Count) return null;
        return waypoints[index];
    }

    /// <summary>Advances to the next waypoint and refreshes visuals.</summary>
    public void advance_to_next_waypoint()
    {
        active_waypoint_index++;
        refresh_marker_styles();
    }

    /// <summary>Resets progression back to the first waypoint.</summary>
    public void reset_progression()
    {
        active_waypoint_index = 0;
        refresh_marker_styles();
    }

    /// <summary>Sets the active waypoint to a specific index.</summary>
    public void set_active_waypoint(int index)
    {
        active_waypoint_index = Mathf.Clamp(index, 0, waypoints.Count);
        refresh_marker_styles();
    }

    #endregion

    #region Marker Management

    private void rebuild_marker_ui()
    {
        foreach (Compass_Marker m in marker_instances)
        {
            if (m.element != null)
                compass_markers?.Remove(m.element);
        }
        marker_instances.Clear();

        if (compass_markers == null) return;

        for (int i = 0; i < waypoints.Count; i++)
        {
            Compass_Marker_Data data = waypoints[i];
            if (data.target == null) continue;

            bool is_active = (i == active_waypoint_index);
            Color color = is_active ? active_marker_color : data.color;

            Compass_Marker marker = new Compass_Marker(data.target, color);

            marker.element = new VisualElement();
            marker.element.AddToClassList("compass-marker");

            marker.dot_element = new VisualElement();
            marker.dot_element.AddToClassList("compass-marker-dot");
            marker.dot_element.style.backgroundColor = color;

            if (is_active)
                marker.dot_element.AddToClassList("compass-marker-dot-active");

            marker.distance_label = new Label();
            marker.distance_label.AddToClassList("compass-marker-distance");
            marker.distance_label.style.display = is_active ? DisplayStyle.Flex : DisplayStyle.None;

            marker.element.Add(marker.dot_element);
            marker.element.Add(marker.distance_label);

            compass_markers.Add(marker.element);
            marker_instances.Add(marker);
        }
    }

    private void refresh_marker_styles()
    {
        int instance_idx = 0;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i].target == null) continue;
            if (instance_idx >= marker_instances.Count) break;

            Compass_Marker marker = marker_instances[instance_idx];
            bool is_active = (i == active_waypoint_index);

            Color color = is_active ? active_marker_color : waypoints[i].color;
            marker.color = color;
            marker.dot_element.style.backgroundColor = color;

            if (is_active)
                marker.dot_element.AddToClassList("compass-marker-dot-active");
            else
                marker.dot_element.RemoveFromClassList("compass-marker-dot-active");

            marker.distance_label.style.display = is_active ? DisplayStyle.Flex : DisplayStyle.None;

            instance_idx++;
        }
    }

    private void update_markers()
    {
        if (compass_markers == null) return;

        float current_yaw = get_current_yaw();
        float half_width = compass_width * 0.5f;
        Vector3 player_pos = get_player_position();

        foreach (Compass_Marker marker in marker_instances)
        {
            if (marker.target == null)
            {
                set_marker_visible(marker, false);
                continue;
            }

            Vector3 dir = marker.target.position - player_pos;
            dir.y = 0f;

            float target_angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            if (target_angle < 0f) target_angle += 360f;

            float relative = Mathf.DeltaAngle(current_yaw, target_angle);
            bool in_view = Mathf.Abs(relative) <= half_fov;

#if UNITY_EDITOR
            if (!Application.isPlaying && !in_view)
            {
                relative = Mathf.Clamp(relative, -half_fov, half_fov);
                in_view = true;
            }
#endif

            if (!in_view)
            {
                set_marker_visible(marker, false);
                continue;
            }

            set_marker_visible(marker, true);
            marker.element.style.left = relative * pixels_per_degree + half_width;

            int distance = Mathf.RoundToInt(dir.magnitude);
            if (distance != marker.cached_distance)
            {
                marker.cached_distance = distance;
                marker.distance_label.text = format_distance(distance);
            }
        }
    }

    private void set_marker_visible(Compass_Marker marker, bool visible)
    {
        if (marker.is_visible == visible) return;
        marker.is_visible = visible;

        if (visible)
            marker.element.RemoveFromClassList("compass-marker-hidden");
        else
            marker.element.AddToClassList("compass-marker-hidden");
    }

    #endregion

    #region Helpers

    private Vector3 get_player_position()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return player_transform != null ? player_transform.position : transform.position;
#endif
        if (player_transform != null) return player_transform.position;
        if (cam_transform != null) return cam_transform.position;
        return transform.position;
    }

    private static string format_distance(int meters)
    {
        if (meters >= 1000)
            return $"{meters / 1000f:0.0} km";
        return $"{meters} m";
    }

    #endregion
}