using UnityEngine;
using UnityEngine.UI;

public class DisconnectPlayerButton : MonoBehaviour, IMenuItem
{
    [SerializeField] Image defaultImage;
    [SerializeField] Image hoverImage;
    [SerializeField] bool changeSprite;
    [SerializeField] Color hoveredColor;
    [SerializeField] CharacterPortrait characterPortrait;

    int assignedPlayer;

    public void SetPlayerVar(int ap)
    {
        assignedPlayer = ap;
    }

    public void OnConfirm(int playerIndex)
    {
        if (assignedPlayer == playerIndex)
        {
            characterPortrait.OnDisconnectButton();
            OnHoverExit(-1);
        }
    }

    public void OnHoverEnter(int playerIndex)
    {
        if (assignedPlayer == playerIndex)
        {
            if (changeSprite)
            {
                GetComponent<Image>().sprite = hoverImage.sprite;
            }
            else
            {
                GetComponent<Image>().color = hoveredColor;
            }
        }
    }

    public void OnHoverExit(int playerIndex)
    {
        if (assignedPlayer == playerIndex)
        {
            if (changeSprite)
            {
                GetComponent<Image>().sprite = defaultImage.sprite;
            }
            else
            {
                GetComponent<Image>().color = Color.white;
            }
        }
    }
}
