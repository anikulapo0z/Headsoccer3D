using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInputController : MonoBehaviour
{
    public int PlayerIndex { get; private set; }
    public InputDevice AssignedDevice { get; private set; }

    public string ControllerId { get; private set; }
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

        //moveAction.canceled += OnMoveCancelled;
        //sprintAction.canceled += OnSprint;
        //kickAction.canceled += OnKick;


    public void PlayerDisconnect()
    {
        IsConnected = false;
        AssignedDevice = null;

        ControllerId = null;

        actionsInstance?.Disable();
    }


    public void SetControlledObject(IPlayerControllable obj, GameObject ob, bool resetControlledObjectList)
    {
        if (resetControlledObjectList)
        {
            controlledObject.Clear();

            foreach(var p in controlledGameObject)
            {
                Destroy(p);
            }

            controlledGameObject.Clear();

        }

        controlledObject.Add(obj);
        controlledGameObject.Add(ob);
    }

    static string BuildControllerId(InputDevice device)
    {
        var d = device.description;
        return $"{d.interfaceName}_{d.product}_{device.deviceId}";
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        foreach(IPlayerControllable p in controlledObject)
            p?.OnMove(ctx.ReadValue<Vector2>());
    }

    void OnMoveCancelled(InputAction.CallbackContext ctx)
    {
        foreach (IPlayerControllable p in controlledObject)
            p?.OnMove(Vector2.zero);
    }

    void OnSprint(InputAction.CallbackContext ctx)
    {
        foreach (IPlayerControllable p in controlledObject)
            p?.OnSprint(ctx.ReadValueAsButton());
    }

    void OnConfirm(InputAction.CallbackContext ctx)
    {
        foreach (IPlayerControllable p in controlledObject)
            p?.OnConfirm();
    }
    void OnCancel(InputAction.CallbackContext ctx)
    {
        foreach (IPlayerControllable p in controlledObject)
            p?.OnCancel();
    }
    void OnJump(InputAction.CallbackContext ctx)
    {
        foreach (IPlayerControllable p in controlledObject)
            p?.OnJump();
    }
    void OnKick(InputAction.CallbackContext ctx)
    {
        foreach (IPlayerControllable p in controlledObject)
            p?.OnKick(ctx.ReadValueAsButton());
    }
    void OnJoin(InputAction.CallbackContext ctx)
    {
        foreach (IPlayerControllable p in controlledObject)
            p?.OnJoin();
    }
    void OnAbility(InputAction.CallbackContext ctx)
    {
        foreach (IPlayerControllable p in controlledObject)
            p?.OnAbility();
    }

    void OnDestroy()
    {
        actionsInstance?.Disable();
        Destroy(actionsInstance);
    }
}
