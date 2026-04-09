using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomHealthUI : MonoBehaviour
{
    [Header("UI Elementen")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Kleuren")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;

    [SerializeField] private GameObject fixButton;

    private RoomHealth roomHealth;

    private void Awake()
    {
        roomHealth = GetComponent<RoomHealth>();
    }

    private void OnEnable()
    {
        roomHealth.OnHealthChanged += UpdateUI;
        roomHealth.OnRoomDestroyed += HandleDestroyed;
    }

    private void OnDisable()
    {
        roomHealth.OnHealthChanged -= UpdateUI;
        roomHealth.OnRoomDestroyed -= HandleDestroyed;
    }

    private void Start()
    {
        if (roomNameText != null)
            roomNameText.text = roomHealth.RoomName;

        UpdateUI(roomHealth.CurrentHealth, roomHealth.MaxHealth);
    }

    private void UpdateUI(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";

        if (fillImage != null)
        {
            float pct = current / max;
            if (pct > 0.5f)
                fillImage.color = Color.Lerp(warningColor, healthyColor, (pct - 0.5f) / 0.5f);
            else
                fillImage.color = Color.Lerp(criticalColor, warningColor, pct / 0.5f);
        }

        if(healthSlider.value <= 0)
        {
            fixButton.SetActive(true);
        }
        else
        {
            fixButton.SetActive(false);
        }
    }

    private void HandleDestroyed(RoomHealth room)
    {
        if (healthText != null)
        {
            healthText.text = "KAPOT!";
            healthText.color = criticalColor;
        }
    }
}