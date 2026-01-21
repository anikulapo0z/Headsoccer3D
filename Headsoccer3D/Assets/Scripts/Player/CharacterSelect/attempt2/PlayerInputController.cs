using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public int PlayerIndex { get; private set; }
    public InputDevice AssignedDevice { get; private set; }

    IPlayerControllable controlledObject;

    InputActionAsset actionsInstance;
    InputAction moveAction;
    InputAction confirmAction;
    InputAction cancelAction;

    public void Initialize(
        int playerIndex,
        InputDevice device,
        InputActionAsset sourceActions,
        string actionMapName
    ) // lots of input fields :(
    {
        PlayerIndex = playerIndex;
        AssignedDevice = device;

        // clone it so inputs dont get mixed ig
        actionsInstance = Instantiate(sourceActions);

        var map = actionsInstance.FindActionMap(actionMapName);

        // set map to device
        map.devices = new[] { device };

        moveAction = map.FindAction("Move");
        confirmAction = map.FindAction("Confirm");
        cancelAction = map.FindAction("Cancel");

        moveAction.performed += OnMove;
        confirmAction.performed += OnConfirm;
        cancelAction.performed += OnCancel;

        map.Enable();
    }

    public void SetControlledObject(IPlayerControllable obj)
    {
        controlledObject = obj;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        controlledObject?.OnMove(ctx.ReadValue<Vector2>());
    }

    void OnConfirm(InputAction.CallbackContext ctx)
    {
        controlledObject?.OnConfirm();
    }

    void OnCancel(InputAction.CallbackContext ctx)
    {
        controlledObject?.OnCancel();
    }

    void OnDestroy()
    {
        actionsInstance?.Disable();
        Destroy(actionsInstance);
    }
}
