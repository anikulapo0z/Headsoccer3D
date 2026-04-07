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

    public void SetBackground(Sprite newSprite)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(fadeOverlay.DOFade(1f, fadeDuration));
        seq.AppendCallback(() =>
        {
            backgroundImage.sprite = newSprite;
            StartPan();
        });
        seq.Append(fadeOverlay.DOFade(0f, fadeDuration));
    }

    void StartPan()
    {
        backgroundImage.color = Color.white;
        panTween?.Kill();

        Vector2 backgroundSize = new Vector2(
            backgroundRect.rect.width * backgroundRect.localScale.x,
            backgroundRect.rect.height * backgroundRect.localScale.y
        );

        float x = backgroundSize.x - canvasRect.rect.width;
        float y = backgroundSize.y - canvasRect.rect.height;

        Vector2 end = new Vector2(0, 0);
        Vector2 start = new Vector2(x, -y);

        backgroundRect.anchoredPosition = start;
        panTween = backgroundRect.DOAnchorPos(end, panDuration).SetEase(Ease.InOutSine);
    }
}