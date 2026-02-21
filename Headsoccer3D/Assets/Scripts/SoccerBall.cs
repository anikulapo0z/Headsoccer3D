using System.Collections;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    Rigidbody rb;

    //Additions for momentum and thresholds
    [SerializeField] private float threshold1 = 5f;
    [SerializeField] private float threshold2 = 15f;
    [SerializeField] private float threshold3 = 30f;

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

        dir.Normalize();
        rb.linearVelocity = Vector3.zero;
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
        if (!collision.gameObject.CompareTag("Team") && !collision.gameObject.CompareTag("Player"))
            return;
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
            return;

        float momentum = rb.linearVelocity.magnitude * rb.mass;
        Vector3 hitDirection = rb.linearVelocity.normalized;
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
        // ball bounces off the player
        if(momentum < threshold1)
        {
            // gain control
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            currentActivePlayer = player;

            player.GetHit(this, momentum, hitDirection);
        }

        //medium momentum, ball is deflected
        else if (momentum < threshold2)
        {
            player.GetHit(this, momentum, hitDirection);
        }

        //high momentum, ball is deflected more
        else
        {
            player.GetHit(this, momentum, hitDirection);
            Physics.IgnoreCollision(GetComponent<Collider>(),collision.collider,true);

            StartCoroutine(ReenableCollision(collision.collider, 0.2f));
        }
    }
    private IEnumerator ReenableCollision(Collider col, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics.IgnoreCollision(GetComponent<Collider>(), col, false);
    }
}
