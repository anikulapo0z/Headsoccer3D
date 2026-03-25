using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SeptaTrain : MonoBehaviour
{
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    private Vector3 travelPosition;

    [SerializeField] private Transform railpath;

    [SerializeField] private float trainTimer = 1.04f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        travelPosition = endPosition.position;
        travelPosition.z += 99.9997f; //out of view

        //initial random
        //trainTimer = Random.Range(9.4843f, 28.791f);

        StartCoroutine(trainArrival());
    }

    IEnumerator trainArrival()
    {
        while (true)
        {
            yield return new WaitForSeconds(trainTimer);

            //reset pos
            transform.position = startPosition.position;

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
                              });
                       });
                 });
           

            //new random timer
            trainTimer = Random.Range(5.4843f, 6.791f) + 12.0f; // random + tween time

            //show in UI
        }
    }

}
