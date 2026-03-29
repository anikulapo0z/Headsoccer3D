using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MapSelectBackground : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] RectTransform backgroundRect;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] Image fadeOverlay;
    [SerializeField] float fadeDuration;
    [SerializeField] float panDuration;

    Tween panTween;

    public void SetBackground(Sprite newImage)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(fadeOverlay.DOFade(1f, fadeDuration));

        seq.AppendCallback(() =>
        {
            backgroundImage.sprite = newImage;
            ResetPan();
        });

        seq.Append(fadeOverlay.DOFade(0f, fadeDuration));
    }

    void ResetPan()
    {
        backgroundImage.color = Color.white;

        panTween?.Kill();

        Vector2 startPos = new Vector2(0, 0);
        Vector2 endPos = GetEndPos();

        backgroundRect.anchoredPosition = startPos;

        panTween = backgroundRect.DOAnchorPos(endPos, panDuration);
    }

    Vector2 GetEndPos()
    {
        float xOffset = canvasRect.rect.width - backgroundRect.rect.width;
        float yOffset = canvasRect.rect.height - backgroundRect.rect.height;

        return new Vector2(xOffset / 2, -yOffset / 2);
    }
}