using UnityEngine;

public class SelectionCursor : MonoBehaviour, IPlayerControllable
{
    public RectTransform parent;
    bool locked = false;
    public int playerIndex;
    public PlayerInputController playerInputController;

    public void OnMove(Vector2 dir)
    {
        if (locked)
            return;
        CharacterButton currentButton = parent.GetComponent<CharacterButton>();
        if (currentButton == null)
            return;

        RectTransform targetButton = null;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x < 0)
            {
                targetButton = currentButton.selectionLeft;
            }
            else if (dir.x > 0)
            {
                targetButton = currentButton.selectionRight;
            }
        }
        else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            if (dir.y < 0)
            {
                targetButton = currentButton.selectionBelow;
            }
            else if (dir.y > 0)
            {
                targetButton = currentButton.selectionAbove;
            }
        }

        if (targetButton != null)
        {
            transform.SetParent(targetButton, false);
            transform.localPosition = Vector3.zero;
            parent = targetButton;

            CharacterSelectionPreview.Instance.SetPortraitInfo(
                playerIndex,
                parent.GetComponent<CharacterButton>().selectedImage,
                parent.GetComponent<CharacterButton>().characterName
                );
        }
    }

    public void OnConfirm()
    {
        CharacterSelectManager.Instance.CheckPlayerConfirm(locked);
        locked = true;
        playerInputController.selectedCharacterID = parent.GetComponent<CharacterButton>().characterID;
        CharacterSelectionPreview.Instance.SetPortraitInfo(
            playerIndex,
            parent.GetComponent<CharacterButton>().selectedImage,
            parent.GetComponent<CharacterButton>().characterName
            );
    }

    public void OnCancel()
    {
        CharacterSelectManager.Instance.PlayerCancel(locked);
        locked = false;
        Debug.Log($"{name} Cancel");
    }

    public void OnJump()
    {
        Debug.Log("OnJumpPressed");
    }

    public void OnKick()
    {
        Debug.Log("OnKickPressed");
    }

    public void OnJoin()
    {
        Debug.Log("OnJoinPressed");
    }

    public void OnAbility()
    {
        Debug.Log("OnAbilityPressed");
    }
}
