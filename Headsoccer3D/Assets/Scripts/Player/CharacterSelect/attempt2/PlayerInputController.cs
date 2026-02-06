using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public int PlayerIndex { get; private set; }
    public InputDevice AssignedDevice { get; private set; }

    public string ControllerId { get; private set; }
    public bool IsConnected { get; private set; }

    public int selectedCharacterID = -1;

    IPlayerControllable controlledObject;

    InputActionAsset actionsInstance;
    InputAction moveAction;
    InputAction confirmAction;
    InputAction cancelAction;
    InputAction joinAction;
    InputAction jumpAction;
    InputAction kickAction;
    InputAction abilityAction;
    InputAction sprintAction;

    public void Initialize(
        int playerIndex,
        InputDevice device,
        InputActionAsset sourceActions,
        string actionMapName
    )
    {
        PlayerIndex = playerIndex;
        AssignDevice(device, sourceActions, actionMapName);
    }

    public void AssignDevice(
        InputDevice device,
        InputActionAsset sourceActions,
        string actionMapName
    )
    {
        AssignedDevice = device;
        IsConnected = true;

        ControllerId = BuildControllerId(device);

        if (actionsInstance != null)
        {
            actionsInstance.Disable();
            Destroy(actionsInstance);
        }

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

        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveCancelled;

        confirmAction.performed += OnConfirm;
        cancelAction.performed += OnCancel;
        joinAction.performed += OnJoin;
        jumpAction.performed += OnJump;
        kickAction.performed += OnKick;
        abilityAction.performed += OnAbility;
        sprintAction.performed += OnSprint;

        map.Enable();
    }

<<<<<<< HEAD
    public void MarkDisconnected()
    {
        IsConnected = false;
        AssignedDevice = null;

        actionsInstance?.Disable();
=======
        moveAction.canceled += OnMoveCancelled;
        sprintAction.canceled += OnSprint;
        kickAction.canceled += OnKick;
>>>>>>> main
    }

    public void SetControlledObject(IPlayerControllable obj)
    {
        controlledObject = obj;
    }

    static string BuildControllerId(InputDevice device)
    {
        var d = device.description;
        return $"{d.interfaceName}_{d.product}_{device.deviceId}";
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        controlledObject?.OnMove(ctx.ReadValue<Vector2>());
    }

    void OnMoveCancelled(InputAction.CallbackContext ctx)
    {
        controlledObject?.OnMove(Vector2.zero);
    }

<<<<<<< HEAD
    void OnConfirm(InputAction.CallbackContext ctx) => controlledObject?.OnConfirm();
    void OnCancel(InputAction.CallbackContext ctx) => controlledObject?.OnCancel();
    void OnJump(InputAction.CallbackContext ctx) => controlledObject?.OnJump();
    void OnKick(InputAction.CallbackContext ctx) => controlledObject?.OnKick();
    void OnJoin(InputAction.CallbackContext ctx) => controlledObject?.OnJoin();
    void OnAbility(InputAction.CallbackContext ctx) => controlledObject?.OnAbility();
=======
    void OnConfirm(InputAction.CallbackContext ctx)
    {
        controlledObject?.OnConfirm();
    }

    void OnCancel(InputAction.CallbackContext ctx)
    {
        controlledObject?.OnCancel();
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        controlledObject?.OnJump();
    }

    void OnKick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) controlledObject?.OnKick(true);
        else if (ctx.canceled) controlledObject?.OnKick(false);
    }

    void OnJoin(InputAction.CallbackContext ctx)
    {
        controlledObject?.OnJoin();
    }
    void OnAbility(InputAction.CallbackContext ctx)
    {
        controlledObject?.OnAbility();
    }
    void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) controlledObject?.OnSprint(true);
        else if (ctx.canceled) controlledObject?.OnSprint(false);
    }

>>>>>>> main

    void OnDestroy()
    {
        actionsInstance?.Disable();
        Destroy(actionsInstance);
    }
}
