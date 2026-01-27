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
            if(side == GoalSide.Right)
            {
                scoreTracker.PointForRight();
            }
            else
            {
                scoreTracker.PointForLeft();
            }
        }
    }

}
