using UnityEngine;

public class BubbleTrailManager : MonoBehaviour
{
    public static BubbleTrailManager Instance;

    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private RectTransform canvasRect;

    [SerializeField] private float bubbleSpacing = 25f;
    [SerializeField] private float bubbleLifetime = 0.4f;

    private Vector2 lastBubblePos;
    private bool isActive = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (!isActive) return;

        Vector2 mousePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            null,
            out mousePos
        );

        if (Vector2.Distance(mousePos, lastBubblePos) < bubbleSpacing) return;

        SpawnBubble(mousePos);
        lastBubblePos = mousePos;
    }

    void SpawnBubble(Vector2 pos)
    {
        GameObject bubble = Instantiate(bubblePrefab, canvasRect);
        RectTransform rt = bubble.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;

        rt.localScale = Vector3.one * Random.Range(0.3f, 0.5f);
        rt.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

        StartCoroutine(FadeBubble(bubble));
    }

    System.Collections.IEnumerator FadeBubble(GameObject bubble)
    {
        CanvasGroup cg = bubble.GetComponent<CanvasGroup>();
        float t = 0f;

        while (t < bubbleLifetime)
        {
            t += Time.deltaTime;
            cg.alpha = 1f - (t / bubbleLifetime);
            yield return null;
        }

        Destroy(bubble);
    }

    public void ActivateTrail()
    {
        isActive = true;
    }

    public void DeactivateTrail()
    {
        isActive = false;
    }
}