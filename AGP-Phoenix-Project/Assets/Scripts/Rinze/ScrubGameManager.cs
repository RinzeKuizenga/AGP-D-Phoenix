using System.Collections;
using UnityEngine;

public class ScrubGameManager : MonoBehaviour
{
    private int totalDirt;
    private int cleanedDirt;

    [SerializeField] public Animator animator;

    void Start()
    {
        ScrubClean[] allDirt = FindObjectsOfType<ScrubClean>();
        totalDirt = allDirt.Length;

        foreach (var dirt in allDirt)
        {
            Debug.Log("Assigned");
            dirt.OnCleaned += HandleCleaned;
        }
    }

    void HandleCleaned(ScrubClean dirt)
    {
        cleanedDirt++;

        if (cleanedDirt >= totalDirt)
        {
            Debug.Log($"Cleaned: {cleanedDirt}");
            WinGame();
        }
    }

    void WinGame()
    {
        StartCoroutine(SparkleAndQuit());
    }

    IEnumerator SparkleAndQuit()
    {
        animator.SetTrigger("sparkle");
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);  
    }
}