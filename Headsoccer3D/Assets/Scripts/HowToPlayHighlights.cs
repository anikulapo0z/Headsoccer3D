using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class HowToPlayHighlights : MonoBehaviour
{
    [SerializeField] float timeToTween;
    [SerializeField] float timeToTurnOff;

    private Image image;
    [SerializeField] Color targetColor = Color.yellow;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void SetHighlight(bool val, bool autoTurnOff)
    {
        image.DOColor(targetColor, timeToTween);
        /*
        if (val)
        {
            transform.DOScaleX(1, timeToTween);
        }
        else
            transform.DOScaleX(0, timeToTween);*/

        if (autoTurnOff)
            Invoke("AutoTurnOff", timeToTurnOff);
    }

    public void moveJoystick(Vector2 _dir)
    {

    }
    void AutoTurnOff()
    {
        image.DOColor(Color.white, timeToTween);
        //transform.DOScaleX(0, timeToTween);
    }
}
