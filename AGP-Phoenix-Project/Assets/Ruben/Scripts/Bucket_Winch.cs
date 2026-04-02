using UnityEngine;
public class Bucket_Winch : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;
    [SerializeField] private float rotationOffset = -90f;
    private float totalRotation = 0f;
    private float lastAngle = 0f;
    private bool firstFrame = true;
    public float FullRotations => totalRotation / 360f;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
    void Update()
    {
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
                Debug.Log($"Total Degrees: {totalRotation:F1} | Full Rotations: {FullRotations:F2}");
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