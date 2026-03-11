using UnityEngine;

public class CrewTarget : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("Ribbit");
        CrewmateMovement[] crew = FindObjectsOfType<CrewmateMovement>();

        foreach (var c in crew)
        {
            c.MoveToArea(transform);
        }
    }
}