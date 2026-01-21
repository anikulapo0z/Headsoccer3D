using UnityEngine;

public class SelectionCursor : MonoBehaviour, IPlayerControllable
{
    public RectTransform parent;

    public void OnMove(Vector2 dir)
    {
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

    public void OnConfirm()
    {
        Debug.Log($"{name} Confirm");
    }

    public void OnCancel()
    {
        Debug.Log($"{name} Cancel");
    }

}
