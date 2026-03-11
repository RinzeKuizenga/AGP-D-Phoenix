using UnityEngine;

public class CrewFollow : MonoBehaviour
{
    [SerializeField] public Transform crewPosition;
    public Transform boat;
    
    void Update()
    {
        crewPosition.transform.position = boat.position;
        crewPosition.transform.rotation = boat.rotation;
    }
}
