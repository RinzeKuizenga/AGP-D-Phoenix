using UnityEngine;
public class ScrubClean : MonoBehaviour
{
    [SerializeField] private CanvasGroup dirtCanvas;
    [SerializeField] private RectTransform scrubArea;
    [SerializeField] private float scrubThreshold = 100f;
    [SerializeField] private float cleanSpeed = 0.5f;
    private Vector3 lastMousePos;
    private bool isCleaned = false;
    public System.Action<ScrubClean> OnCleaned;

    void Update()
    {
        if (isCleaned) return;

        float speed = (Input.mousePosition - lastMousePos).magnitude / Time.deltaTime;

        if (RectTransformUtility.RectangleContainsScreenPoint(scrubArea, Input.mousePosition))
        {
            if (speed > scrubThreshold)
            {
                BubbleTrailManager.Instance.ActivateTrail();

                dirtCanvas.alpha -= cleanSpeed * Time.deltaTime;
                dirtCanvas.alpha = Mathf.Clamp01(dirtCanvas.alpha);
            }
            else
            {
                BubbleTrailManager.Instance.DeactivateTrail();
            }
        }

        if (dirtCanvas.alpha <= 0.01f && !isCleaned)
        {
            isCleaned = true;

            BubbleTrailManager.Instance.DeactivateTrail();

            OnCleaned?.Invoke(this);

            gameObject.SetActive(false);
        }

        lastMousePos = Input.mousePosition;
    }
}
