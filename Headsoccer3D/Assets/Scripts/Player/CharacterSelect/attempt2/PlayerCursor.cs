using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCursor : MonoBehaviour, IPlayerControllable
{
    [SerializeField] private float moveSpeed = 800f;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnMove(Vector2 input)
    {
        rect.anchoredPosition += input * moveSpeed * Time.deltaTime;
    }

    public void OnConfirm()
    {
        Debug.Log($"{name} Confirm");
    }

    public void OnCancel()
    {
        Debug.Log($"{name} Cancel");
    }

    void IPlayerControllable.OnJump()
    {
        throw new System.NotImplementedException();
    }

    void IPlayerControllable.OnKick()
    {
        throw new System.NotImplementedException();
    }

    void IPlayerControllable.OnJoin()
    {
        throw new System.NotImplementedException();
    }

    void IPlayerControllable.OnConfirm()
    {
        throw new System.NotImplementedException();
    }

    void IPlayerControllable.OnCancel()
    {
        throw new System.NotImplementedException();
    }

    void IPlayerControllable.OnMove(Vector2 input)
    {
        throw new System.NotImplementedException();
    }
}
