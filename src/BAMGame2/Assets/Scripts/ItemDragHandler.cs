using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent; // used to snap back to original slot position
    CanvasGroup canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; //save OG parent
        transform.SetParent(transform.root); // above other canvas components
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; //alpha is transparency--semi transparent while dragging
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; //follows mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); //slot where item is dropped
        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null)
        {
            //condition 1: Dragging item to an occupied slot SWAPS the two items
            if (dropSlot.currentItem != null) //slot has an item
            {
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // snaps to middle of slot
            }
            else
            {
                originalSlot.currentItem = null;
            }
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject; 
        }
        else
        {
            //no slot drop point
            transform.SetParent(originalParent);
        }
        
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
