using System.ComponentModel;
using UnityEngine;

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
    [SerializeField] RoomType roomType;
    [SerializeField] public SpriteRenderer hoverImage;
    [SerializeField] public bool isFilled;

    void OnMouseDown()
    {
        if (isFilled || !CrewmateMovement.selectedCrewmate.isSelected)
        {
            TextBubble.Instance.BubbleText("Ik kan hier nich hen.");
            return;
        }

        TextBubble.Instance.BubbleText("Efkes lopen, heur.");

        CrewmateMovement[] crew = FindObjectsOfType<CrewmateMovement>();
        foreach (var c in crew)
        {
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
}