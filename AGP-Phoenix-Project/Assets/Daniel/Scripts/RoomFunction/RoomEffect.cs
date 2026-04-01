using System;
using UnityEngine;

public abstract class RoomEffect : MonoBehaviour
{
    [SerializeField] protected CrewTarget crewTarget;
    protected CrewmateMovement occupant;
    [NonSerialized] protected CrewMateStats cachedStats; 
    
    public virtual void StartEffect(CrewmateMovement crewmate)
    {
        occupant = crewmate;
        cachedStats = crewmate.GetComponentInChildren<CrewMateStats>(); // ← change this
    
        if (cachedStats == null)
            Debug.LogWarning($"RoomEffect: Still null! Check {crewmate.name}'s hierarchy for CrewMateStats");
        else
            Debug.Log($"RoomEffect: Found CrewMateStats on {cachedStats.gameObject.name}");
    }
    public abstract void StopEffect();

    protected bool IsCrewmatePresent()
    {
        return occupant != null && occupant.currentRoom == crewTarget;
    } 
}
