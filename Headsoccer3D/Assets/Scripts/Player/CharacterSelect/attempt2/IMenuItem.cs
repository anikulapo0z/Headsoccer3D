using UnityEngine;

public interface IMenuItem
{
    void OnHoverEnter(int playerIndex);
    void OnHoverExit(int playerIndex);
    void OnConfirm(int playerIndex);
}
