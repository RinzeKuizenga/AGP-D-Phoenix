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

    public override void StartEffect()
    {
        Debug.Log("KitchenEffect: StartEffect() called");
        isFeeding = true;
        feedTimer = 0f;
    }

    public override void StopEffect()
    {
        Debug.Log("KitchenEffect: StopEffect() called");
        isFeeding = false;
        feedTimer = 0f;
    }

    private void FeedCrewmate()
    {
        Debug.Log("KitchenEffect: FeedCrewmate() called");

        CrewmateMovement crewmate = CrewmateMovement.selectedCrewmate;

        if (crewmate == null)
        {
            Debug.LogWarning("KitchenEffect: selectedCrewmate is NULL");
            StopEffect();
            return;
        }

        if (crewmate.currentRoom != crewTarget)
        {
            Debug.LogWarning($"KitchenEffect: crewmate.currentRoom ({crewmate.currentRoom}) != crewTarget ({crewTarget})");
            StopEffect();
            return;
        }

        CrewMateStats stats = crewmate.GetComponent<CrewMateStats>();
        if (stats == null)
        {
            Debug.LogWarning("KitchenEffect: CrewmateStats component not found on crewmate!");
            return;
        }

        if (stats.hunger >= stats.maxHunger)
        {
            Debug.Log("KitchenEffect: Crewmate is already full");
            StopEffect();
            return;
        }

        stats.Feed(feedAmount);
        Debug.Log($"KitchenEffect: Fed crewmate, hunger is now {stats.hunger}/{stats.maxHunger}");
    }
}