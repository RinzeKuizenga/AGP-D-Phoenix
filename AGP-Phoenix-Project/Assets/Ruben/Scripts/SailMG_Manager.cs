// MinigameManager.cs
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SailMG_Manager : MonoBehaviour
{
    public StitchPath stitchPath;
    private RoomHealth roomHealth;
    [SerializeField] private string roomName;

    private void Start()
    {
        roomHealth = GameObject.Find(roomName).GetComponent<RoomHealth>();
    }
    void Update()
    {
        if (stitchPath.IsComplete())
        {
            OnWin();
        }
    }

    void OnWin()
    {
        roomHealth.ResetHealth();
        Destroy(gameObject);
        Debug.Log("DIKKE VETTE BANGER");
    }

}