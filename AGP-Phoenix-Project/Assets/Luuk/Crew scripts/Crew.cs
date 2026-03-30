using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "NewCrewMember", menuName = "CrewMember")]
public class Crew : ScriptableObject
{
    public Sprite sprite;
    public Sprite SpecialtyIcon;
    public string specialtyText;
    public string specialty;
    public int crewSkillLevel;
    public string crewname;
    public int crewSpeed;
    public float crewHealth;
    public float crewHunger;
    public float crewDamageReceived;

}
