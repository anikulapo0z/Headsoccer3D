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

        goalSign.TriggerGoalSign();

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

        goalSign.TriggerGoalSign();
    }

    public bool LeftTeamWon()
    {
        if(leftScore > rightScore)
            return true;
        else
            return false;
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
