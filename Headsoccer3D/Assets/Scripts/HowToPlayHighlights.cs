using UnityEngine;
using DG.Tweening;
using System.Collections;

public class HowToPlayHighlights : MonoBehaviour
{
    [SerializeField] float timeToScale;
    [SerializeField] float timeToTurnOff;

    public void SetHighlight(bool val, bool autoTurnOff)
    {
        if (val)
        {
            transform.DOScaleX(1, timeToScale);
        }
        else
            transform.DOScaleX(0, timeToScale);

        if (autoTurnOff)
            Invoke("AutoTurnOff", timeToTurnOff);
    }

    void AutoTurnOff()
    {
        transform.DOScaleX(0, timeToScale);
    }
}
