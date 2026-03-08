using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;

    public enum MapType
    {
        Default,
        Stadium,
        Street,
        Training
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

    bool canScore = false;

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
        EndGame();
    }


    IEnumerator EndGame()
    {
        scoreTracker.canScore = false;

        if (gameTimeCoroutine != null)
            StopCoroutine(gameTimeCoroutine);

        yield return new WaitForSeconds(delayBeforeScoreScreen);

        // load score screen
    }


    public IEnumerator ResetBall()
    {
        yield return new WaitForSeconds(delayBeforeResetBall);

        ResetPlayers();
        LockBall();
        DestroyFakeBalls();

        yield return new WaitForSeconds(delayBeforeUnlockPlayer);

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

            case MapType.Stadium:
                break;

            case MapType.Street:
                break;

            case MapType.Training:
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


            playerObj.GetComponent<PlayerGroundMarker>().SetPlayerWorldUIAndColor(
                playerObj.transform.position.x < 0 ? leftTeamPositionIndicator : rightTeamPositionIndicator,
                characterMaterials[player.selectedCharacterID]
            );

            player.SetControlledObject(playerController);

            playerObj.GetComponent<CharacterController>().enabled = true;
        }
    }
}