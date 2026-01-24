using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerControllable
{
    public void OnAbility()
    {
        throw new System.NotImplementedException();
    }

    public void OnCancel()
    {
        throw new System.NotImplementedException();
    }

    public void OnConfirm()
    {
        throw new System.NotImplementedException();
    }

    public void OnJoin()
    {
        throw new System.NotImplementedException();
    }

    public void OnJump()
    {
        throw new System.NotImplementedException();
    }

    public void OnKick()
    {
        throw new System.NotImplementedException();
    }

    public void OnMove(Vector2 input)
    {
        Debug.Log("Moving: " + input);
        throw new System.NotImplementedException();
    }
}
