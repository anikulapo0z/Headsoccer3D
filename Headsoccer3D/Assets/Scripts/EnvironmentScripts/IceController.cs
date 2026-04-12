using System.Collections;
using UnityEngine;
using DG.Tweening;

public class IceController : MonoBehaviour
{
    public bool paused = false;
    [SerializeField] Transform[] iceFragments;
    [SerializeField] float startDelay;
    [SerializeField] float fractureDelay;
    [SerializeField] float sinkPosition;
    [SerializeField] float moveSpeed;

    Coroutine fractureRoutine;


    public void StartIceFracture()
    {
        fractureRoutine = StartCoroutine(Fracture());
    }

    IEnumerator Fracture()
    {

        yield return new WaitForSeconds(startDelay);

        while (!paused)
        {
            int index = Random.Range(0, iceFragments.Length);

            iceFragments[index].DOMoveY(sinkPosition, moveSpeed);


            yield return new WaitForSeconds(fractureDelay);
        }


    }


}
