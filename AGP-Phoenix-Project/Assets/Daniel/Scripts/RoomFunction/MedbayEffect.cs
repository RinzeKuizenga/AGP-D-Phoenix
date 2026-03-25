using UnityEngine;

public class MedbayEffect : MonoBehaviour
{
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private float healInterval = 1f;
    [SerializeField] private CrewTarget crewTarget;

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

    // Hook this into CrewTarget's onEnter UnityEvent in the Inspector
    public void StartHealing()
    {
        isHealing = true;
        healTimer = 0f;
    }

    public void StopHealing()
    {
        isHealing = false;
        healTimer = 0f;
    }

    private void HealCrewmate()
    {
        CrewmateMovement crewmate = CrewmateMovement.selectedCrewmate;

        if (crewmate == null || crewmate.currentRoom != crewTarget)
        {
            StopHealing();
            return;
        }

        crewmate.health = Mathf.Min(crewmate.health + healAmount, crewmate.maxHealth);
        Debug.Log($"{crewmate.name} healed to {crewmate.health}/{crewmate.maxHealth}");
    }
}