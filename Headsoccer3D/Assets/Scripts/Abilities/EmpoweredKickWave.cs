using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class EmpoweredKickWave : MonoBehaviour
{
    [SerializeField] float moveDist;
    [SerializeField] float moveSpeed;
    [SerializeField] float scaleAmount;
    List<GameObject> hitObjects = new List<GameObject>();

    [SerializeField] float yKick;
    [SerializeField] float ballKickForce;
    [SerializeField] float playerKickForce;
    public GameObject player;


    private void Start()
    {
        transform.DOMove(transform.position + transform.forward * moveDist, moveSpeed)
            .SetEase(Ease.Linear)
            .OnComplete(()=> transform.DOScale(new Vector3(.5f, .5f, .5f), .5f)
            .OnComplete(()=>Destroy(gameObject)));
        transform.DOScaleX(transform.localScale.x * scaleAmount, moveSpeed);
        transform.DOScaleZ(transform.localScale.z * scaleAmount, moveSpeed);
        transform.DOScaleY(transform.localScale.z * (scaleAmount * 3), moveSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 kickDirection;
        kickDirection = (transform.forward);

        kickDirection.y = 0f;
        kickDirection.Normalize();

        if (other.CompareTag("Ball") || other.CompareTag("FakeBall"))
        {
            if (hitObjects.Contains(other.gameObject)) return;
            hitObjects.Add(other.gameObject);

            other.GetComponent<SoccerBall>().LaunchAtDirection(kickDirection + (Vector3.up * yKick), ballKickForce);
        }

        PlayerController otherPlayer = other.GetComponent<PlayerController>();
        if (otherPlayer == null || other == player.GetComponent<PlayerController>()) return;

        otherPlayer.GetHitFromPlayer(playerKickForce, kickDirection);
    }
}
