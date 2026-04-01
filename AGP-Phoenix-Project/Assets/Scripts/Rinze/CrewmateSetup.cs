using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrewmateSetup : MonoBehaviour
{
    [Header("Slot index 0–4, matches selection order")]
    [SerializeField] int slotIndex;

    [Header("Popup UI references")]
    [SerializeField] Image portraitImage;      // the big portrait Image
    [SerializeField] Image specialtyIconImage; // the small specialty icon Image
    [SerializeField] TMP_Text nameText;        // the NameText TMP component

    void Awake()
    {
        Crew crew = CrewRoster.SelectedCrew[slotIndex];
        if (crew == null) return;

        if (portraitImage != null) portraitImage.sprite = crew.sprite;
        if (specialtyIconImage != null) specialtyIconImage.sprite = crew.SpecialtyIcon;
        if (nameText != null) nameText.text = crew.crewname;
    }
}