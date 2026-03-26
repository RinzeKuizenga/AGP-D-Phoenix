using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Arrival_Panel : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField, Tooltip("The Compass_Controller that manages waypoints.")]
    private Compass_Controller compass_controller;

    [SerializeField, Tooltip("Text element showing the destination name on arrival.")]
    private TextMeshProUGUI arrival_text;

    [Header("Timing")]
    [SerializeField, Tooltip("How long the fade-in takes in seconds.")]
    private float fade_in_duration = 0.4f;

    [SerializeField, Tooltip("How long the panel stays fully visible in seconds.")]
    private float display_duration = 2.0f;

    [SerializeField, Tooltip("How long the fade-out takes in seconds.")]
    private float fade_out_duration = 0.6f;

    [Header("Player Reference")]
    [SerializeField, Tooltip("Player transform for distance check. Falls back to main camera.")]
    private Transform player_transform;

    [SerializeField, Tooltip("Distance in meters to trigger arrival.")]
    private float arrival_threshold = 3f;

    #endregion

    #region Private State

    private CanvasGroup canvas_group;
    private bool is_showing;
    private int last_checked_index = -1;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        canvas_group = GetComponent<CanvasGroup>();

        // Start hidden
        if (canvas_group != null)
        {
            canvas_group.alpha = 0f;
            canvas_group.interactable = false;
            canvas_group.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (compass_controller == null || is_showing) return;
        if (compass_controller.all_waypoints_reached()) return;

        // Only check if waypoint index changed or on first frame
        int current_index = compass_controller.get_active_waypoint_index();
        Compass_Marker_Data active = compass_controller.get_active_waypoint_data();

        if (active == null || active.target == null) return;

        // Check distance to active waypoint
        Vector3 delta = active.target.position - get_player_position();
        delta.y = 0f;

        if (delta.magnitude <= arrival_threshold)
        {
            StartCoroutine(show_arrival(active.destination_name));
        }
    }

    #endregion

    #region Arrival Sequence

    /// <summary>
    /// Plays the full arrival sequence: fade in, hold, fade out, then advance waypoint.
    /// </summary>
    private IEnumerator show_arrival(string destination_name)
    {
        is_showing = true;

        // Update text
        if (arrival_text != null)
            arrival_text.text = destination_name;

        // Fade in
        yield return StartCoroutine(fade(0f, 1f, fade_in_duration));

        // Hold
        yield return new WaitForSeconds(display_duration);

        // Fade out
        yield return StartCoroutine(fade(1f, 0f, fade_out_duration));

        // Advance to next waypoint after panel disappears
        if (compass_controller != null)
            compass_controller.advance_to_next_waypoint();

        is_showing = false;
    }

    /// <summary>
    /// Smoothly interpolates the CanvasGroup alpha between two values.
    /// </summary>
    private IEnumerator fade(float from, float to, float duration)
    {
        if (canvas_group == null) yield break;

        float elapsed = 0f;

        canvas_group.alpha = from;
        canvas_group.interactable = to > 0f;
        canvas_group.blocksRaycasts = to > 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvas_group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        canvas_group.alpha = to;
    }

    #endregion

    #region Helpers

    private Vector3 get_player_position()
    {
        if (player_transform != null) return player_transform.position;
        if (Camera.main != null) return Camera.main.transform.position;
        return Vector3.zero;
    }

    #endregion
}
