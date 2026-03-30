using UnityEngine;

public class StitchInput : MonoBehaviour
{
    public StitchPath stitchPath;
    public Camera mainCamera;
    public Canvas canvas;

    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) isDragging = true;
        if (Input.GetMouseButtonUp(0)) isDragging = false;

        if (isDragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                Input.mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
                out Vector2 localPoint
            );

            Debug.Log($"Local pos: {localPoint}, Target: {stitchPath.stitchPoints[stitchPath.GetCurrentTarget()].localPosition}");

            stitchPath.TryStitch(localPoint);
        }
    }
}