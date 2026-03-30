using Unity.VisualScripting;
using UnityEngine;

public class Wheel_MG_Manager : MonoBehaviour
{
    public Winch_BMG WinchBMG;
    public GameObject Wheel_Canvas;
    private void Update()
    {

        if (WinchBMG.CounterClockwiseRotations >= 2f)
        {
            Wheel_Canvas.SetActive(false);
        }
    }
}
