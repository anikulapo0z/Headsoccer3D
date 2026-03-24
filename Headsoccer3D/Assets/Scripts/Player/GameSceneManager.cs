using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MenuManager;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    public enum MapType
    {
        Default,
        BusMap,
        LibertyBell
    }

    [Header("Map Setup")]
    [SerializeField] MapType mapType;

    public List<PlayerInputController> inputControllers = new List<PlayerInputController>();
    public List<GameObject> playerCharacters = new List<GameObject>();

    public GameObject characterPrefab;
    public List<GameObject> FourP_SpawnPoints = new List<GameObject>();
    public List<GameObject> TwoP_SpawnPoints = new List<GameObject>();

    // starting countdown
    [SerializeField] int currentStartCoundown = 0;
    [SerializeField] int maxStartCoundown;
    [SerializeField] TMP_Text startCountdownText;

    // delay before starting countdown
    [SerializeField] float startDelay;

    // game time
    [SerializeField] int currentGameTime = 0;
    [SerializeField] int maxGameTime;
    [SerializeField] int pausedGameTime;
    Coroutine gameTimeCoroutine;

    // time before score screen
    [SerializeField] float delayBeforeScoreScreen;

    public bool canScore = false;

    [SerializeField] float delayBeforeResetBall;
    [SerializeField] float delayBeforeUnlockPlayer;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject ballObject;
    [SerializeField] BallDropHalftone ballDropHalftone;
    [SerializeField] BallDropHalftone ballDropHalftoneWalls;
    [SerializeField] Transform ballStartingPos;
    [SerializeField] ScoreTracker scoreTracker;

    [SerializeField] CameraController camera;

    [Space(10)]
    [Header("Team Distinctions")]
    [SerializeField] Material[] characterMaterials;
    [SerializeField] GameObject leftTeamPositionIndicator;
    [SerializeField] GameObject rightTeamPositionIndicator;

    public List<GameObject> fakeballList = new List<GameObject>();



    // win game area

    [SerializeField] Transform[] winAreaSpawnPoints;
    List<GameObject> leftTeam = new List<GameObject>();
    List<GameObject> rightTeam = new List<GameObject>();
    [SerializeField] GameObject winAreaCamera;
    bool gameOver = false;

    [SerializeField] float totalWinAreaTime;
    [SerializeField] float currentWinAreaTime;
    [SerializeField] float startFadeWinArea;
    Coroutine winAreaCoroutine;




    void Start()
    {
        Instance = this;
        SetUpGameLevel(MenuManager.Instance.GetTeamSize(), MenuManager.Instance.GetGameMode());
    }

    void LoadGameStart()
    {
        inputControllers = PlayerInputHolder.Instance.playerList;

        ballObject = Instantiate(ballPrefab, ballStartingPos.position, Quaternion.identity);

        ballDropHalftone.setBallTransform(ballObject.transform);
        ballDropHalftoneWalls.setBallTransform(ballObject.transform);

        camera.target = ballObject.transform;

        StartCoroutine(StartGameCountDown());
    }


    void StartGame()
    {
        currentGameTime = maxGameTime;
        startCountdownText.text = currentGameTime.ToString();
        canScore = true;
        UnlockPlayers();
        UnlockBall();
        scoreTracker.canScore = true;

        gameTimeCoroutine = StartCoroutine(GameTimer());
    }


    public void PauseTimer()
    {
        if (gameTimeCoroutine != null)
        {
            StopCoroutine(gameTimeCoroutine);
            gameTimeCoroutine = null;
        }
    }

    public void ResumeTimer()
    {
        if (gameTimeCoroutine == null)
        {
            gameTimeCoroutine = StartCoroutine(GameTimer());
        }
    }


    IEnumerator StartGameCountDown()
    {
        startCountdownText.text = "";
        yield return new WaitForSeconds(startDelay);

        ResetPlayers();
        LockBall();

        currentStartCoundown = maxStartCoundown;
        startCountdownText.text = currentStartCoundown.ToString();

        while (currentStartCoundown > 0)
        {
            currentStartCoundown--;
            startCountdownText.text = currentStartCoundown.ToString();
            yield return new WaitForSeconds(1);
        }

        StartGame();
    }


    void ResetPlayers()
    {
        LockPlayers();

        foreach (var player in playerCharacters)
        {
            if (MenuManager.Instance.GetTeamSize() == MenuManager.TeamSizes.v1)
                player.transform.position = TwoP_SpawnPoints[playerCharacters.IndexOf(player)].transform.position;
            else
                player.transform.position = FourP_SpawnPoints[playerCharacters.IndexOf(player)].transform.position;
        }
    }


    IEnumerator GameTimer()
    {
        while (currentGameTime > 0)
        {
            yield return new WaitForSeconds(1);

            currentGameTime--;
            startCountdownText.text = currentGameTime.ToString();
        }
        gameTimeCoroutine = null;
        TryEndGame();
    }


    void TryEndGame()
    {
        if (!gameOver)
        {
            scoreTracker.canScore = false;
            gameOver = true;
            EndGame();
        }
    }


    void EndGame()
    {
        scoreTracker.canScore = false;

        if (gameTimeCoroutine != null)
            StopCoroutine(gameTimeCoroutine);

        winAreaCamera.SetActive(true);
        LockPlayers();

        foreach (var p in playerCharacters)
        {
            p.transform.localScale = p.transform.localScale * 0.7f;
            p.GetComponent<PlayerController>().SetReadyForEndArea();
        }
        foreach (var player in inputControllers)
        {
            if (playerCharacters.Count == 2)
            {
                playerCharacters[0].transform.position = winAreaSpawnPoints[0].transform.position;
                playerCharacters[1].transform.position = winAreaSpawnPoints[2].transform.position;

            }
            else if (playerCharacters.Count == 4)
            {
                playerCharacters[0].transform.position = winAreaSpawnPoints[0].transform.position;
                playerCharacters[1].transform.position = winAreaSpawnPoints[1].transform.position;
                playerCharacters[2].transform.position = winAreaSpawnPoints[2].transform.position;
                playerCharacters[3].transform.position = winAreaSpawnPoints[3].transform.position;
            }
        }

        if (scoreTracker.LeftTeamWon())
        {
            // winning team
            foreach(var p in leftTeam)
            {
                p.GetComponent<PlayerController>().SetWin();
                p.transform.localScale = p.transform.localScale * 2f;
            }

            // losing team
            foreach (var p in rightTeam)
            {
                p.GetComponent<PlayerController>().SetFalling();
            }

            GameSceneManager.Instance.gameObject.GetComponent<WordSpawner>().SpawnWord("red team", -1);
            GameSceneManager.Instance.gameObject.GetComponent<WordSpawner>().SpawnWord("wins", 7);
            GameSceneManager.Instance.gameObject.GetComponent<WordSpawner>().SpawnWord(scoreTracker.GetScore(), 4);
        }
        else
        {
            // winning team
            foreach (var p in rightTeam)
            {
                p.GetComponent<PlayerController>().SetWin();
                p.transform.localScale = p.transform.localScale * 2f;
            }

            // losing team
            foreach (var p in leftTeam)
            {
                p.GetComponent<PlayerController>().SetFalling();
            }

            GameSceneManager.Instance.gameObject.GetComponent<WordSpawner>().SpawnWord("blue team", -1);
            GameSceneManager.Instance.gameObject.GetComponent<WordSpawner>().SpawnWord("wins", 7);
            GameSceneManager.Instance.gameObject.GetComponent<WordSpawner>().SpawnWord(scoreTracker.GetScore(), 4);

        }
        UnlockPlayers();

        currentWinAreaTime = totalWinAreaTime;
        winAreaCoroutine = StartCoroutine(WinAreaCountDown());

        // load score screen
    }

    IEnumerator WinAreaCountDown()
    {

        while(currentWinAreaTime >= 0)
        {
            if(currentWinAreaTime - 3 <= 0)
            {
                // PRASIN ADD SCREEN TRANSITION
            }

            currentWinAreaTime--;
            yield return new WaitForSeconds(1);
        }
        //string name = SceneManager.GetActiveScene().name;

        PlayerInputHolder.Instance.KillSingletons();

        SceneManager.LoadScene("MainMenu");

    }


    public IEnumerator ResetBall()
    {
        yield return new WaitForSeconds(delayBeforeResetBall);

        ResetPlayers();
        LockBall();
        DestroyFakeBalls();

        yield return new WaitForSeconds(delayBeforeUnlockPlayer);

        canScore = true;
        UnlockPlayers();
        UnlockBall();
        TossBall();
    }


    void DestroyFakeBalls()
    {
        foreach (GameObject g in fakeballList)
        {
            Destroy(g);
        }

        fakeballList.Clear();
    }


    public void GoalScored(char c)
    {
        canScore = false;
        PauseTimer();

        sideThatScored = c;

        scoreTracker.canScore = false;

        StartCoroutine(ResetBall());
    }


    char sideThatScored;


    void TossBall()
    {
        ResumeTimer();

        if (sideThatScored == 'l')
            ballObject.GetComponent<Rigidbody>().AddForce(new Vector3(-1.5f, 0, 0), ForceMode.Impulse);
        else if (sideThatScored == 'r')
            ballObject.GetComponent<Rigidbody>().AddForce(new Vector3(1.5f, 0, 0), ForceMode.Impulse);

        sideThatScored = ' ';

        scoreTracker.canScore = true;
        ballObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }


    void LockPlayers()
    {
        foreach (var player in playerCharacters)
        {
            player.GetComponent<PlayerController>().LockPlayerMove();
        }
    }


    void UnlockPlayers()
    {
        foreach (var player in playerCharacters)
        {
            player.GetComponent<PlayerController>().UnlockPlayerMove();
        }
    }


    void LockBall()
    {
        ballObject.GetComponent<SphereCollider>().enabled = false;
        ballObject.GetComponent<Rigidbody>().isKinematic = true;
        ballObject.transform.position = ballStartingPos.position;
    }


    void UnlockBall()
    {
        Destroy(ballObject);

        ballObject = Instantiate(ballPrefab, ballStartingPos.position, Quaternion.identity);

        ballDropHalftone.setBallTransform(ballObject.transform);
        ballDropHalftoneWalls.setBallTransform(ballObject.transform);

        ballObject.GetComponent<SphereCollider>().enabled = true;
        ballObject.GetComponent<Rigidbody>().isKinematic = false;
        ballObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        ballObject.GetComponent<SoccerBall>().resetBallParent();
    }


    public void SetUpGameLevel(MenuManager.TeamSizes teamSize, MenuManager.GameMode mode)
    {
        LoadGameStart();

        SetupTeamSize(teamSize);
        SetupGameMode(mode);

        SetupMapSpecific();
    }


    void SetupTeamSize(MenuManager.TeamSizes teamSize)
    {
        InstanPlayers(teamSize);
    }


    void SetupGameMode(MenuManager.GameMode mode)
    {
        switch (mode)
        {
            case MenuManager.GameMode.Classic:
                break;

            case MenuManager.GameMode.RandomBall:
                break;

            case MenuManager.GameMode.StageHazards:
                break;

            case MenuManager.GameMode.RandomBallAndStageHazards:
                break;
        }
    }


    void SetupMapSpecific()
    {
        switch (mapType)
        {
            case MapType.Default:
                break;

            case MapType.BusMap:
                if (GetComponent<SetupBusMap>() != null)
                    GetComponent<SetupBusMap>().SetupBusGame(playerCharacters, inputControllers);
                else
                    Debug.LogError("'SetupBusMap' Component not found");

                    break;

            case MapType.LibertyBell:
                break;

        }
    }


    void InstanPlayers(MenuManager.TeamSizes teamSizes)
    {
        foreach (var player in inputControllers)
        {
            GameObject playerObj = Instantiate(characterPrefab);

            playerCharacters.Add(playerObj);

            PlayerController playerController = playerObj.GetComponent<PlayerController>();

            playerObj.GetComponent<CharacterController>().enabled = false;

            if (teamSizes == MenuManager.TeamSizes.v1)
                playerObj.transform.position = TwoP_SpawnPoints[inputControllers.IndexOf(player)].transform.position;
            else
                playerObj.transform.position = FourP_SpawnPoints[inputControllers.IndexOf(player)].transform.position;



            if(playerObj.transform.position.x < 0)
            {
                playerObj.GetComponent<PlayerGroundMarker>().SetPlayerWorldUIAndColor(leftTeamPositionIndicator, characterMaterials[player.selectedCharacterID]);
                leftTeam.Add(playerObj);
            }
            else
            {
                playerObj.GetComponent<PlayerGroundMarker>().SetPlayerWorldUIAndColor(rightTeamPositionIndicator, characterMaterials[player.selectedCharacterID]);
                rightTeam.Add(playerObj);
            }



            /*playerObj.GetComponent<PlayerGroundMarker>().SetPlayerWorldUIAndColor(
                playerObj.transform.position.x < 0 ? leftTeamPositionIndicator : rightTeamPositionIndicator,
                characterMaterials[player.selectedCharacterID]
            );*/

            player.SetControlledObject(playerController, playerObj, true);

            playerObj.GetComponent<CharacterController>().enabled = true;
        }
    }
}