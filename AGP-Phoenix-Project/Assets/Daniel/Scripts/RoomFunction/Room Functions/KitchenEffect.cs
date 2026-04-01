using UnityEngine;

public class KitchenEffect : RoomEffect
{
    [SerializeField] private float feedAmount = 10f;
    [SerializeField] private float feedInterval = 1f;

    private float feedTimer;
    private bool isFeeding;
    
    private void Update()
    {
        if (!isFeeding) return;

        feedTimer += Time.deltaTime;
        if (feedTimer >= feedInterval)
        {
            feedTimer = 0f;
            FeedCrewmate();
        }
    }

    public override void StartEffect(CrewmateMovement crewmate)
    {
        
        if (!ShipSystem.Instance.CanEat)
        {
            Debug.Log("Kitchen is destroyed, cannot eat!");
            return;
        }
        
        base.StartEffect(crewmate);

        Debug.Log($"KitchenEffect: cachedStats = {cachedStats}");  // ← is this null?
        Debug.Log($"KitchenEffect: crewmate = {crewmate}");        // ← is crewmate null?

        if (cachedStats == null) return;

        cachedStats.isBeingFed = true;
        Debug.Log($"KitchenEffect: isBeingFed set to {cachedStats.isBeingFed}");

        isFeeding = true;
        feedTimer = 0f;
    }

    public override void StopEffect()
    {
        if (cachedStats != null)
        {
            cachedStats.isBeingFed = false;
            cachedStats = null; // ← clear when done
        }

        isFeeding = false;
        feedTimer = 0f;
    }

    private void FeedCrewmate()
    {
        if (cachedStats == null) { StopEffect(); return; }
        if (cachedStats.hunger >= cachedStats.maxHunger) { StopEffect(); return; }

        cachedStats.Feed(feedAmount);
        Debug.Log($"KitchenEffect: Fed crewmate, hunger now {cachedStats.hunger}/{cachedStats.maxHunger}");
    }
}