using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;
    public List<PlayerInputController> inputControllers = new List<PlayerInputController>();
    
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


    // ai stuff
    [SerializeField] Raumdeuter raumdeuter;
    [SerializeField] CPUEnemy cpu1;
    [SerializeField] CPUEnemy cpu2;



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
            PlayerController playerController = playerObj.GetComponent<PlayerController>();

            player.SetControlledObject(playerController);

            if (inputControllers.Count > 2)
                playerObj.transform.position = FourP_SpawnPoints[inputControllers.IndexOf(player)].transform.position;
            else
            {
                raumdeuter.charactersToLookFor[inputControllers.IndexOf(player)] = playerObj.transform;
                cpu1.realPlayers[inputControllers.IndexOf(player)] = playerObj.transform;
                cpu2.realPlayers[inputControllers.IndexOf(player)] = playerObj.transform;
                cpu1.ball = ballObject.transform;
                cpu1.ball = ballObject.transform;

                playerObj.transform.position = TwoP_SpawnPoints[inputControllers.IndexOf(player)].transform.position;
            }

            playerController.LockPlayerMove();
        }

    }

    void StartGame()
    {
        currentGameTime = maxGameTime;
        gameTimeCoroutine = StartCoroutine(GameTimer());
    }

    public void PauseTimer()
    {
        pausedGameTime = currentGameTime;
        StopCoroutine(gameTimeCoroutine);
    }
    public void ResumeTimer()
    {
        currentGameTime = pausedGameTime;
        StartCoroutine("gameTimeCoroutine");
    }


    IEnumerator StartGameCountDown()
    {
        startCountdownText.text = "";
        yield return new WaitForSeconds(startDelay);
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

    IEnumerator GameTimer()
    {
        while (currentGameTime > 0)
        {

            currentGameTime--;
            yield return new WaitForSeconds(1);
        }
    }

    void TryEndGame()
    {
        // call EndGame next time the ball scores of touches the ground
        EndGame();
    }

    IEnumerator EndGame()
    {
        canScore = false;
        StopCoroutine(gameTimeCoroutine);
        yield return new WaitForSeconds(delayBeforeScoreScreen);
        // load csore screen
    }



    public IEnumerator ResetBall()
    {
        yield return new WaitForSeconds(delayBeforeResetBall);
        foreach(var chars in inputControllers)
        {
            chars.GetComponent<PlayerController>().LockPlayerMove();
            chars.transform.position = GetComponent<PlayerController>().startingPos;
        }
        LockBall();

        yield return new WaitForSeconds(delayBeforeUnlockPlayer);
        foreach (var chars in inputControllers)
        {
            chars.GetComponent<PlayerController>().UnlockPlayerMove();
        }
        UnlockBall();
        TossBall();
    }
    void TossBall()
    {

    }
    void LockBall()
    {
        ballObject.GetComponent<SphereCollider>().enabled = false;
        ballObject.GetComponent<Rigidbody>().isKinematic = true;
    }
    void UnlockBall()
    {
        ballObject.GetComponent<SphereCollider>().enabled = true;
        ballObject.GetComponent<Rigidbody>().isKinematic = false;
        ballObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }



}
