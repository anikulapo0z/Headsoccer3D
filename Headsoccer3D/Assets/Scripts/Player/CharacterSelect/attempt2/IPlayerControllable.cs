using UnityEngine;
public interface IPlayerControllable
{
    void OnJump();
    void OnKick();
    void OnJoin();
    void OnConfirm();
    void OnCancel();
    void OnAbility();
    void OnMove(Vector2 input);

}
