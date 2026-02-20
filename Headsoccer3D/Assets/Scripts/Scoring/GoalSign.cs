using UnityEngine;
using DG.Tweening;

public class GoalSign : MonoBehaviour
{
    [SerializeField] float leftPos;
    [SerializeField] float rightPos;
    [SerializeField] float totalTravelTime;
    [SerializeField] GameObject goalText;

    public void TriggerGoalSign()
    {
        goalText.transform.position = new Vector3(rightPos, 0, 0);
        goalText.transform.DOMove(new Vector3(leftPos, 0, 0), totalTravelTime).SetEase(Ease.Linear)
            .OnComplete(() => goalText.transform.position = new Vector3(rightPos, 0, 0));
    }

}
