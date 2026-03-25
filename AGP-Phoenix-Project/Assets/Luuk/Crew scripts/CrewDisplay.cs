
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrewDisplay : MonoBehaviour
{
    public Crew crewMate;

    public Image crewImage;
    public Image crewIcon;
    public TextMeshProUGUI crewclass;
    public TextMeshProUGUI crewname;
    public int crewSpeed;
    void Start()
    {
        crewname.text = crewMate.crewname;
        crewclass.text = crewMate.specialtyText;
    crewImage.sprite = crewMate.sprite;
        crewIcon.sprite = crewMate.SpecialtyIcon;
    }

   
}
