using UnityEngine;

public class MedbayEffect : RoomEffect
{
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private float healInterval = 1f;

    private float healTimer;
    private bool isHealing;

    private void Update()
    {
        if (!isHealing) return;

        healTimer += Time.deltaTime;
        if (healTimer >= healInterval)
        {
            healTimer = 0f;
            HealCrewmate();
        }
    }

    public override void StartEffect(CrewmateMovement crewmate)
    {
        if (!ShipSystem.Instance.CanHeal)
        {
            Debug.Log("Medbay is destroyed, cannot heal!");
            return;
        }
        
        base.StartEffect(crewmate);
        
        if (cachedStats == null)
        {
            Debug.LogWarning("MedbayEffect: No CrewMateStats found on crewmate!");
            return;
        }

        isHealing = true;
        healTimer = 0f;
    }

    public override void StopEffect()
    {
        cachedStats = null;
        isHealing = false;
        healTimer = 0f;
    }

    private void HealCrewmate()
    {
        if (cachedStats == null) { StopEffect(); return; }
        if (cachedStats.health >= cachedStats.maxHealth) { StopEffect(); return; }

        cachedStats.Heal(healAmount);
        Debug.Log($"MedbayEffect: Healed crewmate to {cachedStats.health}/{cachedStats.maxHealth}");
    }
}