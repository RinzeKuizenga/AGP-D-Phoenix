using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CrewMateStats))]
public class CrewMateStatsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CrewMateStats stats;

    [Header("Sliders")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider hungerSlider;

    private void Start()
    {
        // Set slider max values once
        healthSlider.maxValue = stats.maxHealth;
        hungerSlider.maxValue = stats.maxHunger;
    }

    private void Update()
    {
        healthSlider.value = stats.health;
        hungerSlider.value = stats.hunger;
        
        UpdateSliderColor(healthSlider, stats.health, stats.maxHealth);
        UpdateSliderColor(hungerSlider, stats.hunger, stats.maxHunger);
    }
    
    private void UpdateSliderColor(Slider slider, float current, float max)
    {
        Image fill = slider.fillRect.GetComponent<Image>();
        float percent = current / max;

        if (percent > 0.5f)
            fill.color = Color.green;
        else if (percent > 0.25f)
            fill.color = Color.yellow;
        else
            fill.color = Color.red;
    }
}
