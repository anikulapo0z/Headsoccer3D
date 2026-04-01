using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MenuManager;
using static UnityEngine.Rendering.DebugUI;

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

    [SerializeField] Image transitionImage;
    private Material transitionMaterial;
    [SerializeField] Image exitTransitionImage;
    private Material exitTransitionMaterial;

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
    public Action gameTimeTick;

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
    [Space(10)]
    [Header("Win Game")]
    [SerializeField] Transform[] winAreaSpawnPoints;
    List<GameObject> leftTeam = new List<GameObject>();
    List<GameObject> rightTeam = new List<GameObject>();
    [SerializeField] GameObject winAreaCamera;
    [SerializeField] RawImage winAreaImage;
    [SerializeField] Material blurMaterial;
    Material winAreaMaterial;
    bool gameOver = false;

    [SerializeField] float totalWinAreaTime;
    [SerializeField] float currentWinAreaTime;
    [SerializeField] float startFadeWinArea;
    Coroutine winAreaCoroutine;

    [SerializeField] MenuMusic backgroundMusic;


    void Start()
    {
        Instance = this;

        //reset materials
        winAreaMaterial = winAreaImage.material;
        if (winAreaMaterial)
            winAreaMaterial.SetFloat("_Transition", 0f);
        if (blurMaterial)
            blurMaterial.SetFloat("_GridSize", 0f);
        transitionMaterial = transitionImage.material;
        transitionMaterial.SetFloat("_Transition", 0f);
        //set exit materials now and reset
        exitTransitionImage.gameObject.SetActive(false);
        exitTransitionMaterial = exitTransitionImage.material;
        exitTransitionMaterial.SetFloat("_Transition", 0);

        SetUpGameLevel(MenuManager.Instance.GetTeamSize(), MenuManager.Instance.GetGameMode());

        Debug.Log("HUIS");
        StartCoroutine(fadeTransitionThenLoad());
    }

    IEnumerator fadeTransitionThenLoad()
    {

        Debug.Log("FDEYHP*AEHFP uisyhfep i");

        //make sure its on, in case we disable it in editor while working and we forget
        transitionImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.659f);

        float elapsed = 0f;

        while (elapsed < 2.05f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 2.05f);

            transitionMaterial.SetFloat("_Transition", t);

            yield return null;
        }
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
            gameTimeTick?.Invoke();
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
            StartCoroutine(EndGame());
        }
    }


    IEnumerator EndGame()
    {
        scoreTracker.canScore = false;

        if (gameTimeCoroutine != null)
            StopCoroutine(gameTimeCoroutine);

        LockPlayers();

        yield return new WaitForSeconds(startFadeWinArea);

        //everything is locked
        ////start blur
        float _Timer = 0f;
        float _blurTime = 2.0794f;
        blurMaterial.SetFloat("_GridSize", 0f);
        while (_Timer < _blurTime)
        {
            _Timer += Time.deltaTime ;
            blurMaterial.SetFloat("_GridSize", (_Timer / _blurTime) * 10f);
            yield return null;
        }
        blurMaterial.SetFloat("_GridSize", 10f);

        //init Win area 
        winAreaCamera.SetActive(true);
        winAreaImage.gameObject.SetActive(true);

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

        if (scoreTracker.WhichTeamWon() == "left")
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
        else if(scoreTracker.WhichTeamWon() == "right")
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
        else
        {

            foreach (var p in rightTeam)
            {
                p.GetComponent<PlayerController>().SetFalling();
            }
            foreach (var p in leftTeam)
            {
                p.GetComponent<PlayerController>().SetFalling();
            }

            GameSceneManager.Instance.gameObject.GetComponent<WordSpawner>().SpawnWord("tie game", -1);
            GameSceneManager.Instance.gameObject.GetComponent<WordSpawner>().SpawnWord("letters to", 7);
            GameSceneManager.Instance.gameObject.GetComponent<WordSpawner>().SpawnWord("play with", 4);
        }





            //fade in the win area

            _Timer = 0f;
        _blurTime = 1.8736f;
        winAreaMaterial = winAreaImage.material;
        winAreaMaterial.SetFloat("_Transition", 0f);
        while (_Timer < _blurTime)
        {
            _Timer += Time.deltaTime;
            winAreaMaterial.SetFloat("_Transition", _Timer / _blurTime);
            yield return null;
        }
        winAreaMaterial.SetFloat("_Transition", 1f);

        //let them move
        UnlockPlayers();

        //start end timer
        currentWinAreaTime = totalWinAreaTime;
        winAreaCoroutine = StartCoroutine(WinAreaCountDown());
    }

    IEnumerator WinAreaCountDown()
    {
        yield return new WaitForSeconds(currentWinAreaTime);
        //string name = SceneManager.GetActiveScene().name;

        PlayerInputHolder.Instance.KillSingletons();

        //load main menu after fade
        backgroundMusic.FadeOut();

        exitTransitionImage.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < 2.05f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 2.05f);

            exitTransitionMaterial.SetFloat("_Transition", t);

            yield return null;
        }

        exitTransitionMaterial.SetFloat("_Transition", 1f);
        yield return new WaitForSeconds(0.4986f);

        //reset materials
        //if (winAreaMaterial)
        //    winAreaMaterial.SetFloat("_Transition", 0f);
        if (blurMaterial)
           blurMaterial.SetFloat("_GridSize", 0f);

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