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


    public void Start()
    {
        fractureRoutine = StartCoroutine(Fracture());
    }

    IEnumerator Fracture()
    {

        yield return new WaitForSeconds(startDelay);

        while (!paused)
        {
            int index = Random.Range(0, iceFragments.Length);
            iceFragments[index].GetComponent<IceShard>().sinking = true;

            iceFragments[index].DOLocalMoveY(0.6f, moveSpeed)
                .OnComplete(() => iceFragments[index].DOLocalMoveY(sinkPosition, moveSpeed * 5));


            yield return new WaitForSeconds(fractureDelay);
        }


    }


}
