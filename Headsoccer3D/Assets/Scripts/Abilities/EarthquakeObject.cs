using UnityEngine;

public class EarthquakeObject : MonoBehaviour
{
    [HideInInspector]
    public float yKick;
    [HideInInspector]
    public float ballKickForce;
    [HideInInspector]
    public float playerKickForce;
    [HideInInspector]
    public GameObject controllingPlayer;


    public void OnTriggerEnter(Collider other)
    {
        Vector3 kickDirection;
        kickDirection = (other.transform.position - transform.position);

        kickDirection.y = 0f;
        kickDirection.Normalize();

        Vector3 t = new Vector3(kickDirection.x * 15, kickDirection.y, kickDirection.z * 15);

        if (other.CompareTag("Ball") || other.CompareTag("FakeBall"))
        {
            other.GetComponent<SoccerBall>().LaunchAtDirection(t + (Vector3.up * yKick), ballKickForce);
        }

        PlayerController otherPlayer = other.GetComponent<PlayerController>();
        if (otherPlayer == null || otherPlayer == controllingPlayer.GetComponent<PlayerController>()) return;

        otherPlayer.GetHitFromPlayer(playerKickForce, t + (Vector3.up * yKick));
    }

}
