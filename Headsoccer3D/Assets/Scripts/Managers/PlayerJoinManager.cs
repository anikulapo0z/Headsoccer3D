using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerJoinManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject pressAnyButtonScreen;
    [SerializeField] GameObject characterSelectScreen;
    
    [SerializeField] private MenuAudioManager menuAudioManager;


    [Header("Settings")]
    [SerializeField] float characterSelectOpenDelay = 3f;
    [SerializeField] int maxPlayers = 4;

    [Header("Cursor")]
    [SerializeField] GameObject[] characterCursorPrefab;
    [SerializeField] RectTransform characterCursorParent;
    [SerializeField] RectTransform mainCanvas;

    [Header("Input")]
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] string actionMapName;
    [SerializeField] string joinActionName = "Join";

    public PlayerInputController[] playerSlots;

    bool characterSelectOpen;
    bool isLocked;

    InputAction joinAction;

    void Awake()
    {
        playerSlots = new PlayerInputController[maxPlayers];
        var map = inputActions.FindActionMap(actionMapName);
        map.Enable();
        joinAction = new InputAction(binding: "/*/<button>");
        joinAction.performed += OnJoinPerformed;
        joinAction.Enable();
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnEnable()
    {
        InputSystem.onAnyButtonPress.Call(OnAnyButtonPressed);
    }

    void Start()
    {
        ResetJoinManager();
    }

    void ResetJoinManager()
    {
        characterSelectOpen = false;
        isLocked = false;

        pressAnyButtonScreen.SetActive(true);
        characterSelectScreen.SetActive(false);
    }

    void OnDestroy()
    {
        joinAction.performed -= OnJoinPerformed;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }


    void OnAnyButtonPressed(InputControl control)
    {
        if (isLocked || characterSelectOpen)
            return;

        pressAnyButtonScreen.SetActive(false);
        StartCoroutine(OpenCharacterSelect());
    }

    IEnumerator OpenCharacterSelect()
    {
        isLocked = true;
        characterSelectScreen.SetActive(true);
        yield return new WaitForSeconds(characterSelectOpenDelay);

        characterSelectOpen = true;
        isLocked = false;

        joinAction.Enable();
    }


    void OnJoinPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device is Keyboard || ctx.control.device is Mouse)
            return;

        if (!characterSelectOpen) return;
        if (ctx.control.device == null) return;

        if (MenuManager.Instance.currentScreen == MenuManager.MenuScreen.MapSelect)
        {
            InputDevice d = ctx.control.device;
            if (!IsDeviceAlreadyAssigned(d))
            {
                TryReconnectInMapSelect(d);
            }
            return;
        }




        InputDevice device = ctx.control.device;
        string controllerId = BuildControllerId(device);

        if (IsDeviceAlreadyAssigned(device))
        {
            Debug.Log($"Device {controllerId} is already assigned. Ignoring.");
            return;
        }

        for (int i = 0; i < playerSlots.Length; i++)
        {
            var slot = playerSlots[i];
            if (slot != null && !slot.IsConnected && slot.ControllerId == controllerId)
            {
                slot.AssignDevice(device, inputActions, actionMapName);
                Debug.Log($"Player {i + 1} reconnected with their original controller.");
                return;
            }
        }

        for (int i = 0; i < playerSlots.Length; i++)
        {
            var slot = playerSlots[i];
            if (slot != null && !slot.IsConnected)
            {
                string oldId = slot.ControllerId;
                slot.AssignDevice(device, inputActions, actionMapName);
                Debug.Log($"Player {i + 1} slot taken over by a different controller " +
                          $"(was: {oldId}, now: {controllerId}).");
                return;
            }
        }

        int index = GetNextAvailableSlot();
        if (index == -1)
        {
            Debug.Log("All player slots are full. Cannot join.");
            return;
        }

        PlayerInputController newController = CreatePlayerController(index, device);

        var (cursor, obj) = CreateCursor(index);
        newController.SetControlledObject(cursor, obj, true);

        playerSlots[index] = newController;
        PlayerInputHolder.Instance.playerList.Add(newController);


        PlayerInputHolder.Instance.sourceInputActions = inputActions;
        PlayerInputHolder.Instance.actionMapName = actionMapName;

        DontDestroyOnLoad(newController);

        Debug.Log($"New Player {index + 1} joined with controller {controllerId}.");

        int activeCount = GetActivePlayerCount();
        MenuManager.Instance.PlayerJoined(activeCount);
        MenuManager.Instance.AssignPlayerToPortrait(newController);

        if (activeCount > 2)
            MenuManager.Instance.Force2v2(true);
    }

    void TryReconnectInMapSelect(InputDevice device)
    {
        string incomingId = BuildControllerId(device);

        for (int i = 0; i < playerSlots.Length; i++)
        {
            var slot = playerSlots[i];
            if (slot == null || slot.IsConnected) continue;
            if (slot.ControllerId != incomingId) continue;





            slot.AssignDevice(device, inputActions, actionMapName);


            Debug.Log($"Player {slot.PlayerIndex + 1} reconnected on map select.");
            return;
        }

        Debug.Log("Device pressed a button on map select but has no disconnected slot to reclaim.");
    }



    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change != InputDeviceChange.Disconnected) return;

        for (int i = 0; i < playerSlots.Length; i++)
        {
            var controller = playerSlots[i];
            if (controller == null || controller.AssignedDevice != device) continue;

            Debug.Log($"Player {controller.PlayerIndex + 1} disconnected.");
            controller.PlayerDisconnect();

            if (MenuManager.Instance.currentScreen == MenuManager.MenuScreen.MapSelect)
            {
                Debug.Log($"Player {controller.PlayerIndex + 1} disconnected on map select — slot preserved.");
            }
            else
            {
                PlayerInputHolder.Instance.playerList.Remove(controller);
                playerSlots[i] = null;
                MenuManager.Instance.PlayerLeft(controller);
                Destroy(controller.gameObject);

                if (GetActivePlayerCount() <= 2)
                    MenuManager.Instance.Force2v2(false);
            }

            break;
        }
    }

    int GetActivePlayerCount()
    {
        int count = 0;
        foreach (var p in playerSlots)
            if (p != null) count++;
        return count;
    }

    bool IsDeviceAlreadyAssigned(InputDevice device)
    {
        foreach (var controller in playerSlots)
            if (controller != null && controller.IsConnected && controller.AssignedDevice == device)
                return true;
        return false;
    }

    int GetNextAvailableSlot()
    {
        for (int i = 0; i < playerSlots.Length; i++)
            if (playerSlots[i] == null) return i;
        return -1;
    }

    PlayerInputController CreatePlayerController(int index, InputDevice device)
    {
        GameObject obj = new GameObject($"PlayerInput_{index}");
        var controller = obj.AddComponent<PlayerInputController>();
        controller.Initialize(index, device, inputActions, actionMapName);
        return controller;
    }

    (IPlayerControllable, GameObject) CreateCursor(int index)
    {
        menuAudioManager.PlayCharacterJoinSfx();
        
        Vector3 centerPoint = mainCanvas.TransformPoint(mainCanvas.rect.center);

        GameObject obj = Instantiate(
            characterCursorPrefab[index],
            centerPoint,
            Quaternion.identity,
            characterCursorParent
        );

        var cursor = obj.GetComponent<PlayerCursor>();
        cursor.playerIndex = index;
        return (cursor, obj);
    }

    static string BuildControllerId(InputDevice device)
    {
        var d = device.description;
        return $"{d.interfaceName}_{d.product}";
    }
}