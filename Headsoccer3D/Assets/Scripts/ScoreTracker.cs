using TMPro;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] int leftScore = 0;
    [SerializeField] int rightScore = 0;
    [SerializeField] TMP_Text leftScoreText;
    [SerializeField] TMP_Text rightScoreText;

    [SerializeField] GameSceneManager gameSceneManager;



    public void PointForLeft()
    {
        leftScore++;
        leftScoreText.text = leftScore.ToString();
        gameSceneManager.ResetBall();

    }
    public void PointForRight()
    {
        rightScore++;
        rightScoreText.text = rightScore.ToString();
        gameSceneManager.ResetBall();
    }


}
