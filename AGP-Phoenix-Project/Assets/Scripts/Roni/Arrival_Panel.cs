using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Shows a fade-in/out arrival notification when the ship reaches any waypoint.
/// All intermediate waypoints (everything except the last) are optional.
/// The last waypoint is the mandatory final destination.
///
/// HIDING APPROACH:
/// The panel GameObject starts DISABLED (SetActive false) so nothing renders.
/// A hidden helper object runs coroutines since disabled objects cannot.
/// When arrival is detected, the panel activates, fades in, holds, fades out,
/// then deactivates again.
///
/// SETUP IN UNITY:
/// 1. This script goes on the panel GameObject that has your Image/background + texts.
/// 2. Add a CanvasGroup component to the SAME GameObject.
/// 3. Assign compass_controller, player_transform, arrival_title_text, arrival_subtitle_text.
/// 4. The panel can be active or inactive in the editor — the script forces it off at runtime.
/// </summary>
public class Arrival_Panel : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField, Tooltip("The Compass_Controller that manages waypoints.")]
    private Compass_Controller compass_controller;

    [SerializeField, Tooltip("Text showing 'Aangekomen bij: [naam]'.")]
    private TextMeshProUGUI arrival_title_text;

    [SerializeField, Tooltip("Text showing next info or final message.")]
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
    [SerializeField, Tooltip("Scene to load after final destination. Leave empty to skip.")]
    private string final_scene_name = "";

    [SerializeField, Tooltip("Delay before loading the final scene.")]
    private float scene_load_delay = 1.5f;

    #endregion

    #region Private State

    private CanvasGroup canvas_group;
    private bool is_showing;
    private Arrival_Panel_Runner runner;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Ensure CanvasGroup exists
        canvas_group = GetComponent<CanvasGroup>();
        if (canvas_group == null)
            canvas_group = gameObject.AddComponent<CanvasGroup>();

        // Set alpha to 0 and disable interaction
        canvas_group.alpha = 0f;
        canvas_group.interactable = false;
        canvas_group.blocksRaycasts = false;

        // Create a hidden runner object on the PARENT so it stays active
        // even when this panel GameObject is disabled
        create_runner();

        // DISABLE the panel GameObject entirely — nothing renders
        gameObject.SetActive(false);
    }

    private void Start()
    {
        if (compass_controller == null)
            Debug.LogError("Arrival_Panel: compass_controller is niet toegewezen!");
        if (player_transform == null)
            Debug.LogWarning("Arrival_Panel: player_transform niet toegewezen!");
    }

    #endregion

    #region Runner (stays active to run coroutines and Update checks)

    private void create_runner()
    {
        // Place the runner on the parent, or at root if no parent
        Transform parent = transform.parent;
        GameObject runner_go;

        if (parent != null)
        {
            // Check if runner already exists
            Transform existing = parent.Find("__arrival_runner__");
            if (existing != null)
            {
                runner = existing.GetComponent<Arrival_Panel_Runner>();
                if (runner != null)
                {
                    runner.setup(this);
                    return;
                }
                Destroy(existing.gameObject);
            }

            runner_go = new GameObject("__arrival_runner__");
            runner_go.transform.SetParent(parent, false);
        }
        else
        {
            runner_go = new GameObject("__arrival_runner__");
        }

        runner_go.hideFlags = HideFlags.HideInHierarchy;
        runner = runner_go.AddComponent<Arrival_Panel_Runner>();
        runner.setup(this);
    }

    /// <summary>Called every frame by the runner object.</summary>
    public void runner_update()
    {
        // Don't check while showing
        if (is_showing) return;

        // Need references
        if (compass_controller == null) return;
        if (player_transform == null) return;

        // Done — final reached
        if (compass_controller.is_final_reached()) return;

        Vector3 player_pos = player_transform.position;
        int count = compass_controller.get_waypoint_count();

        for (int i = 0; i < count; i++)
        {
            if (compass_controller.is_visited(i)) continue;

            Compass_Marker_Data data = compass_controller.get_waypoint_data(i);
            if (data == null || data.target == null) continue;

            Vector3 delta = data.target.position - player_pos;
            delta.y = 0f;

            if (delta.magnitude <= arrival_threshold)
            {
                runner.StartCoroutine(do_arrival(i));
                return;
            }
        }
    }

    #endregion

    #region Arrival Sequence

    private IEnumerator do_arrival(int arrived_index)
    {
        is_showing = true;

        // Mark visited
        compass_controller.mark_visited(arrived_index);

        Compass_Marker_Data arrived = compass_controller.get_waypoint_data(arrived_index);
        string arrived_name = arrived != null ? arrived.destination_name : "Onbekend";
        bool is_final = compass_controller.is_final_index(arrived_index);

        // Set texts BEFORE activating so there's no flash of old text
        if (arrival_title_text != null)
            arrival_title_text.text = "Aangekomen bij:\n" + arrived_name;

        if (arrival_subtitle_text != null)
        {
            if (is_final)
            {
                arrival_subtitle_text.text = "Eindbestemming bereikt!";
            }
            else
            {
                int remaining = compass_controller.get_unvisited_intermediate_count();
                Compass_Marker_Data final_wp = compass_controller.get_final_data();

                string sub = "";
                if (remaining > 0)
                    sub = "Nog " + remaining + " optionele tussenstop(s)\n";

                if (final_wp != null && final_wp.target != null)
                {
                    Vector3 d = final_wp.target.position - get_player_position();
                    d.y = 0f;
                    sub += "Eindbestemming: " + final_wp.destination_name
                         + "  —  " + format_distance(Mathf.RoundToInt(d.magnitude));
                }

                arrival_subtitle_text.text = sub;
            }
        }

        // Ensure alpha is 0 before activating
        canvas_group.alpha = 0f;
        canvas_group.interactable = false;
        canvas_group.blocksRaycasts = false;

        // ACTIVATE the panel GameObject — now it renders but alpha is 0
        gameObject.SetActive(true);

        // Fade in
        yield return fade(0f, 1f, fade_in_duration);

        // Hold
        yield return new WaitForSeconds(display_duration);

        // Fade out
        yield return fade(1f, 0f, fade_out_duration);

        // DEACTIVATE again — fully hidden
        gameObject.SetActive(false);

        // Load scene if final
        if (is_final && !string.IsNullOrEmpty(final_scene_name))
        {
            Debug.Log("Arrival_Panel: Eindbestemming! Scene '" + final_scene_name + "' laden...");
            yield return new WaitForSeconds(scene_load_delay);

            if (Application.CanStreamedLevelBeLoaded(final_scene_name))
                SceneManager.LoadScene(final_scene_name);
            else
                Debug.LogError("Scene '" + final_scene_name + "' niet gevonden in Build Settings!");
        }

        is_showing = false;
    }

    private IEnumerator fade(float from, float to, float duration)
    {
        if (canvas_group == null) yield break;

        float elapsed = 0f;
        canvas_group.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvas_group.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
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
            return (meters / 1000f).ToString("0.0") + " km";
        return meters + " m";
    }

    #endregion
}

/// <summary>
/// Tiny helper that stays active on the parent Canvas and calls
/// Arrival_Panel.runner_update() every frame + hosts coroutines.
/// Hidden in the hierarchy.
/// </summary>
public class Arrival_Panel_Runner : MonoBehaviour
{
    private Arrival_Panel panel;

    public void setup(Arrival_Panel p)
    {
        panel = p;
    }

    private void Update()
    {
        if (panel != null)
            panel.runner_update();
    }
}