using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Professional destination HUD panel built with UI Toolkit.
/// Shows final destination name + distance, compass bearing, and nearest waypoint.
/// Attaches to the same UIDocument as the compass or a separate one.
///
/// SETUP:
/// 1. Add a UIDocument to a GameObject and assign Destination_Panel.uxml as the source asset.
/// 2. Attach this script to the same GameObject.
/// 3. Assign compass_controller and player_transform in the Inspector.
/// 4. The panel auto-hides when the final destination is reached.
/// </summary>
[ExecuteAlways]
public class Destination_Panel : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField, Tooltip("The Compass_Controller that manages waypoints.")]
    private Compass_Controller compass_controller;

    [SerializeField, Tooltip("Ship/player transform for distance + bearing.")]
    private Transform player_transform;

    [Header("UI Document")]
    [SerializeField, Tooltip("UIDocument with the Destination_Panel.uxml layout.")]
    private UIDocument ui_document;

    [Header("Labels (Dutch)")]
    [SerializeField] private string label_destination = "EINDBESTEMMING";
    [SerializeField] private string label_distance = "AFSTAND";
    [SerializeField] private string waypoint_prefix = "Tussenstop: ";

    #endregion

    #region Private State – UI Elements

    private VisualElement panel_root;

    // Top row
    private Label destination_label;
    private Label destination_name;
    private Label distance_label_el;
    private Label distance_number;
    private Label distance_unit;

    // Bottom row
    private Label bearing_text;
    private VisualElement nearby_group;
    private VisualElement nearby_dot;
    private Label nearby_text;

    // Cache
    private int cached_final_dist = -1;
    private int cached_nearby_dist = -1;
    private int cached_bearing = -999;
    private bool ui_bound;

    #endregion

    #region Cardinal Lookup

    private static readonly string[] cardinal_16 =
    {
        "N", "NNO", "NO", "ONO",
        "O", "OZO", "ZO", "ZZO",
        "Z", "ZZW", "ZW", "WZW",
        "W", "WNW", "NW", "NNW"
    };

    private static string angle_to_cardinal(float angle)
    {
        if (angle < 0f) angle += 360f;
        int index = Mathf.RoundToInt(angle / 22.5f) % 16;
        return cardinal_16[index];
    }

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        bind_ui();
    }

    private void Update()
    {
        if (!ui_bound) bind_ui();
        if (!ui_bound)
        {
            Debug.Log("Destination_Panel: UI not bound yet");
            return;
        }

        if (compass_controller == null)
        {
            Debug.LogWarning("Destination_Panel: compass_controller is null");
            return;
        }

        if (compass_controller.is_final_reached())
        {
            Debug.Log("Destination_Panel: final reached — panel hidden");
            set_panel_visible(false);
            return;
        }

        set_panel_visible(true);

        Vector3 player_pos = get_player_position();

        update_final_destination(player_pos);
        update_bearing(player_pos);
        update_nearby(player_pos);
    }

    #endregion

    #region UI Binding

    private void bind_ui()
    {
        if (ui_document == null)
            ui_document = GetComponent<UIDocument>();
        if (ui_document == null || ui_document.rootVisualElement == null)
            return;

        VisualElement root = ui_document.rootVisualElement;

        // CRITICAL: Let mouse events pass through to the game camera.
        // The UIDocument root covers the entire screen by default and
        // blocks all input. Setting pickingMode to Ignore on root and
        // its auto-generated child container fixes camera rotation.
        root.pickingMode = PickingMode.Ignore;
        foreach (VisualElement child in root.Children())
            child.pickingMode = PickingMode.Ignore;

        panel_root = root.Q<VisualElement>("destination-panel");
        if (panel_root == null) return;

        destination_label = root.Q<Label>("destination-label");
        destination_name = root.Q<Label>("destination-name");
        distance_label_el = root.Q<Label>("distance-label");
        distance_number = root.Q<Label>("distance-number");
        distance_unit = root.Q<Label>("distance-unit");
        bearing_text = root.Q<Label>("bearing-text");
        nearby_group = root.Q<VisualElement>("nearby-group");
        nearby_dot = root.Q<VisualElement>("nearby-dot");
        nearby_text = root.Q<Label>("nearby-text");

        // Set static labels
        if (destination_label != null) destination_label.text = label_destination;
        if (distance_label_el != null) distance_label_el.text = label_distance;

        ui_bound = true;
    }

    #endregion

    #region Update Sections

    private void update_final_destination(Vector3 player_pos)
    {
        Compass_Marker_Data final_wp = compass_controller.get_final_data();
        if (final_wp == null || final_wp.target == null) return;

        // Name
        if (destination_name != null)
            destination_name.text = final_wp.destination_name;

        // Distance
        Vector3 delta = final_wp.target.position - player_pos;
        delta.y = 0f;
        int meters = Mathf.RoundToInt(delta.magnitude);

        if (meters != cached_final_dist)
        {
            cached_final_dist = meters;

            if (meters >= 1000)
            {
                float km = meters / 1000f;
                if (distance_number != null) distance_number.text = km.ToString("0.0");
                if (distance_unit != null) distance_unit.text = "km";
            }
            else
            {
                if (distance_number != null) distance_number.text = meters.ToString();
                if (distance_unit != null) distance_unit.text = "m";
            }
        }
    }

    private void update_bearing(Vector3 player_pos)
    {
        Compass_Marker_Data final_wp = compass_controller.get_final_data();
        if (final_wp == null || final_wp.target == null) return;

        Vector3 dir = final_wp.target.position - player_pos;
        dir.y = 0f;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        int rounded = Mathf.RoundToInt(angle);
        if (rounded != cached_bearing)
        {
            cached_bearing = rounded;
            string cardinal = angle_to_cardinal(angle);
            if (bearing_text != null)
                bearing_text.text = cardinal + " " + rounded + "\u00B0";
        }
    }

    private void update_nearby(Vector3 player_pos)
    {
        if (nearby_group == null) return;

        int nearest_idx = compass_controller.get_nearest_unvisited_intermediate();

        if (nearest_idx < 0)
        {
            set_nearby_visible(false);
            return;
        }

        Compass_Marker_Data nearby = compass_controller.get_waypoint_data(nearest_idx);
        if (nearby == null || nearby.target == null)
        {
            set_nearby_visible(false);
            return;
        }

        set_nearby_visible(true);

        Vector3 delta = nearby.target.position - player_pos;
        delta.y = 0f;
        int meters = Mathf.RoundToInt(delta.magnitude);

        if (meters != cached_nearby_dist)
        {
            cached_nearby_dist = meters;
            if (nearby_text != null)
                nearby_text.text = waypoint_prefix + nearby.destination_name
                                 + " \u2014 " + format_distance(meters);
        }
    }

    #endregion

    #region Visibility

    private void set_panel_visible(bool visible)
    {
        if (panel_root == null) return;

        if (visible)
            panel_root.RemoveFromClassList("destination-panel-hidden");
        else
            panel_root.AddToClassList("destination-panel-hidden");
    }

    private void set_nearby_visible(bool visible)
    {
        if (nearby_group == null) return;

        if (visible)
            nearby_group.RemoveFromClassList("nearby-group-hidden");
        else
            nearby_group.AddToClassList("nearby-group-hidden");
    }

    #endregion

    #region Helpers

    private Vector3 get_player_position()
    {
        if (player_transform != null) return player_transform.position;
        if (Camera.main != null) return Camera.main.transform.position;
        return Vector3.zero;
    }

    private static string format_distance(int meters)
    {
        if (meters >= 1000)
            return (meters / 1000f).ToString("0.0") + " km";
        return meters + " m";
    }

    #endregion
}