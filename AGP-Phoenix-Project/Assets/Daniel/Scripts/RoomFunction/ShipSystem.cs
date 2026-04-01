using UnityEngine;

public class ShipSystem : MonoBehaviour
{
    public static ShipSystem Instance { get; private set; }

    [Header("Room References")]
    [SerializeField] private RoomHealth sailRoom;
    [SerializeField] private RoomHealth kitchen;
    [SerializeField] private RoomHealth medbay;
    [SerializeField] private RoomHealth handymanRoom;
    
    [Header("Manager")]
    [SerializeField] private BoatHullManager boatHullManager;
    
    // System states
    public bool CanSteer { get; private set; } = true;
    public bool CanHeal { get; private set; } = true;
    public bool CanEat { get; private set; } = true;
    public bool CanRepair { get; private set; } = true;
    public bool IsHullIntact { get; private set; } = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (sailRoom != null)     sailRoom.OnRoomDestroyed     += OnSailRoomDestroyed;
        if (kitchen != null)      kitchen.OnRoomDestroyed      += OnKitchenDestroyed;
        if (medbay != null)       medbay.OnRoomDestroyed       += OnMedbayDestroyed;
        if (handymanRoom != null) handymanRoom.OnRoomDestroyed += OnHandymanRoomDestroyed;
    }

    private void OnDestroy()
    {
        // Always unsubscribe to avoid memory leaks
        if (sailRoom != null)     sailRoom.OnRoomDestroyed     -= OnSailRoomDestroyed;
        if (kitchen != null)      kitchen.OnRoomDestroyed      -= OnKitchenDestroyed;
        if (medbay != null)       medbay.OnRoomDestroyed       -= OnMedbayDestroyed;
        if (handymanRoom != null) handymanRoom.OnRoomDestroyed -= OnHandymanRoomDestroyed;
    }

    private void OnSailRoomDestroyed(RoomHealth room)
    {
        CanSteer = false;
        Debug.Log("SailRoom destroyed — steering disabled!");
        CheckAllRoomsDestroyed();
    }

    private void OnKitchenDestroyed(RoomHealth room)
    {
        CanEat = false;
        Debug.Log("Kitchen destroyed — crew cannot eat!");
        CheckAllRoomsDestroyed();
    }

    private void OnMedbayDestroyed(RoomHealth room)
    {
        CanHeal = false;
        Debug.Log("Medbay destroyed — crew cannot heal!");
        CheckAllRoomsDestroyed();
    }

    private void OnHandymanRoomDestroyed(RoomHealth room)
    {
        CanRepair = false;
        Debug.Log("HandymanRoom destroyed — repairs disabled!");
        CheckAllRoomsDestroyed();
    }

    private void OnHullDestroyed(RoomHealth room)
    {
        IsHullIntact = false;
        Debug.Log("Hull destroyed — ship is sinking!");
    }

    private void CheckAllRoomsDestroyed()
    {
        bool allDestroyed = (sailRoom     == null || sailRoom.IsDestroyed)     &&
                            (kitchen      == null || kitchen.IsDestroyed)      &&
                            (medbay       == null || medbay.IsDestroyed)       &&
                            (handymanRoom == null || handymanRoom.IsDestroyed);
        
        if (allDestroyed)
        {
            IsHullIntact = false;
            Debug.Log("All rooms destroyed — ship is sinking!");
            
            if (boatHullManager != null)
                foreach (var section in boatHullManager.hullSections)
                    section.ApplyDamage(float.MaxValue);
        }
    }
}