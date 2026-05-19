using UnityEngine;
using UnityEngine.UI;

public class ModeSelectButton : MonoBehaviour, IMenuItem
{
    [SerializeField] Image imageToChange;
    [SerializeField] Sprite defaultImage;
    [SerializeField] Sprite hoveringImage;


    void StartMenuTransition()
    {
        gameObject.SetActive(false);
    }


    public void OnConfirm(int playerIndex)
    {
        StartMenuTransition();
    }

    public void OnHoverEnter(int playerIndex)
    {
        imageToChange.sprite = hoveringImage;
    }

    public void OnHoverExit(int playerIndex)
    {
        imageToChange.sprite = defaultImage;
    }
}
