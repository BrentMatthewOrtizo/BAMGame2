using System;
using System.Collections;
using System.Collections.Generic;
using Game.Runtime;
using Game399.Shared.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUpUIController : MonoBehaviour
{
    private static IGameLog Log => ServiceResolver.Resolve<IGameLog>();
    public static ItemPickUpUIController Instance {get; private set;}
    
    public GameObject popUpPrefab;

    public int maxPopUps = 5;
    public float popUpDuration = 3f;

    private readonly Queue<GameObject> activePopUps = new();
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            Log.Warn("Instance already exists, destroying object");
        }
    }

    public void ShowItemPickUp(String itemName, Sprite itemIcon)
    {
        GameObject newPopUp = Instantiate(popUpPrefab, transform);
        newPopUp.GetComponentInChildren<TMP_Text>().text = itemName;
        
        Image itemImage = newPopUp.transform.Find("ItemIcon")?.GetComponent<Image>();
        if (itemImage)
        {
            itemImage.sprite = itemIcon;
        }
        activePopUps.Enqueue(newPopUp);
        if (activePopUps.Count > maxPopUps)
        {
            Destroy(activePopUps.Dequeue());
        }
        
        //fade out and destroy
        StartCoroutine(FadeOutAndDestroy(newPopUp));
    }

    private IEnumerator FadeOutAndDestroy(GameObject popUp)
    {
        yield return new WaitForSeconds(popUpDuration);
        if (popUp == null)
        {
            yield break;
        } 
        CanvasGroup canvasGroup = popUp.GetComponent<CanvasGroup>();
        for (float timePassed = 0f; timePassed < 1f; timePassed += Time.deltaTime)
        {
            if (popUp == null)
            {
                yield break;
            }
            canvasGroup.alpha = 1f - timePassed;
            yield return null;
        }
        Destroy(popUp);
    }
}
