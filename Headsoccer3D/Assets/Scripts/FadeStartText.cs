using System.Collections;
using TMPro;
using UnityEngine;

public class FadeStartText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textToFlash;
    [SerializeField] bool flashing = true;
    [SerializeField] float flashSpeed;
    Color startColor;


    void Start()
    {
        startColor = textToFlash.color;
    }

    void FixedUpdate()
    {
        if (!flashing)
            return;

        float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f);
        textToFlash.color = new Color(
            startColor.r,
            startColor.g,
            startColor.b,
            alpha
        );
    }

}
