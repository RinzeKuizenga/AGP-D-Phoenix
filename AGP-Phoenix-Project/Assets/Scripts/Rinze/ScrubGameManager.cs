using System.Collections;
using UnityEngine;

public class ScrubGameManager : MonoBehaviour
{
    private int totalDirt;
    private int cleanedDirt;
    public GameObject scrubMG;

    public Animator animator;
    private RoomHealth roomhealth;
    [SerializeField] private string roomName;

    void Start()
    {
        roomhealth = GameObject.Find(roomName).GetComponent<RoomHealth>();
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
        roomhealth.ResetHealth();
        Destroy(scrubMG);
    }
}