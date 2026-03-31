using System;
using UnityEngine;

public class RoomHealth : MonoBehaviour
{
    [Header("Kamer Instellingen")]
    [SerializeField] private string roomName = "Room";
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private float decayPerSecond = 2f;
    [SerializeField] private float pauseDuration = 3f;

    private float currentHealth;
    private bool isDestroyed = false;
    private float pauseTimer = 0f;
    public bool isHealing;

    public event Action<float, float> OnHealthChanged;
    public event Action<RoomHealth> OnRoomDestroyed;

    public string RoomName => roomName;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDestroyed => isDestroyed;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (isDestroyed) return;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        float change = decayPerSecond * Time.deltaTime;

        if (!isHealing)
        {
            currentHealth -= change;
        }
        else
        {
            currentHealth += change;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            isDestroyed = true;
            OnRoomDestroyed?.Invoke(this);
        }
    }
    
    public void PerformAction(float healAmount)
    {
        if (isDestroyed) return;

        pauseTimer = pauseDuration;
        currentHealth += healAmount;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void PerformAction()
    {
        if (isDestroyed) return;
        pauseTimer = pauseDuration;
    }

    public void ResetHealth()
    {
        isDestroyed = false;
        currentHealth = maxHealth;
        pauseTimer = 0f;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}