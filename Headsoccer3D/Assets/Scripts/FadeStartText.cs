using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeStartText : MonoBehaviour
{
    [SerializeField] float flashDuration;
    [SerializeField] float updateRate;
    [SerializeField] TextMeshProUGUI textToFlash;
    [SerializeField] bool flashing = true;
    [SerializeField] bool fadeOut = true;
    [SerializeField] float flashSpeed;
    float alpha;
    Color startColor;
    float currentTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startColor = textToFlash.color;
        StartCoroutine(FlashText());
    }

    IEnumerator FlashText()
    {
        currentTime = 0;
        while (flashing)
        {
            if (fadeOut)
            {
                alpha = Mathf.Lerp(1f, 0f, currentTime/flashDuration);
                textToFlash.color = new Color(startColor.r, startColor.g, startColor.b, alpha * flashSpeed);
                currentTime += Time.deltaTime;
                if(alpha <= 0f)
                {
                    fadeOut = false;
                    alpha = .1f;
                    currentTime = 0;
                }
            }
            else
            {
                alpha = Mathf.Lerp(0f, 1f, currentTime / flashDuration);
                textToFlash.color = new Color(startColor.r, startColor.g, startColor.b, alpha * flashSpeed);
                currentTime += Time.deltaTime;
                if (alpha >= 1f)
                {
                    fadeOut = true;
                    alpha = .9f;
                    currentTime = 0;
                }
            }
            Debug.Log(alpha);
            yield return new WaitForSeconds(updateRate);
           

        }

        //yield return null;
    }
}
