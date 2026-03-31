using UnityEngine;

public class CrewMateStats : MonoBehaviour
{
    [Header("Health")]
    public float health = 100f;
    public float maxHealth = 100f;

    [Header("Hunger")]
    public float hunger = 100f;
    public float maxHunger = 100f;
    public float hungerDecayRate = 1f; // per second
    
    public bool isBeingFed = false;
    
    private void Update()
    {
        DecayHunger();
    }

    private void DecayHunger()
    {
        if (isBeingFed) return;
        
        hunger = Mathf.Max(hunger - hungerDecayRate * Time.deltaTime, 0f);

        if (hunger <= 0f)
            health = Mathf.Max(health - Time.deltaTime, 0f);
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        health = Mathf.Max(health - amount, 0f);
    }

    public void Feed(float amount)
    {
        hunger = Mathf.Min(hunger + amount, maxHunger);
    }

    public bool IsDead() => health <= 0f;
    public bool IsStarving() => hunger <= 0f;
}