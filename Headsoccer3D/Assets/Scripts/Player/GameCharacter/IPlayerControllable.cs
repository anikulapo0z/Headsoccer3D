using UnityEngine;
public interface IPlayerControllable
{
    void OnSprint(bool held);
    void OnJump();
    void OnKick(bool held);
    void OnStart();
    void OnConfirm();
    void OnCancel();
    void OnAbility();
    void OnMove(Vector2 input);
}
