using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "NewCrewMember", menuName = "CrewMember")]
public class Crew : ScriptableObject
{
    public Sprite sprite;
    public Sprite SpecialtyIcon;
    public string crewclass;
    public string crewname;
    public int crewSpeed;

}
