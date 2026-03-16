using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class CrewmateMovement : MonoBehaviour
{
    Transform target;
    [SerializeField] float speed = 1f;
    public CrewTarget currentRoom;  
    public CrewTarget previousRoom;  
    [SerializeField] public static CrewmateMovement selectedCrewmate;

    [SerializeField] CanvasGroup popupScreen;
    [SerializeField] float targetAlpha;

    public bool isSelected = false;

    private void Update()
    {
        popupScreen.alpha = Mathf.Lerp(popupScreen.alpha, targetAlpha, Time.deltaTime * 8f);
        if (target == null) return;
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

    }

    public void MoveToArea(Transform newTarget, CrewTarget room)
    {
        if (!isSelected) return; 
        
        if (currentRoom != null)
        {
            currentRoom.isFilled = false;
            currentRoom.hoverImage.color = new Color(0f, 0.7f, 0f, 0f);
        }

        previousRoom = currentRoom;
        currentRoom = room;
        currentRoom.isFilled = true;
        currentRoom.hoverImage.color = new Color(0.7f, 0f, 0f, 0f);

        target = newTarget;
        isSelected = false;
        selectedCrewmate = null;
    }


    void OnMouseDown()
    {
        isSelected = true;
        selectedCrewmate = this;
    }

    void OnMouseEnter()
    {
        targetAlpha = 1f;
    }

    void OnMouseExit()
    {
        targetAlpha = 0f;
    }
}