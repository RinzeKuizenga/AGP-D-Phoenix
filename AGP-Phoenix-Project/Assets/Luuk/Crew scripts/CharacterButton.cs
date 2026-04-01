using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    [HideInInspector] public Transform originalParent;
    public Crew crewData;
    private CrewSelectingManager manager;
    [SerializeField] private AudioClip UIClick;

    void Start()
    {
        originalParent = transform.parent;
        manager = FindObjectOfType<CrewSelectingManager>();

        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(UIClick, 1f);
            manager.SelectCrewMember(gameObject, crewData);
        });
    }
}
