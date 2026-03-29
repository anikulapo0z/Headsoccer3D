using System.Collections;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;


    // players joining
    [SerializeField] int totalPlayerCount;
    [SerializeField] bool canMoveToNextScreen = false;
    [SerializeField] GameObject pressConfirmPrompt;
    [SerializeField] int lockedPlayerCount;


    // menu space references
    [SerializeField] GameObject characterSelectMenu;
    [SerializeField] GameObject mapSelectMenu;


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

    public enum TeamSizes
    {
        v1,
        v2
    };
    public enum GameMode
    {
        Classic,
        RandomBall,
        StageHazards,
        RandomBallAndStageHazards
    };

    private void Start()
    {
        Instance = this;
        //DontDestroyOnLoad(this);

        ResetMenu();
    }

    public void ResetMenu()
    {
        totalPlayerCount = 0;
        lockedPlayerCount = 0;
        canMoveToNextScreen = false;

        pressConfirmPrompt.SetActive(false);

        characterSelectMenu.SetActive(true);
        mapSelectMenu.SetActive(false);


        foreach (var p in portraits)
        {
            p.SetNotJoined();
        }
    }

    public void PlayerJoined(int count)
    {
        totalPlayerCount = PlayerInputHolder.Instance.playerList.Count;
        canMoveToNextScreen = false;
        pressConfirmPrompt.SetActive(false);
    }
    public void CheckPlayerConfirm(bool isLocked)
    {

        if (!isLocked)
        {
            lockedPlayerCount++;
            if (lockedPlayerCount == totalPlayerCount)
            {
                canMoveToNextScreen = true;
                pressConfirmPrompt.SetActive(true);
            }
            return;
        }

        if (canMoveToNextScreen)
        {
            MoveToNextScreen();
        }
    }
    public void PlayerCancel(bool isLocked)
    {
        if (isLocked)
        {
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
        characterSelectMenu.SetActive(false);
        mapSelectMenu.SetActive(true);

        foreach(Transform t in cursorParent)
        {
            Destroy(t.gameObject);
        }

        GameObject playerControllable = Instantiate(mapCursorPrefab, Vector3.zero, Quaternion.identity, cursorParent);
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
                t.SetControlledObject(controller, playerControllable, true);
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




    #endregion



    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
    }


    public void LoadGameLevel(string sceneName)
    {
        StartCoroutine(fadeTransitionThenLoad(sceneName));
    }

    IEnumerator fadeTransitionThenLoad(string sceneName)
    {
        float elapsed = 0f;

        transitionMaterial = transitionImage.material;
        //make an instance at runtime 
        transitionMaterial = Instantiate(transitionImage.material);
        transitionImage.material = transitionMaterial;

        while (elapsed < 2.05f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 2.05f);

            transitionMaterial.SetFloat("_Transition", t);

            yield return null;
        }

        transitionMaterial.SetFloat("_Transition", 1f);

        //wait a min
        yield return null;
        yield return null;
        yield return null;

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



    public MenuManager.TeamSizes GetTeamSize()
    {
        return currentTeamSize;
    }

    public MenuManager.GameMode GetGameMode()
    {
        return currentGameMode;
    }
}
