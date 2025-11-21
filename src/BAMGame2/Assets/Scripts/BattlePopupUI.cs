using UnityEngine;
using TMPro;
using System.Collections;

public class BattlePopupUI : MonoBehaviour
{
    public TMP_Text popupText;
    public CanvasGroup canvasGroup;

    public float fadeSpeed = 1.2f;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        gameObject.SetActive(true);
    }

    // ---------------------------------------------------------
    // SHOW
    // ---------------------------------------------------------
    public void Show(string message)
    {
        popupText.text = message;
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float a = canvasGroup.alpha;

        while (a < 1f)
        {
            a += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = a;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    // ---------------------------------------------------------
    // HIDE
    // ---------------------------------------------------------
    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float a = canvasGroup.alpha;

        while (a > 0f)
        {
            a -= Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = a;
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}