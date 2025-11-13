using Game.Runtime;
using Game399.Shared.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,  IPointerClickHandler
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    
    Transform originalParent; // used to snap back to original slot position
    CanvasGroup canvasGroup;
    Canvas targetCanvas;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        targetCanvas = GetComponentInParent<Canvas>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; //save OG parent
        // Reparent to the Canvas
        transform.SetParent(targetCanvas.transform, worldPositionStays: true); // above other canvas components
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
            
            if (dropSlot.currentItem != null) //slot has an item
            {
                Item draggedItem = GetComponent<Item>();
                Item targetItem = dropSlot.currentItem.GetComponent<Item>();
                if (draggedItem.ID == targetItem.ID)
                {
                    targetItem.AddToStack(draggedItem.quantity);
                    originalSlot.currentItem = null;
                    Destroy(gameObject);
                }
                else
                {
                    //SWAP the two items
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    originalSlot.currentItem = dropSlot.currentItem;
                    
                    dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // snaps to middle of slot
                    
                    //move item into drop slot
                    transform.SetParent(dropSlot.transform);
                    dropSlot.currentItem = gameObject; 
                    
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }
            }
            else
            {
                originalSlot.currentItem = null;
                
                //move item into drop slot
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject; 
            }
        }
        else
        {
            //no slot drop point
            transform.SetParent(originalParent);
            
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Log.Info($"right click is being called {eventData.pointerDrag.name}");
            //Split the Stack
            SplitStack();
        }
    }

    private void SplitStack()
    {
        
        Item item = GetComponent<Item>();
        if (item == null || item.quantity <= 1)
        {
            return;
        }
        
        int splitAmount = item.quantity / 2;
        if (splitAmount <= 0)
        {
            return;
        }
        item.RemoveFromStack(splitAmount);

        GameObject newItem = item.CloneItem(splitAmount);
        
        if (InventoryManager.Instance == null || newItem == null)
        {
            return;
        }

        foreach (Transform slotTransform in InventoryManager.Instance.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                slot.currentItem =  newItem;
                newItem.transform.SetParent(slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }
        }
        
        //no empty slot - return to stack
        item.AddToStack(splitAmount);
        Destroy(newItem);
    }
}
