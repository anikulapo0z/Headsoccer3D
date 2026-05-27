using DG.Tweening;
//using System;
using System.Collections;
using UnityEngine;

public class IceController : MonoBehaviour
{
    public bool paused = false;
    [SerializeField] Transform[] iceFragments;
    [SerializeField] float startDelay;
    [SerializeField] float fractureDelay;
    [SerializeField] float sinkPosition;
    [SerializeField] float moveSpeed;

    Coroutine fractureRoutine;

    Transform startPos;
    [SerializeField] float unpauseDelay;


    public void Start()
    {
        fractureRoutine = StartCoroutine(Fracture());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
            ResetIce();
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
        StartCoroutine(Fracture());
    }

    public void ResetIce()
    {
        paused = true;

        foreach (var fragment in iceFragments)
        {
            fragment.DOLocalMoveY(0.33f, moveSpeed * 2f);
        }
        Invoke("Unpause", unpauseDelay);
    }
    
    void Unpause()
    {
        foreach (var fragment in iceFragments)
        {
            fragment.GetComponent<IceShard>().sinking = false;
        }
        paused = false;
    }
}
