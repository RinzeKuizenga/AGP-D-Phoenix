using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Shows a fade-in/out arrival notification EACH time the ship reaches a waypoint.
/// Displays: arrived destination name, next destination name, and distance to next.
/// After the final waypoint, optionally loads a new scene.
/// </summary>
public class Arrival_Panel : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField, Tooltip("The Compass_Controller that manages waypoints.")]
    private Compass_Controller compass_controller;

    [SerializeField, Tooltip("Text showing 'Aangekomen bij: [naam]'.")]
    private TextMeshProUGUI arrival_title_text;

    [SerializeField, Tooltip("Text showing the next destination info or final message.")]
    private TextMeshProUGUI arrival_subtitle_text;

    [Header("Timing")]
    [SerializeField] private float fade_in_duration = 0.5f;
    [SerializeField] private float display_duration = 2.5f;
    [SerializeField] private float fade_out_duration = 0.6f;

    [Header("Player Reference")]
    [SerializeField, Tooltip("Ship/player transform for distance check.")]
    private Transform player_transform;

    [SerializeField, Tooltip("Distance in units to trigger arrival.")]
    private float arrival_threshold = 10f;

    [Header("Scene Loading (Final Destination)")]
    [SerializeField, Tooltip("Scene to load when the LAST destination is reached. Leave empty to skip.")]
    private string final_scene_name = "";

    [SerializeField, Tooltip("Delay before loading the final scene.")]
    private float scene_load_delay = 1.5f;

    #endregion

    #region Private State

    private CanvasGroup canvas_group;
    private bool is_showing;
    private int last_triggered_index = -1;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        canvas_group = GetComponent<CanvasGroup>();
        if (canvas_group == null)
        {
            Debug.LogWarning("Arrival_Panel: Geen CanvasGroup, wordt automatisch aangemaakt.");
            canvas_group = gameObject.AddComponent<CanvasGroup>();
        }

        canvas_group.alpha = 0f;
        canvas_group.interactable = false;
        canvas_group.blocksRaycasts = false;
    }

    private void Start()
    {
        if (compass_controller == null)
            Debug.LogError("Arrival_Panel: compass_controller is niet toegewezen!");
        if (player_transform == null)
            Debug.LogWarning("Arrival_Panel: player_transform niet toegewezen! Wijs je schip toe.");
    }

    private void Update()
    {
        if (compass_controller == null || is_showing) return;
        if (compass_controller.all_waypoints_reached()) return;

        int current_index = compass_controller.get_active_waypoint_index();
        if (current_index == last_triggered_index) return;

        Compass_Marker_Data active = compass_controller.get_active_waypoint_data();
        if (active == null || active.target == null) return;

        Vector3 delta = active.target.position - get_player_position();
        delta.y = 0f;

        if (delta.magnitude <= arrival_threshold)
        {
            last_triggered_index = current_index;
            StartCoroutine(show_arrival(current_index));
        }
    }

    #endregion

    #region Arrival Sequence

    private IEnumerator show_arrival(int arrived_index)
    {
        is_showing = true;

        Compass_Marker_Data arrived = compass_controller.get_waypoint_data(arrived_index);
        string arrived_name = arrived != null ? arrived.destination_name : "Onbekend";

        // Check if there is a next waypoint
        int next_index = arrived_index + 1;
        Compass_Marker_Data next_wp = compass_controller.get_waypoint_data(next_index);
        bool is_final = (next_wp == null);

        // --- Set title ---
        if (arrival_title_text != null)
            arrival_title_text.text = $"Aangekomen bij:\n{arrived_name}";

        // --- Set subtitle with next destination + distance ---
        if (arrival_subtitle_text != null)
        {
            if (is_final)
            {
                arrival_subtitle_text.text = "Eindbestemming bereikt!";
            }
            else
            {
                // Calculate distance from current position to next waypoint
                float dist_to_next = 0f;
                if (next_wp.target != null)
                {
                    Vector3 delta = next_wp.target.position - get_player_position();
                    delta.y = 0f;
                    dist_to_next = delta.magnitude;
                }

                string dist_text = format_distance(Mathf.RoundToInt(dist_to_next));
                arrival_subtitle_text.text = $"Volgende: {next_wp.destination_name}  —  {dist_text}";
            }
        }

        // Fade in
        yield return StartCoroutine(fade(0f, 1f, fade_in_duration));

        // Hold
        yield return new WaitForSeconds(display_duration);

        // Fade out
        yield return StartCoroutine(fade(1f, 0f, fade_out_duration));

        // Advance to next waypoint
        if (compass_controller != null)
            compass_controller.advance_to_next_waypoint();

        // If final, load scene
        if (is_final && !string.IsNullOrEmpty(final_scene_name))
        {
            Debug.Log($"Arrival_Panel: Eindbestemming! Scene '{final_scene_name}' laden...");
            yield return new WaitForSeconds(scene_load_delay);

            if (Application.CanStreamedLevelBeLoaded(final_scene_name))
                SceneManager.LoadScene(final_scene_name);
            else
                Debug.LogError($"Scene '{final_scene_name}' niet gevonden in Build Settings!");
        }

        is_showing = false;
    }

    private IEnumerator fade(float from, float to, float duration)
    {
        if (canvas_group == null) yield break;

        float elapsed = 0f;
        canvas_group.alpha = from;
        canvas_group.interactable = false;
        canvas_group.blocksRaycasts = false;

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

    private static string format_distance(int meters)
    {
        if (meters >= 1000)
            return $"{meters / 1000f:0.0} km";
        return $"{meters} m";
    }

    #endregion
}