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

    [Header("Settings")]
    [SerializeField] float characterSelectOpenDelay = 3f;
    [SerializeField] int maxPlayers = 4;

    [Header("Cursor")]
    [SerializeField] GameObject[] characterCursorPrefab;
    [SerializeField] RectTransform characterCursorParent;
    [SerializeField] RectTransform startingCharacterButton;


    [Header("Input")]
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] string actionMapName;
    [SerializeField] string joinActionName = "Join";

    [Space(10)]
    [SerializeField] MapSelectionManager mapSelectionManager;
    [SerializeField] List<PlayerInputController> inputControllers = new List<PlayerInputController>();

    bool characterSelectOpen;
    bool isLocked;

    InputAction joinAction;

    readonly Dictionary<InputDevice, PlayerInputController> players =
        new Dictionary<InputDevice, PlayerInputController>();

    void Awake()
    {
        var map = inputActions.FindActionMap(actionMapName);
        joinAction = map.FindAction(joinActionName);

        joinAction.performed += OnJoinPerformed;
    }

    void OnEnable()
    {
        InputSystem.onAnyButtonPress.Call(OnAnyButtonPressed);
    }

    void OnDisable()
    {
        //InputSystem.onAnyButtonPress.Clear();
        joinAction.performed -= OnJoinPerformed;
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
        CharacterSelectManager.Instance.StartCountDown();
    }

    void OnJoinPerformed(InputAction.CallbackContext ctx)
    {
        if (!characterSelectOpen)
            return;
        Debug.Log(players.Count);

        if (players.Count >= maxPlayers)
            return;

        InputDevice device = ctx.control.device;

        if (players.ContainsKey(device))
            return;

        int playerIndex = players.Count;

        PlayerInputController controller = CreatePlayerController(playerIndex, device);
        mapSelectionManager.inputControllers = inputControllers;

        IPlayerControllable cursor = CreateCursor(playerIndex);

        controller.SetControlledObject(cursor);
        players.Add(device, controller);

        PlayerInputHolder.Instance.playerList.Add(controller);
        DontDestroyOnLoad(controller);

        Debug.Log($"Player {playerIndex + 1} joined using {device.displayName}");
        CharacterSelectManager.Instance.totalPlayerCount = players.Count;
    }

    PlayerInputController CreatePlayerController(int index, InputDevice device)
    {
        GameObject playerObj = new GameObject($"PlayerInput_{index}");
        PlayerInputController controller = playerObj.AddComponent<PlayerInputController>();

        controller.Initialize(index, device, inputActions, actionMapName);
        inputControllers.Add(controller);

        return controller;
    }

    IPlayerControllable CreateCursor(int playerIndex)
    {
        GameObject cursorObj = Instantiate(characterCursorPrefab[playerIndex], characterCursorParent);

        SelectionCursor cursor = cursorObj.GetComponent<SelectionCursor>();
        if (cursor == null)
        {
            Debug.LogError("Cursor prefab missing SelectionCursor!");
            return null;
        }
        cursor.playerInputController = inputControllers[playerIndex];
        cursor.playerIndex = playerIndex;

        if (startingCharacterButton != null)
        {
            cursorObj.transform.SetParent(startingCharacterButton, false);
            cursorObj.transform.localPosition = Vector3.zero;

            cursor.parent = startingCharacterButton;
        }

        return cursor;
    }

    public void AssignControlledObjectToPlayer(
        InputDevice device,
        IPlayerControllable controllable
    )
    {
        if (players.TryGetValue(device, out var controller))
        {
            controller.SetControlledObject(controllable);
        }
    }

    public int PlayerCount => players.Count;

    public IEnumerable<PlayerInputController> AllPlayers => players.Values;

    public void ResetPlayers()
    {
        foreach (var controller in players.Values)
        {
            Destroy(controller.gameObject);
        }

        players.Clear();

        characterSelectOpen = false;
        isLocked = false;

        joinAction.Disable();

        pressAnyButtonScreen.SetActive(true);
        characterSelectScreen.SetActive(false);
    }
}
