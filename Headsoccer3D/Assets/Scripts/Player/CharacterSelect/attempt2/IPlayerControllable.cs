using UnityEngine;
public interface IPlayerControllable
{
    void OnMove(Vector2 input);
    void OnConfirm();
    void OnCancel();
}
