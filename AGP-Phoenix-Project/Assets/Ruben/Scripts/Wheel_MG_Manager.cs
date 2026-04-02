using Unity.VisualScripting;
using UnityEngine;

public class Wheel_MG_Manager : MonoBehaviour
{
    private RoomHealth roomHealth;
    [SerializeField] private string roomName;
    private void Start()
    {
        roomHealth = GameObject.Find(roomName).GetComponent<RoomHealth>();
    }
    public Winch_BMG WinchBMG;
    public GameObject Wheel_Canvas;
    private void Update()
    {

        if (WinchBMG.CounterClockwiseRotations >= 2f)
        {
            roomHealth.ResetHealth();
            Destroy(Wheel_Canvas);
            Wheel_Canvas.SetActive(false);
        }
    }
}
