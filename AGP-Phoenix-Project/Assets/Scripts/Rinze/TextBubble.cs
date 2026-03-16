using TMPro;
using UnityEngine;
using System.Collections;
public class TextBubble : MonoBehaviour
{
    public static TextBubble Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float targetAlpha;
    [SerializeField] public CanvasGroup textBubble;
    [SerializeField] public Transform bubbleTransform;

    private Vector3 bubbleRotation;


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

    void LateUpdate()
    {
        Vector3 rot = bubbleTransform.eulerAngles;

        rot.z = -90f;

        bubbleTransform.eulerAngles = rot;
    }

    void Update()
    {
        textBubble.alpha = Mathf.Lerp(textBubble.alpha, targetAlpha, Time.deltaTime * 8f);
    }

    public void BubbleText(string type)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            textBubble.alpha = Mathf.Lerp(0, 0, Time.deltaTime * 8f);
        }
        currentRoutine = StartCoroutine(BubbleRoutine(type));
    }

    IEnumerator BubbleRoutine(string type)
    {
        switch (type)
        {
            case "Travel":
                TravelDialogue();
                break;
            case "Denied":
                DeniedDialogue();
                break;
        }
        targetAlpha = 1f;

        yield return new WaitForSeconds(5f);

        targetAlpha = 0f;
    }

    void TravelDialogue()
    {
        switch (Random.Range(0, 5))
        {
            case 0:
                text.text = "<+spread amplitude = 0.1> Ik kan hier nich hen.";
                break;
            case 1:
                text.text = "<+spread amplitude = 0.1> Die kaante kan ’k nich op.";
                break;
            case 2:
                text.text = "<+spread amplitude = 0.1> Die kamer is al vol, mien jong.";
                break;
            case 3:
                text.text = "<+spread amplitude = 0.1> Dat geet nich gebeuren.";
                break;
            case 4:
                text.text = "<+spread amplitude = 0.1> Dat kan ik nich.";
                break;
            case 5:
                text.text = "<+spread amplitude = 0.1> ’k Kan daor nich hen.";
                break;
        }
    }

    void DeniedDialogue()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                text.text = "<+spread amplitude = 0.1> Efkes lopen, heur.";
                break;
            case 1:
                text.text = "<+spread amplitude = 0.1> k Gao die kaante op.";
                break;
            case 2:
                text.text = "<+spread amplitude = 0.1> Efkes daorhen.";
                break;
            case 3:
                text.text = "<+spread amplitude = 0.1> n Niej taakske lig veur mie.";
                break;
            case 4:
                text.text = "<+spread amplitude = 0.1> Misskien mut ’k daorhen";
                break;
        }
    }
}
