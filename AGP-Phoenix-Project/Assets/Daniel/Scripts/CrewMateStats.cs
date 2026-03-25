using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrewMateStats : MonoBehaviour
{
    [Header("Refs")] 
    [SerializeField] private CrewmateMovement crewmateMovement;
    [SerializeField] private Crew crewData;
    [SerializeField] private TextMeshProUGUI crewname;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider hungerSlider;
    
    private float maxHealth;

    private void Start()
    {
        maxHealth = crewData.crewHealth;
        healthSlider.maxValue = maxHealth;
    }

    public void DealDamageToCrew()
    {
        maxHealth -= crewData.crewDamageReceived;
        healthSlider.value = maxHealth;
    }
    
}
