using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ScoreTracker_FFA : MonoBehaviour
{
    [SerializeField] int goal1Score = 0;
    [SerializeField] int goal2Score = 0;
    [SerializeField] int goal3Score = 0;
    [SerializeField] int goal4Score = 0;

    [SerializeField] TMP_Text goal1ScoreText;
    [SerializeField] TMP_Text goal2ScoreText;
    [SerializeField] TMP_Text goal3ScoreText;
    [SerializeField] TMP_Text goal4ScoreText;

    [SerializeField] GameSceneManager gameSceneManager;
    public bool canScore = false;

    [SerializeField] List<GameObject> goal1Particles = new List<GameObject>();
    [SerializeField] List<GameObject> goal2Particles = new List<GameObject>();
    [SerializeField] List<GameObject> goal3Particles = new List<GameObject>();
    [SerializeField] List<GameObject> goal4Particles = new List<GameObject>();
    
    [SerializeField] float timeUntilTurnParticlesOff;

    [SerializeField] CameraController cameraController;

    [Space(10)]
    [Header("Camera Shake Values")]
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] int shakeVibrato;

    [Space(5)]
    [SerializeField] float littleShakeMultiplier;
    [SerializeField] float bigShakeMultiplier;
    [SerializeField] float massiveShakeMultiplier;

    [Space(5)]
    [SerializeField] float littleShakeSpeed;
    [SerializeField] float bigShakeSpeed;
    [SerializeField] float massiveShakeSpeed;


    [Space(10)]
    [Header("Field Specific Properties")]
    [Space(3)]
    [Header("Bus Map")]
    [SerializeField] GoalSign goalSign;


    public void ScoreGoal(int index)
    {
        if (!canScore)
            return;

        switch (index)
        {
            case (1):
                goal1Score++;
                goal1ScoreText.text = goal1Score.ToString();
                gameSceneManager.GoalScored('r');


                foreach (var p in goal1Particles)
                {
                    Debug.LogWarning(p.name);
                    p.SetActive(true);
                }
                Invoke("TurnOffParticles", timeUntilTurnParticlesOff);
                break;

            case (2):
                goal2Score++;
                goal2ScoreText.text = goal2Score.ToString();
                gameSceneManager.GoalScored('l');

                var particles2 = goal2Particles;

                foreach (var p in particles2)
                {
                    p.SetActive(true);
                }
                Invoke("TurnOffParticles", timeUntilTurnParticlesOff);

                break;

            case (3):
                goal3Score++;
                goal3ScoreText.text = goal3Score.ToString();
                gameSceneManager.GoalScored('r');

                var particles3 = goal3Particles;

                foreach (var p in particles3)
                {
                    p.SetActive(true);
                }
                Invoke("TurnOffParticles", timeUntilTurnParticlesOff);

                break;

            case (4):
                goal4Score++;
                goal4ScoreText.text = goal4Score.ToString();
                gameSceneManager.GoalScored('l');

                var particles4 = goal4Particles;

                foreach (var p in particles4)
                {
                    p.SetActive(true);
                }
                Invoke("TurnOffParticles", timeUntilTurnParticlesOff);

                break;

        }
    }

    public (List<int>, int) WhoTeamWon()
    {
        int maxScore = Mathf.Max(goal1Score, goal2Score, goal3Score, goal4Score);
        int[] scores = { goal1Score, goal2Score, goal3Score, goal4Score };

        List<int> winningIndexes = new List<int>();

        for (int i = 0; i < scores.Length; i++)
        {
            if (scores[i] == maxScore)
            {
                winningIndexes.Add(i);
            }
        }

        return (winningIndexes, maxScore);

    }


    void TurnOffParticles()
    {
        foreach (var t in goal1Particles)
        {
            t.gameObject.SetActive(false);
        }
        foreach (var t in goal2Particles)
        {
            t.gameObject.SetActive(false);
        }
        foreach (var t in goal3Particles)
        {
            t.gameObject.SetActive(false);
        }
        foreach (var t in goal4Particles)
        {
            t.gameObject.SetActive(false);
        }
    }

    public void ShakeCamera(float ballSpeed)
    {
        if (ballSpeed > massiveShakeSpeed)
        {
            cameraController.ShakeCamera(
                shakeDuration * massiveShakeMultiplier,
                shakeStrength * massiveShakeMultiplier,
                (int)(shakeVibrato * massiveShakeMultiplier));
            return;
        }
        if (ballSpeed > bigShakeSpeed)
        {
            cameraController.ShakeCamera(
                shakeDuration * bigShakeMultiplier,
                shakeStrength * bigShakeMultiplier,
                (int)(shakeVibrato * bigShakeMultiplier));
            return;
        }
        if (ballSpeed > littleShakeSpeed)
        {
            cameraController.ShakeCamera(
                shakeDuration * littleShakeMultiplier,
                shakeStrength * littleShakeMultiplier,
                (int)(shakeVibrato * littleShakeMultiplier));
            return;
        }
    }

}
