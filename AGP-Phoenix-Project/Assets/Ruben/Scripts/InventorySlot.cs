using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private bool isActivationSlot = false; // check this on the specific slot only

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform;

            // ── Activate winch if this is the activation slot and ItemID matches ──
            if (isActivationSlot && draggableItem.ItemID == 1)
            {
                Winch_BMG winch = dropped.GetComponent<Winch_BMG>();
                if (winch != null)
                    winch.Activate();
            }
        }
    }
}