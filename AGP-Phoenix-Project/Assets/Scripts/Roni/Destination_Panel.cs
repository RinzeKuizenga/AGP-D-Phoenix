using UnityEngine;
using TMPro;

/// <summary>
/// Always shows the final destination (last waypoint) with live distance.
/// Optionally shows the nearest unvisited intermediate waypoint.
/// Hides when the final destination is reached.
/// </summary>
public class Destination_Panel : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField, Tooltip("Reference to the Compass_Controller.")]
    private Compass_Controller compass_controller;

    [SerializeField, Tooltip("Text showing the final destination name.")]
    private TextMeshProUGUI name_text;

    [SerializeField, Tooltip("Text showing distance to final destination.")]
    private TextMeshProUGUI distance_text;

    [Header("Optional: Nearest Intermediate")]
    [SerializeField, Tooltip("(Optional) Text showing nearest optional waypoint name.")]
    private TextMeshProUGUI nearby_name_text;

    [SerializeField, Tooltip("(Optional) Text showing nearest optional waypoint distance.")]
    private TextMeshProUGUI nearby_distance_text;

    [Header("Settings")]
    [SerializeField] private Color name_color = new Color(1f, 0.78f, 0.2f, 1f);
    [SerializeField] private Color distance_color = new Color(1f, 1f, 1f, 0.9f);
    [SerializeField] private Color nearby_name_color = new Color(0.8f, 0.8f, 0.8f, 0.9f);
    [SerializeField] private Color nearby_distance_color = new Color(0.7f, 0.7f, 0.7f, 0.8f);

    [SerializeField, Tooltip("Ship/player transform for distance.")]
    private Transform player_transform;

    [Header("Labels")]
    [SerializeField] private string destination_prefix = "Eindbestemming: ";
    [SerializeField] private string nearby_prefix = "Tussenstop: ";

    #endregion

    #region Private State

    private CanvasGroup canvas_group;
    private int cached_final_distance = -1;
    private int cached_nearby_distance = -1;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        canvas_group = GetComponent<CanvasGroup>();

        if (name_text != null) name_text.color = name_color;
        if (distance_text != null) distance_text.color = distance_color;
        if (nearby_name_text != null) nearby_name_text.color = nearby_name_color;
        if (nearby_distance_text != null) nearby_distance_text.color = nearby_distance_color;

        if (canvas_group != null)
        {
            canvas_group.interactable = false;
            canvas_group.blocksRaycasts = false;
        }
    }

    private void Start()
    {
        if (compass_controller == null)
            Debug.LogError("Destination_Panel: compass_controller niet toegewezen!");
        if (player_transform == null)
            Debug.LogWarning("Destination_Panel: player_transform niet toegewezen!");
    }

    private void Update()
    {
        if (compass_controller == null) return;

        if (compass_controller.is_final_reached())
        {
            set_panel_visible(false);
            set_nearby_visible(false);
            return;
        }

        set_panel_visible(true);

        Vector3 player_pos = get_player_position();

        // --- Final destination ---
        Compass_Marker_Data final_wp = compass_controller.get_final_data();
        if (final_wp != null && final_wp.target != null)
        {
            if (name_text != null)
                name_text.text = destination_prefix + final_wp.destination_name;

            Vector3 delta = final_wp.target.position - player_pos;
            delta.y = 0f;
            int meters = Mathf.RoundToInt(delta.magnitude);

            if (meters != cached_final_distance && distance_text != null)
            {
                cached_final_distance = meters;
                distance_text.text = format_distance(meters);
            }
        }

        // --- Nearest intermediate ---
        if (nearby_name_text == null && nearby_distance_text == null)
            return;

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

        if (nearby_name_text != null)
            nearby_name_text.text = nearby_prefix + nearby.destination_name;

        Vector3 nearby_delta = nearby.target.position - player_pos;
        nearby_delta.y = 0f;
        int nearby_meters = Mathf.RoundToInt(nearby_delta.magnitude);

        if (nearby_meters != cached_nearby_distance && nearby_distance_text != null)
        {
            cached_nearby_distance = nearby_meters;
            nearby_distance_text.text = format_distance(nearby_meters);
        }
    }

    #endregion

    #region Helpers

    private void set_panel_visible(bool visible)
    {
        if (canvas_group != null)
        {
            canvas_group.alpha = visible ? 1f : 0f;
            canvas_group.interactable = false;
            canvas_group.blocksRaycasts = false;
        }
        else
        {
            gameObject.SetActive(visible);
        }
    }

    private void set_nearby_visible(bool visible)
    {
        if (nearby_name_text != null)
            nearby_name_text.gameObject.SetActive(visible);
        if (nearby_distance_text != null)
            nearby_distance_text.gameObject.SetActive(visible);
    }

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