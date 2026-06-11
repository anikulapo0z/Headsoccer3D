using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;


    // players joining
    [SerializeField] int totalPlayerCount;
    [SerializeField] bool canMoveToNextScreen = false;
    public GameObject pressConfirmPrompt;
    public GameObject wrongPlayerCountPrompt;
    [SerializeField] int lockedPlayerCount;


    // menu space references
    [SerializeField] GameObject characterSelectMenu;
    [SerializeField] GameObject mapSelectMenu;
    [SerializeField] RectTransform mainCanvas;


    bool force2v2 = false;
    [SerializeField] PlayerCountSwitch playerCountSwitch;


    public TeamSizes currentTeamSize = TeamSizes.v1;
    public GameMode currentGameMode = GameMode.Classic;


    // character portraits
    [SerializeField] CharacterPortrait[] portraits;

    [SerializeField] Transform cursorParent;
    [SerializeField] GameObject mapCursorPrefab;

    public PlayerJoinManager joinManager;

    public bool isPaused = false;

    [Space]
    [SerializeField] Image transitionImage;
    private Material transitionMaterial;



    [Header("How To Play")]
    [SerializeField] GameObject howToPlayerCharacter;
    [SerializeField] Transform howToPlayerPosition;
    public Transform cursorHolder_map;
    public Transform cursorHolder_character;

    [Space(10)]
    public HowToPlayHighlights jumpHighlight1_controller;
    public HowToPlayHighlights jumpHighlight2_controller;
    public HowToPlayHighlights jumpHighlight_exit_controller;
    public HowToPlayHighlights kickHighlight_controller;
    public HowToPlayHighlights kickHighlight_exit_controller;
    public HowToPlayHighlights moveHighlight_controller;
    public HowToPlayHighlights abilityHighlight_controller;
    public HowToPlayHighlights sprintHighlight_controller;
    public HowToPlayHighlights poseTauntHighlight_controller;
    public HowToPlayHighlights textTauntHighlight_controller;

    [Space]
    public HowToPlayHighlights jumpHighlight_arcade;
    public HowToPlayHighlights jumpHighlight_exit_arcade;
    public HowToPlayHighlights kickHighlight_arcade;
    public HowToPlayHighlights kickHighlight_exit_arcade;
    public HowToPlayHighlights moveHighlight_arcade;
    public HowToPlayHighlights abilityHighlight_arcade;
    public HowToPlayHighlights sprintHighlight_arcade;
    public HowToPlayHighlights poseTauntHighlight_arcade;
    public HowToPlayHighlights textTauntHighlight_arcade;


    public GameObject[] objectsToTurnBackOn;
    public GameObject[] objectsToTurnBackOff;

    [SerializeField] MenuAudioManager menuAudioManager;
    public bool isHowToPlayOpen = false;
    [SerializeField] List<GameObject> maps = new List<GameObject>();

    public enum MenuScreen
    {
        CharacterSelect,
        MapSelect
    }
    public MenuScreen currentScreen { get; private set; } = MenuScreen.CharacterSelect;



    public enum TeamSizes
    {
        v1,
        v2
    };
    public enum GameMode
    {
        Classic,
        FFA,
        RandomBall,
        StageHazards,
        RandomBallAndStageHazards
    };

    private void Start()
    {
        Instance = this;
        //DontDestroyOnLoad(this);
        Cursor.visible = false;
        ResetMenu();
    }

    public void ResetMenu()
    {
        totalPlayerCount = 0;
        lockedPlayerCount = 0;
        canMoveToNextScreen = false;

        pressConfirmPrompt.SetActive(false);
        wrongPlayerCountPrompt.SetActive(false);

        //characterSelectMenu.SetActive(true);
        mapSelectMenu.SetActive(false);


        foreach (var p in portraits)
        {
            p.SetNotJoined();
        }
    }

    public void PlayerJoined(int count)
    {
        menuAudioManager.PlayCharacterJoinSfx();
        totalPlayerCount = PlayerInputHolder.Instance.playerList.Count;
        canMoveToNextScreen = false;
        pressConfirmPrompt.SetActive(false);
        wrongPlayerCountPrompt.SetActive(false);
    }
    public void CheckPlayerConfirm(bool isLocked)
    {
        if (canMoveToNextScreen)
        {
            MoveToNextScreen();
        }


        if (!isLocked)
        {




            lockedPlayerCount++;
            if (lockedPlayerCount == totalPlayerCount && totalPlayerCount % 2 == 0)
            {
                currentGameMode = GameMode.Classic;

                canMoveToNextScreen = true;
                pressConfirmPrompt.SetActive(true);
            }
            else if (lockedPlayerCount == totalPlayerCount && lockedPlayerCount == 3)
            {
                currentGameMode = GameMode.FFA;
                canMoveToNextScreen = true;
                pressConfirmPrompt.SetActive(true);
            }
            else if (lockedPlayerCount == totalPlayerCount)
            {
                wrongPlayerCountPrompt.SetActive(true);
            }
        }


    }
    public void PlayerCancel(bool isLocked)
    {
        if (isLocked)
        {
            wrongPlayerCountPrompt.SetActive(false);

            if (canMoveToNextScreen)
            {
                canMoveToNextScreen = false;
                pressConfirmPrompt.SetActive(false);
            }
            lockedPlayerCount--;
        }
    }
    void MoveToNextScreen()
    {
        currentScreen = MenuScreen.MapSelect;

        GameLogs.StartTimer(2, "Map Select Menu");

        canMoveToNextScreen = false;
        characterSelectMenu.SetActive(false);
        mapSelectMenu.SetActive(true);

        if (currentGameMode == GameMode.FFA) {
            foreach (var p in maps)
            {
                if (p.GetComponentInChildren<CharacterButton>().ffa_supported == false)
                    p.SetActive(false);
                else
                    p.SetActive(true);
            }
        }


        // -------------------------------------------------------------
        foreach(Transform t in cursorHolder_character)
        {
            t.gameObject.SetActive(false);
        }

        Vector3 centerPoint = mainCanvas.TransformPoint(mainCanvas.rect.center);


        GameObject playerControllable = Instantiate(mapCursorPrefab, centerPoint, Quaternion.identity, cursorHolder_map);
        IPlayerControllable controller = playerControllable.GetComponent<PlayerCursor>();

        //PlayerInputHolder.Instance.playerList[0].SetControlledObject(controller);

/*
        foreach(PlayerInputController t in PlayerInputHolder.Instance.playerList)
        {
            t.SetControlledObject(controller, playerControllable, true);
        }*/
        foreach (var t in joinManager.playerSlots)
        {
            if (t != null)
            {
                t.SetControlledObject(controller, playerControllable, false);
            }
        }
    }


    #region CharacterMenu
    public bool ToggleTeamSizes()
    {

        if (currentTeamSize == TeamSizes.v1)
        {
            currentTeamSize = TeamSizes.v2;
            return false;
        }
        else
        {
            currentTeamSize = TeamSizes.v1;
            return true;
        }
    }

    public void Force2v2(bool val)
    {
        force2v2 = val;
        if (force2v2)
        {
            currentTeamSize = TeamSizes.v2;
            playerCountSwitch.force2v2 = true;
            playerCountSwitch.Set2v2();
        }
    }
    public void StopForce2v2()
    {
        playerCountSwitch.force2v2 = false;
    }
    public void SetTeamSize(TeamSizes size)
    {
        currentTeamSize = size;
    }


    // character portraits
    public void SetPortraitInfo(int index, Sprite image, string name)
    {
        if (index < 0 || index >= portraits.Length)
            return;

        portraits[index].SetJoined(index, image, name);
    }


    public void SetHowToPlayer(int playerIndex)
    {
        Debug.Log("jghckhgcvkhvk");
        GameObject playerObj = Instantiate(howToPlayerCharacter, howToPlayerPosition.position, Quaternion.identity);
        playerObj.GetComponent<HowToPlayerCharacterController>().playerIndex = playerIndex;
        PlayerInputController target = joinManager.playerSlots[playerIndex];

        //howToPlayerCharacter.GetComponent<PlayerInputController>().SetControlled

        //IPlayerControllable controller = howToPlayerCharacter.GetComponent<HowToPlayerCharacterController>();
        IPlayerControllable controller = playerObj.GetComponent<HowToPlayerCharacterController>();
        joinManager.playerSlots[playerIndex].SetControlledObject(controller, howToPlayerCharacter, false);

        //GameObject playerControllable = Instantiate(mapCursorPrefab, Vector3.zero, Quaternion.identity, cursorParent);
        //IPlayerControllable controller = playerControllable.GetComponent<PlayerCursor>();

    }




    #endregion



    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
    }


    public void LoadGameLevel(string sceneName)
    {
        GameLogs.EndTimer(2);

        StartCoroutine(fadeTransitionThenLoad(sceneName));
    }

    IEnumerator fadeTransitionThenLoad(string sceneName)
    {
        menuAudioManager.FadeOut();

        float elapsed = 0f;

        transitionMaterial = transitionImage.material;

        transitionMaterial = Instantiate(transitionImage.material);
        transitionImage.material = transitionMaterial;
        transitionMaterial.SetFloat("_Transition", 0f);

        while (elapsed < 2.05f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 2.05f);

            transitionMaterial.SetFloat("_Transition", t);

            yield return null;
        }

        transitionMaterial.SetFloat("_Transition", 1f);

        yield return new WaitForSeconds(1.659f);

        SceneManager.LoadScene(sceneName);
    }

    /*    public void SetGameLevelFields(*//*Scene sceneName, LoadSceneMode mode*//*)
        {
            if(GameSceneManager.Instance == null)
            {
                Debug.LogError("No GameSceneManager, we are SOOooooo fucked");
                return;
            }

            GameSceneManager.Instance.SetUpGameLevel(currentTeamSize, currentGameMode);



        }*/



    public void DisconnectPlayer(int playerIndex)
    {
        var joinManager = this.joinManager;

        if (joinManager == null)
            return;

        PlayerInputController target = joinManager.playerSlots[playerIndex];

        if (target == null)
            return;

        target.PlayerDisconnect();

        foreach (var p in target.controlledGameObject)
        {
            Destroy(p);
        }

        if (target.portraitIndex >= 0 && target.portraitIndex < portraits.Length)
        {
            portraits[target.portraitIndex].SetNotJoined();
        }

        joinManager.playerSlots[playerIndex] = null;

        PlayerInputHolder.Instance.playerList.Remove(target);

        Destroy(target.gameObject);

        totalPlayerCount = PlayerInputHolder.Instance.playerList.Count;
        
        menuAudioManager.PlayUIBackSfx();

        Debug.Log(totalPlayerCount);

        if (lockedPlayerCount > totalPlayerCount)
            lockedPlayerCount = totalPlayerCount;

        canMoveToNextScreen = false;
        pressConfirmPrompt.SetActive(false);
    }

    public void AssignPlayerToPortrait(PlayerInputController controller)
    {
        for (int i = 0; i < portraits.Length; i++)
        {
            if (!portraits[i].IsOccupied)
            {
                portraits[i].SetJoined(controller.PlayerIndex, null, $"Player_{controller.PlayerIndex + 1}");
                controller.portraitIndex = i;
                return;
            }
        }

    }
    public void PlayerLeft(PlayerInputController controller)
    {
        if (controller == null) return;

        foreach (var go in controller.controlledGameObject)
        {
            if (go != null) Destroy(go);
        }
        controller.controlledGameObject.Clear();
        controller.controlledObject.Clear();

        if (controller.portraitIndex >= 0 && controller.portraitIndex < portraits.Length)
        {
            portraits[controller.portraitIndex].SetNotJoined();
            controller.portraitIndex = -1;
        }

        totalPlayerCount = PlayerInputHolder.Instance.playerList.Count;
        lockedPlayerCount = Mathf.Min(lockedPlayerCount, totalPlayerCount);

        canMoveToNextScreen = false;
        pressConfirmPrompt.SetActive(false);

        if (totalPlayerCount <= 2)
        {
            force2v2 = false;
            StopForce2v2();
        }

        if (totalPlayerCount > 0 && lockedPlayerCount == totalPlayerCount)
        {
            canMoveToNextScreen = true;
            pressConfirmPrompt.SetActive(true);
        }

        Debug.Log($"Player {controller.PlayerIndex + 1} left. Active players: {totalPlayerCount}");
    }
    public void CloseHowToPlay()
    {
        isHowToPlayOpen = false;
        foreach (GameObject g in objectsToTurnBackOn)
        {
            g.SetActive(true);
        }
        foreach (GameObject g in objectsToTurnBackOff)
        {
            g.SetActive(false);
        }
        foreach (Transform t in cursorHolder_character)
        {
            t.gameObject.SetActive(true);
        }

        menuAudioManager.PlayUIBackSfx();
    }
    public void OpenHowToPlay()
    {
        isHowToPlayOpen = true;
    }

    public MenuManager.TeamSizes GetTeamSize()
    {
        return currentTeamSize;
    }

    public MenuManager.GameMode GetGameMode()
    {
        return currentGameMode;
    }


    public void OnInactivityReset()
    {
        totalPlayerCount = 0;
        lockedPlayerCount = 0;
        canMoveToNextScreen = false;

        
        pressConfirmPrompt.SetActive(false);
        wrongPlayerCountPrompt.SetActive(false);

        
        foreach (var p in portraits)
            p.SetNotJoined();

        
        force2v2 = false;
        StopForce2v2();
        currentTeamSize = TeamSizes.v1;


        currentScreen = MenuScreen.CharacterSelect;
        characterSelectMenu.SetActive(true);
        mapSelectMenu.SetActive(false);
    }

    public void SetCurrentScreen(MenuScreen screen)
    {
        currentScreen = screen;
    }
}
