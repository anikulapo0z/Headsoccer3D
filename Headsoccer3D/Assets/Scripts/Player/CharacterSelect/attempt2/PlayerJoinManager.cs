using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerJoinManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject pressAnyButtonScreen;
    [SerializeField] GameObject characterSelectScreen;

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

    //public List<PlayerInputController> inputControllers = new();
    public PlayerInputController[] playerSlots;

    bool characterSelectOpen;
    bool isLocked;

    InputAction joinAction;

    void Awake()
    {
        playerSlots = new PlayerInputController[maxPlayers];

        var map = inputActions.FindActionMap(actionMapName);
        map.Enable();

        joinAction = map.FindAction(joinActionName);
        joinAction.performed += OnJoinPerformed;

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
        //inputControllers.Clear();

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
        if (ctx.control.device == null)
            return;


        if (!characterSelectOpen)
            return;

        InputDevice device = ctx.control.device;
        if (IsDeviceAlreadyAssigned(device))
            return;

        string controllerId = BuildControllerId(device);

/*        foreach (var controller in inputControllers)
        {
            if (!controller.IsConnected &&
                controller.ControllerId == controllerId)
            {
                controller.AssignDevice(device, inputActions, actionMapName);
                Debug.Log($"Reconnected Player {controller.PlayerIndex + 1}");
                return;
            }
        }*/

/*        foreach (var controller in inputControllers)
        {
            if (!controller.IsConnected)
            {
                controller.AssignDevice(device, inputActions, actionMapName);
                Debug.Log($"Reassigned controller to Player {controller.PlayerIndex + 1}");
                return;
            }
        }*/

/*        if (inputControllers.Count >= maxPlayers)
            return;*/



        //int index = inputControllers.Count;

        int index = GetNextAvailableSlot();

        if (index == -1)
            return;


        PlayerInputController newController = CreatePlayerController(index, device);

        var (cursor, obj) = CreateCursor(index);

        //IPlayerControllable cursor = CreateCursor(index);
        newController.SetControlledObject(cursor, obj, true);

        playerSlots[index] = newController;
        //inputControllers.Add(newController);
        PlayerInputHolder.Instance.playerList.Add(newController);
        DontDestroyOnLoad(newController);

        Debug.Log($"New Player {index + 1} joined");

        //MenuManager.Instance.PlayerJoined(inputControllers.Count);
        MenuManager.Instance.PlayerJoined(GetActivePlayerCount());


        MenuManager.Instance.AssignPlayerToPortrait(newController);


        if (GetActivePlayerCount() > 2)
        {
            MenuManager.Instance.Force2v2(true);
        }

    }

    int GetActivePlayerCount()
    {
        int count = 0;

        foreach (var p in playerSlots)
        {
            if (p != null)
                count++;
        }

        return count;
    }

    bool IsDeviceAlreadyAssigned(InputDevice device)
    {
        foreach (var controller in playerSlots)
        {
            if (controller != null && controller.IsConnected && controller.AssignedDevice == device)
                return true;
        }
        return false;
    }


    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change != InputDeviceChange.Disconnected)
            return;

/*        foreach (var controller in inputControllers)
        {
            if (controller.AssignedDevice == device)
            {
                controller.FullDisconnect();
                Debug.Log($"Player {controller.PlayerIndex + 1} disconnected");
            }
        }*/

        foreach (var controller in playerSlots)
        {
            if (controller != null && controller.AssignedDevice == device)
            {
                controller.PlayerDisconnect();
                Debug.Log($"Player {controller.PlayerIndex + 1} disconnected");
            }
        }
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
        GameObject obj = Instantiate(
            characterCursorPrefab[index],
            Vector3.zero,
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
        return $"{d.interfaceName}_{d.product}_{device.deviceId}";
    }

/*    public void RemoveController(PlayerInputController controller)
    {
        if (inputControllers.Contains(controller))
        {
            inputControllers.Remove(controller);
        }
    }*/


    int GetNextAvailableSlot()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (playerSlots[i] == null)
                return i;
        }

        return -1;
    }

}
