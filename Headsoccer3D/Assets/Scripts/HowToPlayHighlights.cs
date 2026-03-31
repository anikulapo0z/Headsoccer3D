using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class HowToPlayHighlights : MonoBehaviour
{
    [SerializeField] float timeToTween;
    [SerializeField] float timeToTurnOff;

    private Image image;
    private RectTransform rectTransform;
    private Vector2 initialRectTransformAnchor;
    [SerializeField] Color targetColor = Color.yellow;
    [SerializeField] float joystickDist = 5;

    private void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        initialRectTransformAnchor = rectTransform.anchoredPosition;
    }


    public void SetHighlight(bool val, bool autoTurnOff, float _dirX = 0, float _dirY = 0)
    {
        Vector2 _dir = new Vector2(_dirX, _dirY);
        image.DOColor(targetColor, timeToTween);

        if (val)
        {
            image.DOColor(targetColor, timeToTween);
            rectTransform.anchoredPosition = initialRectTransformAnchor + (_dir * joystickDist);

            //transform.DOScaleX(1, timeToTween);
        }
        else
        {
            image.DOColor(Color.white, timeToTween);
            rectTransform.anchoredPosition = initialRectTransformAnchor;

            //transform.DOScaleX(0, timeToTween);
        }

        if (autoTurnOff)
            Invoke("AutoTurnOff", timeToTurnOff);
    }

    void AutoTurnOff()
    {
        image.DOColor(Color.white, timeToTween);
        rectTransform.anchoredPosition = initialRectTransformAnchor;
        //transform.DOScaleX(0, timeToTween);
    }
}
