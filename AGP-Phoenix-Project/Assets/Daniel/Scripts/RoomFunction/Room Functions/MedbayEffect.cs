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

    public override void StartEffect()
    {
        isHealing = true;
        healTimer = 0f;
    }

    public override void StopEffect()
    {
        isHealing = false;
        healTimer = 0f;
    }

    private void HealCrewmate()
    {
        CrewMateStats stats = GetCrewmateStats();
        if (stats == null) { StopEffect(); return; }

        if (stats.health >= stats.maxHealth) { StopEffect(); return; }

        stats.Heal(healAmount);
        Debug.Log($"{CrewmateMovement.selectedCrewmate.name} healed to {stats.health}/{stats.maxHealth}");
    }
}