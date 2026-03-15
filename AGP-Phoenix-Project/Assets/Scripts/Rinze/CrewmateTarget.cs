using UnityEngine;

public class CrewTarget : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] private string currentRoom;
    void OnMouseDown()
    {
        RoomChecker();
        CrewmateMovement[] crew = FindObjectsOfType<CrewmateMovement>();
        foreach (var c in crew)
        {
            c.MoveToArea(target.transform);
        }

    }

    private void RoomChecker()
    {
        switch (gameObject.name)
        {
            case "Kitchen":
                currentRoom = "Kitchen"; break;
            case "Handy":
                currentRoom = "HandymanRoom"; break;
            case "Sail":
                currentRoom = "SailmakerRoom"; break;
            case "Medbay":
                currentRoom = "Medbay"; break;
            case "Hull":
                currentRoom = "Hull"; break;
        }

    }
}