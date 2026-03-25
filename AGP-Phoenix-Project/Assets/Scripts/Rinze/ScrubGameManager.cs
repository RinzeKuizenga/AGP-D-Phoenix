using UnityEngine;

public class ScrubGameManager : MonoBehaviour
{
    private int totalDirt;
    private int cleanedDirt;

    void Start()
    {
        ScrubClean[] allDirt = FindObjectsOfType<ScrubClean>();
        totalDirt = allDirt.Length;

        foreach (var dirt in allDirt)
        {
            dirt.OnCleaned += HandleCleaned;
        }
    }

    void HandleCleaned(ScrubClean dirt)
    {
        cleanedDirt++;

        if (cleanedDirt >= totalDirt)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        Debug.Log("You win!");
        // TODO: play sound, animation, next minigame, etc.
    }
}