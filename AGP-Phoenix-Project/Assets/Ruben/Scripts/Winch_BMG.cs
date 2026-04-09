using UnityEngine;
using System.Collections;

public class Winch_BMG : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    [SerializeField] private float rotationOffset = -90f;
    private float totalRotation = 0f;
    private float lastAngle = 0f;
    private bool firstFrame = true;
    public float FullRotations => totalRotation / 360f;
    public float CounterClockwiseRotations => Mathf.Abs(Mathf.Max(totalRotation, 0f)) / 360f;
    // ── Milestone System ─────────────────────────────────────────────────────
    private DraggableItem milestoneScript;
    private const float ROTATIONS_TO_ACTIVATE = 2f;
    private float lastCheckedRotations = 0f;
    private bool milestoneReached = false;
    [SerializeField] private bool requiresNegativeRotation = false;
    public bool CounterClockwiseComplete { get; private set; } = false;
    // ─────────────────────────────────────────────────────────────────────────

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        milestoneScript = GetComponent<DraggableItem>();

        if (milestoneScript != null)
        {
            if (requiresNegativeRotation)
                milestoneScript.enabled = true;
            else
                milestoneScript.enabled = false;
        }

        if (requiresNegativeRotation)
            milestoneReached = true;
    }

    public void Activate()
    {
        StartCoroutine(ActivateNextFrame());
    }

    private IEnumerator ActivateNextFrame()
    {
        yield return null;

        milestoneReached = false;
        lastCheckedRotations = 0f;
        totalRotation = 0f;

        if (milestoneScript != null)
            milestoneScript.enabled = false;

        Debug.Log("Second winch activated — spin counter-clockwise!");
    }

    private void CheckRotationMilestone()
    {
        if (milestoneReached) return;

        float rotations = FullRotations;
        float lastChecked = lastCheckedRotations;

        bool thresholdCrossed = requiresNegativeRotation
            ? (rotations <= -ROTATIONS_TO_ACTIVATE && lastChecked > -ROTATIONS_TO_ACTIVATE)
            : (Mathf.Abs(rotations) >= ROTATIONS_TO_ACTIVATE && Mathf.Abs(lastChecked) < ROTATIONS_TO_ACTIVATE);

        if (thresholdCrossed)
        {
            milestoneReached = true;

            if (requiresNegativeRotation)
                CounterClockwiseComplete = true;

            if (milestoneScript != null)
                milestoneScript.enabled = true;

            Debug.Log("Milestone reached — DraggableItem enabled, Winch stopped.");
        }

        lastCheckedRotations = rotations;
    }

    public void OnHold()
    {
        if (milestoneReached) return;

        Vector2 spriteScreenPos = RectTransformUtility.WorldToScreenPoint(
            canvas.worldCamera,
            rectTransform.position
        );
        Vector2 direction = (Vector2)Input.mousePosition - spriteScreenPos;

        if (direction.magnitude > 5f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;

            if (!firstFrame)
            {
                float delta = Mathf.DeltaAngle(lastAngle, angle);
                totalRotation += delta;
                totalRotation = Mathf.Clamp(totalRotation, -1800f, 1800f);

                CheckRotationMilestone();
            }

            lastAngle = angle;
            firstFrame = false;

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            rectTransform.localRotation = Quaternion.RotateTowards(
                rectTransform.localRotation,
                targetRotation,
                2000f * Time.deltaTime
            );
        }
        else
        {
            firstFrame = true;
        }
    }
}