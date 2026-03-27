using Unity.VisualScripting;
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
    [SerializeField] Transform popupTransform;
    [SerializeField] float targetAlpha;
    [SerializeField] public TextBubble textBubble;
    [SerializeField] public float distance;
    [SerializeField] public float stairDistance;

    public bool isSelected = false;

    [SerializeField] public Transform stairAnchor;
    [SerializeField] public bool doneStairs = false;

    [SerializeField] private AudioClip buildSound;


    private void Update()
    {
        if (target == null) return;
        popupScreen.alpha = Mathf.Lerp(popupScreen.alpha, targetAlpha, Time.deltaTime * 8f);
        if (CheckStairs()) return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        distance = Vector3.Distance(transform.position, target.position);

        if (distance < 0.1f)
        {
            currentRoom.roomHealth.isHealing = true;
        }
        else
        {
            currentRoom.roomHealth.isHealing = false;
            if(previousRoom != null) previousRoom.roomHealth.isHealing = false;
        }
    }

    public bool CheckStairs()
    {
        if ((currentRoom.roomType == RoomType.Hull ||
            (previousRoom != null && previousRoom.roomType == RoomType.Hull))
            && !doneStairs)
        {
            transform.position = Vector3.MoveTowards(transform.position, stairAnchor.position, speed * Time.deltaTime);

            stairDistance = Vector3.Distance(transform.position, stairAnchor.position);

            if (stairDistance < 0.1f)
            {
                doneStairs = true;
            }

            return true; 
        }

        return false; 
    }

    public void ShowBubble(string type)
    {
        textBubble.BubbleText(type);
    }

    void LateUpdate()
    {
        Vector3 rot = popupTransform.eulerAngles;

        rot.z = -106f;

        popupTransform.eulerAngles = rot;
    }

    public void MoveToArea(Transform newTarget, CrewTarget room)
    {
        if (!isSelected) return;

        if (currentRoom != null)
        {
            RoomEffect oldEffect = currentRoom.GetComponent<RoomEffect>();
            if (oldEffect != null)
            {
                oldEffect.StopEffect();
            }
            
            currentRoom.isFilled = false;
            currentRoom.hoverImage.color = new Color(0f, 0.7f, 0f, 0f);
        }

        previousRoom = currentRoom;
        currentRoom = room;
        currentRoom.isFilled = true;
        currentRoom.hoverImage.color = new Color(0.7f, 0f, 0f, 0f);

        doneStairs = false;
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