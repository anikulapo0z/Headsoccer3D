using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    Rigidbody rb;

    CPUEnemy[] CPUPlayers = null;
    bool areThereCPUPlayers = true; //inital val to true is important

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        if(collision.gameObject.tag.Contains("Team") || collision.gameObject.tag.Contains("Player"))
        {
            transform.parent = collision.transform;

            //reset the ball
            resetBallParent();
            
            if (collision.gameObject.tag.Contains("CPU"))
                collision.gameObject.GetComponent<CPUEnemy>().holdingBall = true;

            //physics
            Vector3 _dir = (transform.position - collision.transform.position);
            float _playerBallDot = Vector3.Dot(collision.transform.forward, _dir);

            //ball is in forward of the player
            if(_playerBallDot > 0.2f)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(Vector3.up);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Contains("Team") || collision.gameObject.tag.Contains("Player"))
        {
            resetBallParent();
        }
    }
}
