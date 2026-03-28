using Unity.Cinemachine;
using UnityEngine;

public class PlayerTriggers : MonoBehaviour
{
    Collider col;
    [SerializeField] PlayerController playerController;
    [SerializeField] bool isKickTrigger;
    [SerializeField] bool isHeadTrigger;
    bool ballHit = false;
    [SerializeField] float letterKickForce;
    [SerializeField] float yLetterForce;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    public void TurnOnCollider()
    {
        col.enabled = true;
        ballHit = false;
    }
    public void TurnOffCollider()
    {
        col.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") || other.CompareTag("FakeBall"))
        {
            if (ballHit) return;
            ballHit = true;

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

        if (other.CompareTag("Letter"))
        {
            Vector3 xz = (other.gameObject.transform.position - transform.position).normalized;
            other.GetComponent<Rigidbody>().AddForce(new Vector3(xz.x, yLetterForce, xz.z) * letterKickForce, ForceMode.Impulse);
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
            otherPlayer.GetHitFromPlayer(momentum, hitDir);
            Debug.Log("Hit player with momentum: " + momentum + " and direction: " + hitDir);
            SoccerBall ball = FindFirstObjectByType<SoccerBall>();
            if (ball != null && ball.HasPossession(otherPlayer))
            {
                ball.ReleasePossession(playerController);
            }
            return;
        }
        if (cpu != null)
        {
            cpu.GetComponent<CPUEnemy>().GetHit(null, momentum, hitDir);
            Debug.Log("Hit CPU with momentum: " + momentum + " and direction: " + hitDir);
            return;
        }

        // If they had the ball, forcibly dispossess
        
    }

}
