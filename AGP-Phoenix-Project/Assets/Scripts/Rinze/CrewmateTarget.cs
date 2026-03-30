using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public enum RoomType
{
    Kitchen,
    HandymanRoom,
    SailRoom,
    Medbay,
    Hull
}
public class CrewTarget : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] public RoomType roomType;
    [SerializeField] public SpriteRenderer hoverImage;
    [SerializeField] public bool isFilled;
    [SerializeField] public RoomHealth roomHealth;
    
    [Header("Events")]
    public UnityEvent onEnter;

    void OnMouseDown()
    {
        if (isFilled || !CrewmateMovement.selectedCrewmate.isSelected)
        {
            CrewmateMovement.selectedCrewmate.ShowBubble("Travel");
            return;
        }

        CrewmateMovement.selectedCrewmate.ShowBubble("Denied");

        CrewmateMovement[] crew = FindObjectsOfType<CrewmateMovement>();
        foreach (var c in crew)
        {
            if (CrewmateMovement.selectedCrewmate == null) return;
            CrewmateMovement.selectedCrewmate.MoveToArea(target.transform, this);
        }

    }

    private void OnMouseEnter()
    {
        if (CrewmateMovement.selectedCrewmate == null || !CrewmateMovement.selectedCrewmate.isSelected)
            return;

        SpriteRenderer sr = hoverImage.GetComponent<SpriteRenderer>();

        Color c = sr.color;
        c.a = 0.50f;
        sr.color = c;
    }

    private void OnMouseExit()
    {
        if (CrewmateMovement.selectedCrewmate == null || !CrewmateMovement.selectedCrewmate.isSelected)
            return;

        SpriteRenderer sr = hoverImage.GetComponent<SpriteRenderer>();

        Color c = sr.color;
        c.a = 0f;
        sr.color = c;
    }

    public void StartEffect()
    {
        onEnter.Invoke();
    }
}