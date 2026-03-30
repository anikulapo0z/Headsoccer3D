using TMPro;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SeptaTrain : MonoBehaviour
{
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    private Vector3 travelPosition;
    [SerializeField] private bool trainArriving = false;
    [SerializeField] private Transform railpath;

    [Space]
    [Header("Time Settings")]
    [SerializeField] private int trainRandomLowerLimit = 9;
    [SerializeField] private int trainRandomUpperLimit = 20;
    [SerializeField] private int lateRandomLowerLimit = 2;
    [SerializeField] private int lateRandomUpperLimit = 5;

    [Space]
    public GameSceneManager gameSceneManager;

    [Space]
    [Header("UI")]
    [SerializeField] private string[] destinationNames;
    [SerializeField] private TextMeshProUGUI UI_destinationName;
    [SerializeField] private TextMeshProUGUI UI_destinationTimer;
    [SerializeField] private TextMeshProUGUI UI_destinationTimeNumber;
    [SerializeField] private string trainUIDestination;

    [Space]
    int trainTicksRemaining = 0;
    int lateTicksRemaining = 0;
    bool isLate = false;
    bool trainSequenceRunning = false;
    int lateDisplayCounter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        travelPosition = endPosition.position; //train stop position
        travelPosition.z += 99.9997f; //out of view


        gameSceneManager.gameTimeTick += OnGameTick;

        ScheduleNextTrain(isInitial: true);
    }

    //invoked when game scene manager will ping when a timer goes down
    //doing this way so that the UI change is synced with game's timer, both internally and visually
    private void OnGameTick()
    {
        if (trainArriving || trainSequenceRunning)
        {
            //update UI on the tick
            UpdateUI();
            return;
        }

        if (isLate)
        {
            lateTicksRemaining--;
            lateDisplayCounter++;
            if (lateTicksRemaining <= 0)
            {
                isLate = false;
                CallDaTrain();
            }
            //update UI on the tick
            UpdateUI();
            return;
        }

        trainTicksRemaining--;

        if (trainTicksRemaining <= 0)
        {
            if (!gameSceneManager.canScore)
            {
                trainTicksRemaining = 1;
                UpdateUI();
                return;
            }

            //coin toss, if heads, return no late. if tails, return late value 
            if (Random.value > 0.5f)
            {
                CallDaTrain();
            }
            else
            {
                isLate = true;
                lateDisplayCounter = 0;
                lateTicksRemaining = Random.Range(lateRandomLowerLimit, lateRandomUpperLimit);
            }
        }

        UpdateUI();
    }

    private void CallDaTrain()
    {
        StartCoroutine(RunTrainSequence());
    }

    private void ScheduleNextTrain(bool isInitial)
    {
        int tweenTime = isInitial ? 0 : 12; // no tween time for initial
        trainTicksRemaining = Random.Range(trainRandomLowerLimit, trainRandomUpperLimit) + tweenTime;
        isLate = false;
        lateTicksRemaining = 0;

        PickNextDestination();
    }

    private void PickNextDestination()
    {
        int index = Random.Range(0, destinationNames.Length - 1); //random destination using bag (exclude the last element)
        trainUIDestination = destinationNames[index];
        //swap the _index and last
        string temp = destinationNames[index];
        destinationNames[index] = destinationNames[destinationNames.Length - 1];
        destinationNames[destinationNames.Length - 1] = temp;
    }

    IEnumerator RunTrainSequence()
    {
        //a new train loop started
        trainSequenceRunning = true;
        transform.position = startPosition.position; //reset pos
        trainArriving = true;

        yield return null; // wait a frame

        //better to create a sequence and play
        railpath.DOShakePosition(1.764f, 0.0198f, 10, 90, false, false)// rail shake
            .OnComplete(() =>
            {
                transform.DOMoveZ(endPosition.position.z, 5.0f) //move to stop position
                    .OnComplete(() =>
                    {
                        transform.DOMoveZ(endPosition.position.z, 2.0f) //wait at the station for a while
                            .OnComplete(() =>
                            { //wait at the station for a while
                                transform.DOMoveZ(travelPosition.z, 5.0f); // go out of view
                                trainArriving = false;
                                trainSequenceRunning = false;
                                ScheduleNextTrain(isInitial: false);
                            });
                    });
            });
    }

    public void UpdateUI()
    {
        UI_destinationName.text = trainUIDestination;

        if (trainArriving)
        {
            UI_destinationTimer.color = Color.white;
            UI_destinationTimeNumber.color = Color.white;
            UI_destinationTimer.text = "In Platform";
            UI_destinationTimeNumber.text = "";
            return;
        }

        if (isLate)
        {
            UI_destinationTimer.color = Color.yellow;
            UI_destinationTimeNumber.color = Color.yellow;
            UI_destinationTimer.text = "LATE ";
            UI_destinationTimeNumber.text = lateDisplayCounter.ToString();
        }
        else
        {
            UI_destinationTimer.color = Color.green;
            UI_destinationTimeNumber.color = Color.green;
            UI_destinationTimer.text = "ON TIME ";
            UI_destinationTimeNumber.text = trainTicksRemaining.ToString();
        }
    }
}