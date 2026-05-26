using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInputController : MonoBehaviour
{
    public int PlayerIndex { get; private set; }
    public InputDevice AssignedDevice { get; private set; }

    public string ControllerId;
    public bool IsConnected { get; private set; }

    public int selectedCharacterID = -2;
    public int portraitIndex = -1;

    public List<IPlayerControllable> controlledObject = new List<IPlayerControllable>();
    public List<GameObject> controlledGameObject = new List<GameObject>();

    InputActionAsset actionsInstance;
    InputAction moveAction;
    InputAction confirmAction;
    InputAction cancelAction;
    InputAction joinAction;
    InputAction jumpAction;
    InputAction kickAction;
    InputAction abilityAction;
    InputAction sprintAction;

    InputActionAsset sourceActionsReference;


    public InputActionAsset GetSourceInputActions() => sourceActionsReference;

    public void Initialize(
        int playerIndex,
        InputDevice device,
        InputActionAsset sourceActions,
        string actionMapName)
    {
        PlayerIndex = playerIndex;
        AssignDevice(device, sourceActions, actionMapName);
    }


    public void AssignDevice(InputDevice device, InputActionAsset sourceActions, string actionMapName)
    {
        sourceActionsReference = sourceActions;

        if (actionsInstance != null)
        {
            UnsubscribeActions();
            actionsInstance.Disable();
            Destroy(actionsInstance);
            actionsInstance = null;
        }

        AssignedDevice = device;
        IsConnected = true;
        ControllerId = BuildControllerId(device);

        actionsInstance = Instantiate(sourceActions);
        var map = actionsInstance.FindActionMap(actionMapName);
        map.devices = new[] { device };

        moveAction = map.FindAction("Move");
        confirmAction = map.FindAction("Confirm");
        cancelAction = map.FindAction("Cancel");
        joinAction = map.FindAction("Join");
        jumpAction = map.FindAction("Jump");
        kickAction = map.FindAction("Kick");
        abilityAction = map.FindAction("Ability");
        sprintAction = map.FindAction("Sprint");

        SubscribeActions();

        map.Enable();
    }
    

    public void PlayerDisconnect()
    {
        IsConnected = false;
        AssignedDevice = null;

        if (actionsInstance != null)
        {
            UnsubscribeActions();
            actionsInstance.Disable();
        }
    }


    public void SetControlledObject(IPlayerControllable obj, GameObject go, bool resetList)
    {
        if (resetList)
        {
            controlledObject.Clear();

            foreach (var p in controlledGameObject)
                Destroy(p);

            controlledGameObject.Clear();
        }

        controlledObject.Add(obj);
        controlledGameObject.Add(go);
    }

    public void RemoveControlledObject(IPlayerControllable obj, bool destroyGameObject)
    {
        int index = controlledObject.IndexOf(obj);
        if (index == -1) return;

        if (index < controlledGameObject.Count)
        {
            if (destroyGameObject)
                Destroy(controlledGameObject[index]);
            controlledGameObject.RemoveAt(index);
        }

        controlledObject.RemoveAt(index);
    }


    void SubscribeActions()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveCancelled;
        confirmAction.performed += OnConfirm;
        cancelAction.performed += OnCancel;
        joinAction.performed += OnJoin;
        jumpAction.performed += OnJump;
        kickAction.performed += OnKick;
        abilityAction.performed += OnAbility;
        sprintAction.performed += OnSprint;
    }

    void UnsubscribeActions()
    {
        if (moveAction != null) { moveAction.performed -= OnMove; moveAction.canceled -= OnMoveCancelled; }
        if (confirmAction != null) confirmAction.performed -= OnConfirm;
        if (cancelAction != null) cancelAction.performed -= OnCancel;
        if (joinAction != null) joinAction.performed -= OnJoin;
        if (jumpAction != null) jumpAction.performed -= OnJump;
        if (kickAction != null) kickAction.performed -= OnKick;
        if (abilityAction != null) abilityAction.performed -= OnAbility;
        if (sprintAction != null) sprintAction.performed -= OnSprint;
    }


    void OnMove(InputAction.CallbackContext ctx)
    {
        foreach (var p in controlledObject)
            p?.OnMove(ctx.ReadValue<Vector2>());
    }

    void OnMoveCancelled(InputAction.CallbackContext ctx)
    {
        foreach (var p in controlledObject)
            p?.OnMove(Vector2.zero);
    }

    void OnSprint(InputAction.CallbackContext ctx)
    {
        foreach (var p in controlledObject)
            p?.OnSprint(ctx.ReadValueAsButton());
    }

    void OnConfirm(InputAction.CallbackContext ctx)
    {
        foreach (var p in controlledObject)
            p?.OnConfirm();
    }

    void OnCancel(InputAction.CallbackContext ctx)
    {
        foreach (var p in controlledObject)
            p?.OnCancel();
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        foreach (var p in controlledObject)
            p?.OnJump();
    }

    void OnKick(InputAction.CallbackContext ctx)
    {
        foreach (var p in controlledObject)
            p?.OnKick(ctx.ReadValueAsButton());
    }

    void OnJoin(InputAction.CallbackContext ctx)
    {
        foreach (var p in controlledObject)
            p?.OnJoin();
    }

    void OnAbility(InputAction.CallbackContext ctx)
    {
        foreach (var p in controlledObject)
            p?.OnAbility();
    }


    void OnDestroy()
    {
        UnsubscribeActions();
        if (actionsInstance != null)
        {
            actionsInstance.Disable();
            Destroy(actionsInstance);
        }
    }


    static string BuildControllerId(InputDevice device)
    {
        var d = device.description;
        return $"{d.interfaceName}_{d.product}";
    }
}