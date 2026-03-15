using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] ScoreTracker scoreTracker;

    public enum GoalSide {  Left, Right };
    [SerializeField] GoalSide side;





    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            //Debug.LogError(other.tag);
            scoreTracker.ShakeCamera(other.GetComponent<Rigidbody>().linearVelocity.magnitude);
            if(side == GoalSide.Right)
            {
                scoreTracker.PointForLeft();
            }
            else
            {
                scoreTracker.PointForRight();
            }

            other.GetComponent<SoccerBall>().resetBallParent();

        }
    }

}
