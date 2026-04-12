using UnityEngine;
using DG.Tweening;

public class MoveBus : MonoBehaviour
{
    [SerializeField] float minX;
    [SerializeField] float maxX;
    [SerializeField] float minMoveSpeed;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float minWaitTime;
    [SerializeField] float maxWaitTime;

    Tween moveTween;
    Sequence sequence;
    bool isPaused = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Pause();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Resume();
        }
    }

    void Start()
    {
        StartMove();
    }

    private void StartMove()
    {
        float targetX = Random.Range(minX, maxX);
        float moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        float waitTime = Random.Range(minWaitTime, maxWaitTime);

        sequence = DOTween.Sequence();
        moveTween = transform.DOMoveX(targetX, moveSpeed).SetEase(Ease.InOutSine);
        sequence.Append(moveTween).AppendInterval(waitTime).AppendCallback(StartMove);
    }

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;
        sequence?.Pause();
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;
        sequence?.Play();
    }

}