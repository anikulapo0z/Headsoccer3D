using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;
    public List<PlayerInputController> inputControllers = new List<PlayerInputController>();
    
    public List<GameObject> characterList = new List<GameObject>();

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

    void Start()
    {
        Instance = this;
        LoadGameStart();
    }

    public void LoadGameStart()
    {
        inputControllers = PlayerInputHolder.Instance.playerList;
        CreatePlayers();
        StartCoroutine(StartGameCountDown());
    }


    void CreatePlayers()
    {
        foreach (var player in inputControllers)
        {
            //IPlayerControllable playerPrefab = characterList[player.selectedCharacterID];
            //player.SetControlledObject(playerPrefab);
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

}
