// MinigameManager.cs
using UnityEngine;
using UnityEngine.UI;

public class SailMG_Manager : MonoBehaviour
{
    public StitchPath stitchPath;

    void Update()
    {
        if (stitchPath.IsComplete())
        {
            OnWin();
        }
    }

    void OnWin()
    {
        Debug.Log("DIKKE VETTE BANGER");
    }

}