using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [Header("Minigame Prefabs")]
    public GameObject minigame1Prefab;
    public GameObject minigame2Prefab;
    public GameObject minigame3Prefab;
    public GameObject minigame4Prefab;
    public GameObject minigame5Prefab;

    private GameObject currentMinigame;

    public void OpenMinigame(int index)
    {
        // Destroy current minigame if one is open
        if (currentMinigame != null)
            Destroy(currentMinigame);

        GameObject prefab = index switch
        {
            1 => minigame1Prefab,   //bucket
            2 => minigame2Prefab,   //Poep
            3 => minigame3Prefab,   //Storage
            4 => minigame4Prefab,   //Wheel
            5 => minigame5Prefab,   //Zeil
            _ => null
        };

        if (prefab != null)
            currentMinigame = Instantiate(prefab);
    }
    public void CloseMinigame()
    {
        if (currentMinigame != null)
            Destroy(currentMinigame);
    }
}