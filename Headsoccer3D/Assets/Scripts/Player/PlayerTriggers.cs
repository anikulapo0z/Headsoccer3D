using Unity.Cinemachine;
using UnityEngine;

public class PlayerTriggers : MonoBehaviour
{
    Collider col;
    [SerializeField] PlayerController playerController;
    [SerializeField] bool isKickTrigger;
    [SerializeField] bool isHeadTrigger;


    private void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    public void TurnOnCollider()
    {
        col.enabled = true;
    }
    public void TurnOffCollider()
    {
        col.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;


        if (isKickTrigger)
        {
            playerController.OnKickTrigger(other.GetComponent<SoccerBall>());
            return;
        }

        if (isHeadTrigger)
        {
            playerController.OnHeadTrigger(other.GetComponent<SoccerBall>());
            return;
        }

    }

}
