using System.Xml.Schema;
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

    [SerializeField] ParticleSystem[] leftGoalParticles;
    [SerializeField] ParticleSystem[] rightGoalParticles;


    public void PointForLeft()
    {
        if (!canScore)
            return;
        leftScore++;
        leftScoreText.text = leftScore.ToString();
        gameSceneManager.GoalScored('l');

        Debug.LogWarning(leftGoalParticles.Length);
        foreach (ParticleSystem p in leftGoalParticles)
        {
            Debug.LogWarning(p.name);
            p.Play();
        }
    }
    public void PointForRight()
    {
        if (!canScore)
            return;
        rightScore++;
        rightScoreText.text = rightScore.ToString();
        gameSceneManager.GoalScored('r');

        foreach (ParticleSystem p in rightGoalParticles)
        {
            Debug.LogWarning(p.name);
            p.Play();
        }
    }
}
