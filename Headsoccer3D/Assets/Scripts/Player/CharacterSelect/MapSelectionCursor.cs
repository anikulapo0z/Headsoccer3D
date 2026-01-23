using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class MapSelectionCursor : MonoBehaviour, IPlayerControllable
{
    bool locked = false;
    public RectTransform parent;
    public int playerIndex;



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
        }
    }

    public void OnCancel()
    {
        locked = false;
    }

    public void OnConfirm()
    {
        locked = true;
    }

}
