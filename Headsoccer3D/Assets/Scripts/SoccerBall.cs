using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    Rigidbody rb;

    Transform activeBallPlayer;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void resetBallParent()
    {
        if (activeBallPlayer.tag.Contains("CPU"))
            activeBallPlayer.GetComponent<CPUEnemy>().holdingBall = false;

        activeBallPlayer = null;
        transform.parent = null;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag.Contains("Team") || collision.gameObject.tag.Contains("Player"))
        {
            transform.parent = collision.transform;

            //set the player
            resetBallParent();
            activeBallPlayer = collision.transform;
            if (activeBallPlayer.tag.Contains("CPU"))
                activeBallPlayer.GetComponent<CPUEnemy>().holdingBall = true;

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
