using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] GameObject scoreTracker;

    public enum GoalSide {  Left, Right, FFA };
    [SerializeField] GoalSide side;
    [SerializeField] int goalIndex = -1;

    [SerializeField] AudioSource goalScoredAudioSource;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            //Debug.LogError(other.tag);
            scoreTracker.GetComponent<ScoreTracker_FFA>().ShakeCamera(other.GetComponent<Rigidbody>().linearVelocity.magnitude);
            if(side == GoalSide.Right)
            {
                scoreTracker.GetComponent<ScoreTracker>().PointForLeft();
            }
            else if(side == GoalSide.Left)
            {
                scoreTracker.GetComponent<ScoreTracker>().PointForRight();
            }
            else
            {
                scoreTracker.GetComponent<ScoreTracker_FFA>().ScoreGoal(goalIndex);
            }
                //if (goalScoredAudioSource != null)
                //{
                //    goalScoredAudioSource.Play();
                //}
                //else
                //{
                //   Debug.Log("Goal scored audio source has no clip assigned.");
                //}
                other.GetComponent<SoccerBall>().resetBallParent();

        }
    }

}
