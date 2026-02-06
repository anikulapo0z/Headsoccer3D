using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] int leftScore = 0;
    [SerializeField] int rightScore = 0;
    [SerializeField] TMP_Text leftScoreText;
    [SerializeField] TMP_Text rightScoreText;

    [SerializeField] GameSceneManager gameSceneManager;
    public bool canScore = false;

    [SerializeField] List<GameObject> leftGoalParticles = new List<GameObject>();
    [SerializeField] List<GameObject> rightGoalParticles = new List<GameObject>();

    [SerializeField] private ParticleSystem testConfett;
    [SerializeField] float timeUntilTurnParticlesOff;

    public void PointForLeft()
    {
        if (!canScore)
            return;
        leftScore++;
        leftScoreText.text = leftScore.ToString();
        gameSceneManager.GoalScored('l');

        Debug.LogWarning(leftGoalParticles.Count);

        //leftGoalParticles[0].Play();
        //testConfett.Play();



        var particles = leftGoalParticles;

        foreach (var p in particles)
        {
            p.SetActive(true);
        }
        Invoke("TurnOffParticles", timeUntilTurnParticlesOff);

    }
    public void PointForRight()
    {
        if (!canScore)
            return;
        rightScore++;
        rightScoreText.text = rightScore.ToString();
        gameSceneManager.GoalScored('r');

        //rightGoalParticles[0].Play();

        //testConfett.Play();



        var particles = rightGoalParticles;

        foreach (var p in particles)
        {
            p.SetActive(true);
        }
        Invoke("TurnOffParticles", timeUntilTurnParticlesOff);
    }
    
    void TurnOffParticles()
    {
        foreach(var t in leftGoalParticles)
        {
            t.gameObject.SetActive(false);
        }
        foreach(var t in rightGoalParticles)
        {
            t.gameObject.SetActive(false);
        }
    }

}
