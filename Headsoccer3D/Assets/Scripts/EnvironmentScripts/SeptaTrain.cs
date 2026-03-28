using UnityEngine;
using DG.Tweening;
using System.Collections;
using TMPro;

public class SeptaTrain : MonoBehaviour
{
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    private Vector3 travelPosition;
    [SerializeField] private bool trainArriving = false;
    [SerializeField] private Transform railpath;

    [Space]
    [Header("Time Settings")]
    [SerializeField] private int trainTimer = 1;
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
    [SerializeField] private int trainUITime = 10;
    [SerializeField] private string trainUIDestination;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        travelPosition = endPosition.position; //train stop position
        travelPosition.z += 99.9997f; //out of view

        //update UI on the tick
        gameSceneManager.gameTimeTick += updateUI;

        StartCoroutine(trainArrival());
    }

    IEnumerator trainArrival()
    {
       
        //initial 
        trainTimer = Random.Range(trainRandomLowerLimit, trainRandomUpperLimit);
        updateUIValues(0); // no tweenTime for the initial

        while (true)
        {
            //a new train loop started
            yield return new WaitForSeconds(trainTimer);

            //reset pos
            transform.position = startPosition.position;
            trainArriving = true;

            yield return null; //wait a frame

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
                              });
                       });
                 });


            
            //new random timer
            trainTimer = Random.Range(trainRandomLowerLimit, trainRandomUpperLimit) + 12; // random + tween time
            updateUIValues(12); //update the UI values, and pass the tween time

        }
    }

    //returns int in seconds of what the offset should be in the UI for train arrival
    private int calculateLateOffset()
    {
        //coin toss, if heads, return no late. if tails, return late value 
        return Random.value > 0.5f ? 0 : Random.Range(lateRandomLowerLimit, lateRandomUpperLimit);
    }

    private void updateUIValues(int _tweenTime)
    {
        string _temp;
        int _index;

        //Destination
        _index = Random.Range(0, destinationNames.Length - 2); //random destination using bag (exclude the last element)
        trainUIDestination = destinationNames[_index];
        //swap the _index and last
        _temp = destinationNames[_index];
        destinationNames[_index] = destinationNames[destinationNames.Length - 1];
        destinationNames[destinationNames.Length - 1] = _temp;

        //Time
        trainUITime = trainTimer - calculateLateOffset() - _tweenTime; //time - late - tweentime
    }
    
    //invoked when game scene manager will ping when a timer goes down
    //doing this way so that the UI change is synced with game's timer, both internally and visually
    public void updateUI()
    {
        if(trainArriving)
        {
            UI_destinationTimer.color = Color.white;
            UI_destinationTimeNumber.color = Color.white;

            UI_destinationTimer.text = "In Platform";
            UI_destinationTimeNumber.text = "";

            return;
        }
        trainUITime--;
        UI_destinationTimer.text = trainUITime < 0 ? "LATE " : "ON TIME ";
        UI_destinationTimeNumber.text = Mathf.Abs(trainUITime).ToString();
        UI_destinationTimer.color = trainUITime < 0 ? Color.yellow : Color.green;
        UI_destinationTimeNumber.color = trainUITime < 0 ? Color.yellow : Color.green;
        UI_destinationName.text = trainUIDestination;
    }

    
}
