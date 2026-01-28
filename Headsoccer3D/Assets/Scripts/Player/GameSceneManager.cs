using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.AI;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;
    public List<PlayerInputController> inputControllers = new List<PlayerInputController>();
    public List<GameObject> playerCharacters = new List<GameObject>();

    public List<GameObject> characterList = new List<GameObject>();
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
    [SerializeField] Transform ballStartingPos;
    [SerializeField] ScoreTracker scoreTracker;


    // ai stuff
    [SerializeField] Raumdeuter raumdeuter;
    [SerializeField] CPUEnemy cpu1;
    [SerializeField] CPUEnemy cpu2;
    char sideThatScored;


    void Start()
    {
        Instance = this;
        LoadGameStart();
    }

    public void LoadGameStart()
    {
        inputControllers = PlayerInputHolder.Instance.playerList;
        ballObject = Instantiate(ballPrefab, ballStartingPos.position, Quaternion.identity);

        CreatePlayers();


        StartCoroutine(StartGameCountDown());
    }


    void CreatePlayers()
    {
        foreach (var player in inputControllers)
        {
            GameObject playerObj = Instantiate(characterList[player.selectedCharacterID]);
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
            if (inputControllers.Count > 2)
            {
                player.transform.position = FourP_SpawnPoints[playerCharacters.IndexOf(player)].transform.position;
                player.transform.rotation = Quaternion.Euler(FourP_SpawnPoints[playerCharacters.IndexOf(player)].transform.eulerAngles);
            }
            else
            {
                player.transform.position = FourP_SpawnPoints[playerCharacters.IndexOf(player)].transform.position;
                player.transform.rotation = Quaternion.Euler(FourP_SpawnPoints[playerCharacters.IndexOf(player)].transform.eulerAngles);

                cpu1.enabled = false;
                cpu1.GetComponent<NavMeshAgent>().enabled = false;
                cpu1.transform.position = FourP_SpawnPoints[2].transform.position;
                cpu1.transform.rotation = Quaternion.Euler(FourP_SpawnPoints[2].transform.eulerAngles);

                cpu2.enabled = false;
                cpu2.GetComponent<NavMeshAgent>().enabled = false;
                cpu2.transform.position = FourP_SpawnPoints[3].transform.position;
                cpu2.transform.rotation = Quaternion.Euler(FourP_SpawnPoints[3].transform.eulerAngles);

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

        yield return new WaitForSeconds(delayBeforeUnlockPlayer);
        UnlockPlayers();
        UnlockBall();
        TossBall();
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
        ballObject.GetComponent<SphereCollider>().enabled = true;
        ballObject.GetComponent<Rigidbody>().isKinematic = false;
        ballObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }



}
