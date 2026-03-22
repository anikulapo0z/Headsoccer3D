using UnityEngine;
using UnityEngine.SceneManagement;

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




/*    void OnEnable()
    {
        SceneManager.sceneLoaded += SetGameLevelFields;
    }

    // Called when the script is disabled
    void OnDisable()
    {
        SceneManager.sceneLoaded -= SetGameLevelFields;
    }
*/
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
    }

    public void PlayerJoined(int count)
    {
        totalPlayerCount = count;
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


        foreach(PlayerInputController t in PlayerInputHolder.Instance.playerList)
        {
            t.SetControlledObject(controller, true);
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
        portraits[index].SetPortraitFields(image, name);
    }




    #endregion



    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
    }


    public void LoadGameLevel(string sceneName)
    {
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

    public MenuManager.TeamSizes GetTeamSize()
    {
        return currentTeamSize;
    }

    public MenuManager.GameMode GetGameMode()
    {
        return currentGameMode;
    }
}
