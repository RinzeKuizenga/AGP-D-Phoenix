using TMPro;
using UnityEngine;
using System.Collections;

public class TextBubble : MonoBehaviour
{
    public static TextBubble Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI text; 
    [SerializeField] private float targetAlpha;
    [SerializeField] public CanvasGroup textBubble;

    private Coroutine currentRoutine;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        textBubble.alpha = Mathf.Lerp(textBubble.alpha, targetAlpha, Time.deltaTime * 8f);
    }

    public void BubbleText(string ftext)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            textBubble.alpha = Mathf.Lerp(0, 0, Time.deltaTime * 8f);
        }
        currentRoutine = StartCoroutine(BubbleRoutine(ftext));
    }

    IEnumerator BubbleRoutine(string ftext)
    {
        text.text = ftext;
        targetAlpha = 1f; 

        yield return new WaitForSeconds(5f);

        targetAlpha = 0f; 
    }

}
