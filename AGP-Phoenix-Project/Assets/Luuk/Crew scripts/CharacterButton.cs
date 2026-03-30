using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    [HideInInspector] public Transform originalParent;
    public Crew crewData;
    private CrewSelectingManager manager;

    void Start()
    {
        originalParent = transform.parent;
        manager = FindObjectOfType<CrewSelectingManager>();

        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            manager.SelectCrewMember(gameObject, crewData);
        });
    }
}
