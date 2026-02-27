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
        if (other.CompareTag("Ball"))
        {
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
            return;
        }

        if (!isKickTrigger) return;
        PlayerController otherPlayer = other.GetComponent<PlayerController>();
        CPUEnemy cpu  = other.GetComponent<CPUEnemy>();

        // null checks
        if (otherPlayer == null && cpu == null) return;
        if (otherPlayer != null && otherPlayer == playerController) return;
        if (!playerController.CanApplyKickPlayerHit()) return;

        Vector3 hitDir = (other.transform.position - playerController.transform.position);
        hitDir = hitDir.sqrMagnitude > 0.0001f ? hitDir.normalized : playerController.transform.forward;

        float momentum = playerController.GetKickPlayerMomentum();

        float threshold1 = playerController.KickPlayerThreshold1;
        float threshold2 = playerController.KickPlayerThreshold2;

        if (otherPlayer != null)
        {
            otherPlayer.GetHit(null, momentum, hitDir, threshold1, threshold2);
            Debug.Log("Hit player with momentum: " + momentum + " and direction: " + hitDir);
            return;
        }
        if(cpu != null)
        {
            cpu.GetComponent<CPUEnemy>().GetHit(null, momentum, hitDir);
            Debug.Log("Hit CPU with momentum: " + momentum + " and direction: " + hitDir);
            return;
        }
    }

}
