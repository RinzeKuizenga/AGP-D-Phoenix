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
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sr;
    private bool effectStarted = false; 
    void Start()
    {
        Debug.Log("START");
        if (currentRoom != null)
        {
            isSelected = true;
            MoveToArea(currentRoom.target, currentRoom);
        }
    }


    // In Update, replace your current movement block with:
    private void Update()
    {
        if (target == null) return;
        popupScreen.alpha = Mathf.Lerp(popupScreen.alpha, targetAlpha, Time.deltaTime * 8f);

        Vector3 movement = target.position - transform.position;

        if (Mathf.Abs(movement.x) > 0.01f)
        {
           if(sr != null) sr.flipX = movement.x > 0f;
        }
        CheckClimbAnimation();
        if (CheckStairs()) return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        distance = Vector3.Distance(transform.position, target.position);


        if (distance < 0.1f)
        {
            if (animator != null) animator.SetBool("isWalking", false);
            if (animator != null) animator.SetBool("isClimbing", false);
            currentRoom.roomHealth.isHealing = true;
            if (!effectStarted)
            {
                effectStarted = true;
                Debug.Log($"CrewmateMovement: Arrived at {currentRoom.gameObject.name}, calling StartEffect");
                currentRoom.StartEffect(this); // ← pass this crewmate in
            }
        }
        else
        {
            effectStarted = false;
            if (animator != null) animator.SetBool("isWalking", true); // moving, play Walk
            if (animator != null) animator.SetBool("isClimbing", false);
            currentRoom.roomHealth.isHealing = false;
            if (previousRoom != null) previousRoom.roomHealth.isHealing = false;
        }
    }

    public bool CheckStairs()
    {
        if ((currentRoom.roomType == RoomType.Hull ||
            (previousRoom != null && previousRoom.roomType == RoomType.Hull))
            && !doneStairs)
        {
            Vector3 dir = stairAnchor.position - transform.position;

            // 🔥 HARD FORCE flip while using stairs
            if (Mathf.Abs(dir.x) > 0.01f && sr != null)
            {
                sr.flipX = dir.x < 0f;
            }

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

    public void CheckClimbAnimation()
    {
        if (currentRoom.roomType == RoomType.Hull && stairDistance < 0.1f)
        {
            if (animator != null) animator.SetBool("isWalking", true); // moving, play Walk
            if (animator != null) animator.SetBool("isClimbing", false);
        }
        else if (currentRoom.roomType != RoomType.Hull && stairDistance > 0.1f)
        {
            if (animator != null) animator.SetBool("isWalking", false); // moving, play Walk
            if (animator != null) animator.SetBool("isClimbing", true);
        }
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
        effectStarted = false;
        doneStairs = false;
        target = newTarget;

        // 🔥 HARDCODE FIX: set correct facing immediately
        float dir = target.position.x - transform.position.x;
        if (sr != null)
        {
            sr.flipX = dir < 0f;
        }

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