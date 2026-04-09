using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Bucket_Winch : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Slider sliderRope;

    [SerializeField] private float rotationOffset = -90f;
    private float totalRotation = 0f;
    private float lastAngle = 0f;
    private bool isDragging = false;

    public float FullRotations => totalRotation / 360f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        sliderRope = GetComponentInParent<Transform>().root.GetComponentInChildren<Slider>(true);

        if (sliderRope != null)
        {
            sliderRope.minValue = -5f;
            sliderRope.maxValue = 5f;
            sliderRope.value = 0f;
        }

        Debug.Log("Slider found: " + sliderRope);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        lastAngle = GetAngle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        float angle = GetAngle(eventData.position);
        float delta = Mathf.DeltaAngle(lastAngle, angle);
        totalRotation += delta;
        totalRotation = Mathf.Clamp(totalRotation, -1800f, 1800f);
        lastAngle = angle;

        if (sliderRope != null)
            sliderRope.value = FullRotations;

        Debug.Log($"Total Degrees: {totalRotation:F1} | Full Rotations: {FullRotations:F2}");

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        rectTransform.localRotation = Quaternion.RotateTowards(
            rectTransform.localRotation,
            targetRotation,
            2000f * Time.deltaTime
        );
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    private float GetAngle(Vector2 pointerScreenPos)
    {
        Vector2 spriteScreenPos = RectTransformUtility.WorldToScreenPoint(
            canvas.worldCamera,
            rectTransform.position
        );
        Vector2 direction = pointerScreenPos - spriteScreenPos;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;
    }
}