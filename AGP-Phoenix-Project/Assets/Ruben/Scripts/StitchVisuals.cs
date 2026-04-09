using UnityEngine;
using UnityEngine.UI;

public class StitchVisuals : MonoBehaviour
{
    public StitchPath stitchPath;
    public Transform[] stitchPoints;
    public RectTransform canvas;
    public Sprite stitchSprite;
    private GameObject previewLine;

    private int lastStitched = 0;

    void Update()
    {
        int current = Mathf.FloorToInt(stitchPath.GetProgress() * stitchPoints.Length);
        while (lastStitched < current - 1 && lastStitched < stitchPoints.Length - 1)
        {
            SpawnStitchLine(lastStitched, lastStitched + 1);
            lastStitched++;
        }
        UpdatePreviewLine();
    }

    void SpawnStitchLine(int fromIndex, int toIndex)
    {
        Vector2 a = stitchPoints[fromIndex].localPosition;
        Vector2 b = stitchPoints[toIndex].localPosition;

        // Create a UI GameObject
        GameObject line = new GameObject($"Stitch_{fromIndex}_{toIndex}", typeof(RectTransform), typeof(Image));
        line.transform.SetParent(canvas, false);

        // Style it
        Image img = line.GetComponent<Image>();
        img.color = Color.black; // thread color
        if (stitchSprite != null) img.sprite = stitchSprite;

        // Position it between the two points
        RectTransform rt = line.GetComponent<RectTransform>();
        rt.localPosition = (a + b) / 2f;

        // Size: width = distance between points, height = thickness
        float dist = Vector2.Distance(a, b);
        rt.sizeDelta = new Vector2(dist, 6f); // 6px thick

        // Rotate to point from a to b
        float angle = Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void UpdatePreviewLine()
    {
        if (stitchPath.IsComplete() || !Input.GetMouseButton(0))
        {
            if (previewLine != null) previewLine.SetActive(false);
            return;
        }

        if (stitchPath.IsComplete())
        {
            if (previewLine != null) previewLine.SetActive(false);
            return;
        }

        int currentTarget = stitchPath.GetCurrentTarget();

        if (currentTarget == 0)
        {
            if (previewLine != null) previewLine.SetActive(false);
            return;
        }

        if (previewLine == null)
        {
            previewLine = new GameObject("PreviewLine", typeof(RectTransform), typeof(Image));
            previewLine.transform.SetParent(canvas, false);
            Image img = previewLine.GetComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.4f);
            if (stitchSprite != null) img.sprite = stitchSprite;
        }

        previewLine.SetActive(true);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas,
            Input.mousePosition,
            null,
            out Vector2 mouseLocal
        );

        Vector2 from = stitchPoints[currentTarget - 1].localPosition;
        Vector2 to = mouseLocal;

        RectTransform rt = previewLine.GetComponent<RectTransform>();
        rt.localPosition = (from + to) / 2f;

        float dist = Vector2.Distance(from, to);
        rt.sizeDelta = new Vector2(dist, 6f);

        float angle = Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
    }
}