using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays the active waypoint name and distance on a Canvas panel.
/// Reads waypoint data from the Compass_Controller.
/// Place this on a UI panel inside your existing Canvas.
/// </summary>
public class Destination_Panel : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField, Tooltip("Reference to the Compass_Controller that manages waypoints.")]
    private Compass_Controller compass_controller;

    [SerializeField, Tooltip("Text element showing the destination name.")]
    private TextMeshProUGUI name_text;

    [SerializeField, Tooltip("Text element showing the distance.")]
    private TextMeshProUGUI distance_text;

    [Header("Settings")]
    [SerializeField, Tooltip("Color of the destination name text.")]
    private Color name_color = new Color(1f, 0.78f, 0.2f, 1f); // gold

    [SerializeField, Tooltip("Color of the distance text.")]
    private Color distance_color = new Color(1f, 1f, 1f, 0.9f);

    [SerializeField, Tooltip("Player transform for distance calculation. Falls back to main camera.")]
    private Transform player_transform;

    #endregion

    #region Private State

    private CanvasGroup canvas_group;
    private int cached_distance = -1;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        canvas_group = GetComponent<CanvasGroup>();

        if (name_text != null)
            name_text.color = name_color;

        if (distance_text != null)
            distance_text.color = distance_color;

        // Forceer dat dit paneel de muis NIET blokkeert bij de start
        if (canvas_group != null)
        {
            canvas_group.interactable = false;
            canvas_group.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (compass_controller == null) return;

        // Hide panel when all waypoints are reached
        if (compass_controller.all_waypoints_reached())
        {
            set_visible(false);
            return;
        }

        // Get active waypoint data
        Compass_Marker_Data active = compass_controller.get_active_waypoint_data();

        if (active == null || active.target == null)
        {
            set_visible(false);
            return;
        }

        set_visible(true);

        // Update name
        if (name_text != null)
            name_text.text = $"Bestemming: {active.destination_name}";

        // Update distance
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