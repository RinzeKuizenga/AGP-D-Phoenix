using UnityEngine;

public class CrewTarget : MonoBehaviour
{
    [SerializeField] public Transform target;
    void OnMouseDown()
    {
        Debug.Log("Ribbit");
        CrewmateMovement[] crew = FindObjectsOfType<CrewmateMovement>();
        foreach (var c in crew)
        {
            c.MoveToArea(target.transform);
        }

    }
}