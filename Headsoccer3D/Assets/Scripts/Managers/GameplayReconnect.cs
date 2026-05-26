using UnityEngine;
using UnityEngine.InputSystem;


public class GameplayReconnect : MonoBehaviour
{
    [SerializeField] string actionMapName = "Player";

    InputAction anyButtonAction;

    void Awake()
    {
        anyButtonAction = new InputAction(binding: "/*/<button>");
        anyButtonAction.performed += OnAnyButtonPerformed;
        anyButtonAction.Enable();

        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDestroy()
    {
        anyButtonAction.performed -= OnAnyButtonPerformed;
        anyButtonAction.Disable();
        anyButtonAction.Dispose();

        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void OnAnyButtonPerformed(InputAction.CallbackContext ctx)
    {
        InputDevice device = ctx.control.device;

        if (device == null) return;

        if (device is Keyboard || device is Mouse) return;

        if (IsDeviceAlreadyConnected(device)) return;

        TryReconnectDevice(device);
    }

    void TryReconnectDevice(InputDevice device)
    {
        if (PlayerInputHolder.Instance == null) return;

        string incomingId = BuildControllerId(device);

        foreach (var controller in PlayerInputHolder.Instance.playerList)
        {
            if (controller == null) continue;

            if (controller.IsConnected) continue;

            if (controller.ControllerId != incomingId) continue;

            controller.AssignDevice(device, GetInputActions(controller), actionMapName);

            Debug.Log($"Player {controller.PlayerIndex + 1} reconnected with their original controller ({incomingId}).");
            OnPlayerReconnected(controller);
            return;
        }

        Debug.Log($"Device {incomingId} pressed a button but has no disconnected slot to reclaim.");
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change != InputDeviceChange.Disconnected) return;
        if (PlayerInputHolder.Instance == null) return;

        foreach (var controller in PlayerInputHolder.Instance.playerList)
        {
            if (controller == null) continue;
            if (controller.AssignedDevice != device) continue;

            Debug.Log($"Player {controller.PlayerIndex + 1} disconnected during gameplay.");
            controller.PlayerDisconnect();

            OnPlayerDisconnected(controller);
            break;
        }
    }


    protected virtual void OnPlayerDisconnected(PlayerInputController controller)
    {
        
    }

    protected virtual void OnPlayerReconnected(PlayerInputController controller)
    {
        
    }

    bool IsDeviceAlreadyConnected(InputDevice device)
    {
        if (PlayerInputHolder.Instance == null) return false;

        foreach (var controller in PlayerInputHolder.Instance.playerList)
        {
            if (controller != null && controller.IsConnected && controller.AssignedDevice == device)
                return true;
        }

        return false;
    }


    InputActionAsset GetInputActions(PlayerInputController controller)
    {

        foreach (var c in PlayerInputHolder.Instance.playerList)
        {
            if (c != null && c.IsConnected)
                return c.GetSourceInputActions();
        }

        return null;
    }

    static string BuildControllerId(InputDevice device)
    {
        var d = device.description;
        return $"{d.interfaceName}_{d.product}";
    }
}