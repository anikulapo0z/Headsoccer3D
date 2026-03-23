using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using static MenuManager;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;
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


    // ai stuff
    [SerializeField] Raumdeuter raumdeuter;
    [SerializeField] CPUEnemy cpu1;
    [SerializeField] CPUEnemy cpu2;
    char sideThatScored;
    [SerializeField] int numOfAIs = 0;

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
        //LoadGameStart();
    }

    void LoadGameStart()
    {
        inputControllers = PlayerInputHolder.Instance.playerList;
        ballObject = Instantiate(ballPrefab, ballStartingPos.position, Quaternion.identity);
        ballDropHalftone.setBallTransform(ballObject.transform);
        ballDropHalftoneWalls.setBallTransform(ballObject.transform);
        camera.target = ballObject.transform;


        //CreatePlayers();


        StartCoroutine(StartGameCountDown());
    }


    void CreatePlayers()
    {
        foreach (var player in inputControllers)
        {
            GameObject playerObj = Instantiate(characterPrefab);
            playerCharacters.Add(playerObj);

            PlayerController playerController = playerObj.GetComponent<PlayerController>();

            // disable so we can set position and rotation
            playerObj.GetComponent<CharacterController>().enabled = false;


            player.SetControlledObject(playerController);

            if (inputControllers.Count > 2)
            {
                cpu1.gameObject.SetActive(false);
                cpu2.gameObject.SetActive(false);

                playerObj.transform.position = FourP_SpawnPoints[inputControllers.IndexOf(player)].transform.position;
                playerObj.transform.rotation = Quaternion.Euler(FourP_SpawnPoints[inputControllers.IndexOf(player)].transform.eulerAngles);
            }
            else
            {
                raumdeuter.charactersToLookFor[inputControllers.IndexOf(player)] = playerObj.transform;
                cpu1.realPlayers[inputControllers.IndexOf(player)] = playerObj.transform;
                cpu2.realPlayers[inputControllers.IndexOf(player)] = playerObj.transform;
                cpu1.ball = ballObject.transform;
                cpu1.ball = ballObject.transform;

                playerObj.transform.position = TwoP_SpawnPoints[inputControllers.IndexOf(player)].transform.position;
                playerObj.transform.rotation = Quaternion.Euler(TwoP_SpawnPoints[inputControllers.IndexOf(player)].transform.eulerAngles);
            }
            playerObj.GetComponent<CharacterController>().enabled = true;
        }

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
        while(currentStartCoundown > 0)
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

        // only 2 AI's rn
        if (numOfAIs == 0 || numOfAIs == 1 || numOfAIs == 3)
        {
            cpu1.gameObject.SetActive(false);
            cpu2.gameObject.SetActive(false);
            return;
        }

        // only made for 2 AI rn, CHANGE in future
        for (int i = 0; i < numOfAIs; i++)
        {
            if (MenuManager.Instance.GetTeamSize() == MenuManager.TeamSizes.v2)
            {

                cpu1.transform.position = FourP_SpawnPoints[2].transform.position;
                cpu2.transform.position = FourP_SpawnPoints[3].transform.position;


            }


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
        // call EndGame next time the ball scores of touches the ground
        EndGame();
    }

    IEnumerator EndGame()
    {
        scoreTracker.canScore = false;
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
        foreach(GameObject g in fakeballList)
        {
            Destroy(g);
        }
        fakeballList.Clear();
    }

    public void GoalScored(char c)
    {
        PauseTimer();
        if (c == ' ') sideThatScored = ' ';
        else sideThatScored = c;
        scoreTracker.canScore = false;
        StartCoroutine(ResetBall());
    }
    void TossBall()
    {
        ResumeTimer();
        if (sideThatScored == 'l')
            ballObject.GetComponent<Rigidbody>().AddForce(new Vector3(-1.5f, 0, 0), ForceMode.Impulse);
        else if (sideThatScored == 'r')
            ballObject.GetComponent<Rigidbody>().AddForce(new Vector3(1.5f, 0, 0), ForceMode.Impulse);
        else
            ballObject.GetComponent<Rigidbody>().AddForce(Vector3.zero, ForceMode.Impulse);
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

        cpu1.enabled = false;
        cpu1.GetComponent<NavMeshAgent>().enabled = false;
        cpu2.enabled = false;
        cpu2.GetComponent<NavMeshAgent>().enabled = false;

    }
    void UnlockPlayers()
    {
        foreach (var player in playerCharacters)
        {
            player.GetComponent<PlayerController>().UnlockPlayerMove();
        }

        cpu1.enabled = true;
        cpu1.GetComponent<NavMeshAgent>().enabled = true;
        cpu2.enabled = true;
        cpu2.GetComponent<NavMeshAgent>().enabled = true;
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

    }

    void SetupTeamSize(MenuManager.TeamSizes teamSize)
    {
        int playerCount = PlayerInputHolder.Instance.playerList.Count;
        switch (teamSize)
        {
            case MenuManager.TeamSizes.v1:

                //foreach(var t in playerCharacters) { }

                if(playerCount == 1)
                {
                    // 1 AI
                    InstanPlayers(MenuManager.TeamSizes.v1, 1);
                }
                else
                {
                    InstanPlayers(MenuManager.TeamSizes.v1, 0);

                    // no AI
                }
                break;

            case MenuManager.TeamSizes.v2:
                if(playerCount == 1)
                {
                    // 3 AI
                    InstanPlayers(MenuManager.TeamSizes.v2, 3);

                }
                else if(playerCount == 2)
                {
                    // 2 AI
                    InstanPlayers(MenuManager.TeamSizes.v2, 2);

                }
                else if(playerCount == 3)
                {
                    // 1 AI
                    InstanPlayers(MenuManager.TeamSizes.v2, 1);

                }
                else
                {
                    // no AI
                    InstanPlayers(MenuManager.TeamSizes.v2, 0);

                }
                break;

            default:
                Debug.LogError("Team size isnt valid");
                break;

        }
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

            default:
                Debug.LogError("O GOD O GOD O GOD, THE GAMEMODE ISNT VALID!");
                break;
        }
    }

    void InstanPlayers(MenuManager.TeamSizes teamSizes, int aiCount)
    {
        numOfAIs = aiCount;
        foreach (var player in inputControllers)
        {
            //Debug.LogError(player.selectedCharacterID);
            GameObject playerObj = Instantiate(characterPrefab);
            playerCharacters.Add(playerObj);

            PlayerController playerController = playerObj.GetComponent<PlayerController>();

            // disable so we can set position and rotation
            playerObj.GetComponent<CharacterController>().enabled = false;

            // set player position
            if (teamSizes == MenuManager.TeamSizes.v1)
            {
                playerObj.transform.position = TwoP_SpawnPoints[inputControllers.IndexOf(player)].transform.position;


            }
            else
                playerObj.transform.position = FourP_SpawnPoints[inputControllers.IndexOf(player)].transform.position;


            // setting team ui based on X position, we'll see if this works
            //material set based on selected character
            playerObj.GetComponent<PlayerGroundMarker>().SetPlayerWorldUIAndColor(playerObj.transform.position.x < 0 ? 
                                                                                    leftTeamPositionIndicator : rightTeamPositionIndicator, 
                                                                                    characterMaterials[player.selectedCharacterID]);



            if (aiCount == 2)
            {
                cpu1.realPlayers[inputControllers.IndexOf(player)] = playerObj.transform;
                cpu2.realPlayers[inputControllers.IndexOf(player)] = playerObj.transform;
                cpu1.ball = ballObject.transform;
                cpu2.ball = ballObject.transform;
                raumdeuter.charactersToLookFor[inputControllers.IndexOf(player)] = playerObj.transform;
            }

            player.SetControlledObject(playerController);

            playerObj.GetComponent<CharacterController>().enabled = true;
        }

        // only 2 AI's rn
        if (aiCount == 0 || aiCount == 1 || aiCount == 3)
        {
            Debug.LogError("turning off ai");
            cpu1.gameObject.SetActive(false);
            cpu2.gameObject.SetActive(false);
            return;
        }
        
        // only made for 2 AI rn, CHANGE in future
        for (int i = 0; i < aiCount; i++)
        {
            if (teamSizes == MenuManager.TeamSizes.v2)
            {

                cpu1.transform.position = FourP_SpawnPoints[2].transform.position;
                cpu2.transform.position = FourP_SpawnPoints[3].transform.position;


            }


        }

    }

}
