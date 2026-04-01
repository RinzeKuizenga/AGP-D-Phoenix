using UnityEngine;
using TMPro;

/// <summary>
/// Always-visible panel at the bottom of the screen showing:
/// - Current destination name
/// - Live distance in km/m
/// Reads from Compass_Controller. Never blocks mouse input.
/// </summary>
public class Destination_Panel : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField, Tooltip("Reference to the Compass_Controller.")]
    private Compass_Controller compass_controller;

    [SerializeField, Tooltip("Text element showing the destination name.")]
    private TextMeshProUGUI name_text;

    [SerializeField, Tooltip("Text element showing the distance.")]
    private TextMeshProUGUI distance_text;

    [Header("Settings")]
    [SerializeField] private Color name_color = new Color(1f, 0.78f, 0.2f, 1f);
    [SerializeField] private Color distance_color = new Color(1f, 1f, 1f, 0.9f);

    [SerializeField, Tooltip("Ship/player transform for distance calculation.")]
    private Transform player_transform;

    [Header("Label")]
    [SerializeField] private string destination_prefix = "Bestemming: ";

    #endregion

    #region Private State

    private CanvasGroup canvas_group;
    private int cached_distance = -1;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        canvas_group = GetComponent<CanvasGroup>();

        if (name_text != null) name_text.color = name_color;
        if (distance_text != null) distance_text.color = distance_color;

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
            Debug.LogWarning("Destination_Panel: player_transform niet toegewezen! Wijs je schip toe.");
    }

    private void Update()
    {
        if (compass_controller == null) return;

        if (compass_controller.all_waypoints_reached())
        {
            set_visible(false);
            return;
        }

        Compass_Marker_Data active = compass_controller.get_active_waypoint_data();
        if (active == null || active.target == null)
        {
            set_visible(false);
            return;
        }

        set_visible(true);

        // Update destination name
        if (name_text != null)
            name_text.text = $"{destination_prefix}{active.destination_name}";

        // Update live distance
        Vector3 delta = active.target.position - get_player_position();
        delta.y = 0f;
        int meters = Mathf.RoundToInt(delta.magnitude);

        if (meters != cached_distance && distance_text != null)
        {
            cached_distance = meters;
            distance_text.text = format_distance(meters);
        }
    }

    #endregion

    #region Helpers

    private void set_visible(bool visible)
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

    private Vector3 get_player_position()
    {
        if (player_transform != null) return player_transform.position;
        if (Camera.main != null) return Camera.main.transform.position;
        return Vector3.zero;
    }

    private static string format_distance(int meters)
    {
        if (meters >= 1000)
            return $"{meters / 1000f:0.0} km";
        return $"{meters} m";
    }

    #endregion
}