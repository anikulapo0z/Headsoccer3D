using System.Collections;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    Rigidbody rb;

    //Additions for momentum and thresholds
    [SerializeField] private float threshold1 = 5f;
    [SerializeField] private float threshold2 = 15f;
    [SerializeField] private float threshold3 = 30f;
    [SerializeField] private float thresholdBlend = 1.5f;

    private PlayerController currentActivePlayer;

    CPUEnemy[] CPUPlayers = null;
    bool areThereCPUPlayers = true; //inital val to true is important

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void LaunchAtDirection(Vector3 dir, float force)
    {
        currentActivePlayer = null;
        //Debug.LogError(force);

        dir.Normalize();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(dir * force, ForceMode.Impulse);    
    }

    public void AttractTowards(Vector3 targetPos, float force)
    {
        if(currentActivePlayer != null)
            return;
        Vector3 dir = (targetPos - transform.position).normalized;
        rb.AddForce(dir * force, ForceMode.Force);
    }
    public void resetBallParent()
    {
        //first time this is called, it will check if CPUPlayers are there
        //consequent calls will skip lines if not needed
        if(areThereCPUPlayers)
        {
            //here it will check
            if (CPUPlayers == null)
            {
                CPUPlayers = FindObjectsByType<CPUEnemy>(FindObjectsSortMode.None);
                areThereCPUPlayers = CPUPlayers.Length == 0;
            }
            else
            {
                for (int i = 0; i < CPUPlayers.Length; i++)
                {
                    CPUPlayers[i].holdingBall = false;
                }
            }
        }

        transform.parent = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if(collision.gameObject.tag.Contains("Team") || collision.gameObject.tag.Contains("Player"))
        //{
        //    transform.parent = collision.transform;

        //    //reset the ball
        //    resetBallParent();

        //    if (collision.gameObject.tag.Contains("CPU"))
        //        collision.gameObject.GetComponent<CPUEnemy>().holdingBall = true;

        //    //physics
        //    Vector3 _dir = (transform.position - collision.transform.position);
        //    float _playerBallDot = Vector3.Dot(collision.transform.forward, _dir);

        //    //ball is in forward of the player
        //    if(_playerBallDot > 0.2f)
        //    {
        //        rb.linearVelocity = Vector3.zero;
        //        rb.angularVelocity = Vector3.zero;
        //        rb.AddForce(Vector3.up);
        //    }
        //}
        if (!collision.gameObject.tag.Contains("Team") && !collision.gameObject.CompareTag("Player"))
            return;
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            //float momentum = rb.linearVelocity.magnitude * rb.mass * 50;
            //Vector3 hitDirection = rb.linearVelocity.normalized;
            //ResolveImpact(player, collision, momentum, hitDirection);

            float relSpeed = collision.relativeVelocity.magnitude;
            Vector3 hitDirection = (player.transform.position - transform.position);

            float momentum = rb.mass * relSpeed;

            hitDirection.y = 0f;
            if (hitDirection.sqrMagnitude < 0.0001f)
                hitDirection = collision.GetContact(0).normal; // fallback
            hitDirection.Normalize();

            ResolveImpact(player, collision, momentum, hitDirection);
            Debug.Log($"Resolve Impact {player.name} |Collision: {collision}| Momentum: {momentum} | Hit Direction: {hitDirection}");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Contains("Team") || collision.gameObject.tag.Contains("Player"))
        {
            resetBallParent();
        }
    }

    private void ResolveImpact(PlayerController player, Collision collision, float momentum, Vector3 hitDirection)
    {
        float threshold1Low = threshold1 - thresholdBlend;
        float threshold1High = threshold1 + thresholdBlend;

        float threshold2Low = threshold2 - thresholdBlend;
        float threshold2High = threshold2 + thresholdBlend;

        float threshold3Low = threshold3 - thresholdBlend;

        // ball bounces off the player
        if (momentum <= threshold1Low)
        {
            // gain control
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            currentActivePlayer = player;

            player.GetHit(this, momentum, hitDirection, threshold1Low, threshold1High);
            return;
        }

        //medium momentum, ball is deflected
        if (momentum < threshold1High)
        {
            float blend01 = Mathf.InverseLerp(threshold1Low, threshold1High, momentum);

            rb.linearVelocity *= Mathf.Lerp(0.05f, 1f, blend01);
            rb.angularVelocity *= Mathf.Lerp(0.05f, 1f, blend01);

            // Only assign control if it’s still pretty low
            if (blend01 < 0.35f)
                currentActivePlayer = player;

            player.GetHit(this, momentum, hitDirection, threshold1, threshold2);
            return;
        }

        if(momentum < threshold2High)
        {
            // Player knockback uses real tiers
            player.GetHit(this, momentum, hitDirection, threshold1, threshold2);

            float deflect01 = Mathf.InverseLerp(threshold1High, threshold2High, momentum);
            rb.AddForce(-hitDirection * deflect01 * 2f, ForceMode.Impulse);
            return;
        }

        player.GetHit(this, momentum, hitDirection, threshold1, threshold2);

        // Only pass through when it's truly high (near threshold3)
        if (momentum >= threshold3Low)
        {
            Collider ballCol = GetComponent<Collider>();
            Collider otherCol = collision.collider;

            Physics.IgnoreCollision(ballCol, otherCol, true);
            StartCoroutine(ReenableCollision(otherCol, 0.2f));
        }
    }
    private IEnumerator ReenableCollision(Collider col, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics.IgnoreCollision(GetComponent<Collider>(), col, false);
    }
}
