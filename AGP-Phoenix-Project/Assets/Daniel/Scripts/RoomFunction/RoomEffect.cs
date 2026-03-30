using UnityEngine;

public abstract class RoomEffect : MonoBehaviour
{
    [SerializeField] protected CrewTarget crewTarget;

    public abstract void StartEffect();
    public abstract void StopEffect();

    protected CrewMateStats GetCrewmateStats()
    {
        CrewmateMovement crewmate = CrewmateMovement.selectedCrewmate;
        if (crewmate == null || crewmate.currentRoom != crewTarget) return null;

        return crewmate.GetComponent<CrewMateStats>();
    }

    protected bool IsCrewmatePresent()
    {
        CrewmateMovement crewmate = CrewmateMovement.selectedCrewmate;
        return crewmate != null && crewmate.currentRoom == crewTarget;
    }
}
