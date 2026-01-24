using UnityEngine;

public class MapSelectionCursor : MonoBehaviour, IPlayerControllable
{
    bool locked = false;
    public RectTransform parent;
    public int playerIndex;
    string sceneName;



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
            sceneName = targetButton.GetComponent<CharacterButton>().sceneName;
            if(playerIndex == 0)
                MapSelectionManager.Instance.selectedScene = sceneName;
        }
    }

    public void OnCancel()
    {
        MapSelectionManager.Instance.PlayerCancel(locked);

        locked = false;
    }

    public void OnConfirm()
    {
        MapSelectionManager.Instance.CheckPlayerConfirm(locked);

        locked = true;
    }

    public void OnJump()
    {
        throw new System.NotImplementedException();
    }

    public void OnKick()
    {
        throw new System.NotImplementedException();
    }

    public void OnJoin()
    {
        throw new System.NotImplementedException();
    }

    public void OnAbility()
    {
        throw new System.NotImplementedException();
    }
}
