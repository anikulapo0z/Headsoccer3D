using TMPro;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] int leftScore = 0;
    [SerializeField] int rightScore = 0;
    [SerializeField] TMP_Text leftScoreText;
    [SerializeField] TMP_Text rightScoreText;

    [SerializeField] GameSceneManager gameSceneManager;
    public bool canScore = false;


    public void PointForLeft()
    {
        if (!canScore)
            return;
        leftScore++;
        leftScoreText.text = leftScore.ToString();
        gameSceneManager.GoalScored();

    }
    public void PointForRight()
    {
        if (!canScore)
            return;
        rightScore++;
        rightScoreText.text = rightScore.ToString();
        gameSceneManager.GoalScored();
    }
}
