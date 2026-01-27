using UnityEngine;
using TMPro;

public class GoalRaycastTrigger : MonoBehaviour
{
    public enum LeftOrRight
    {
        Left,
        Right
    }

    [Header("Goal Side")]
    [SerializeField] private LeftOrRight goalSide;

    [Header("Raycast Settings")]
    [SerializeField] private int rayCount = 3;
    [SerializeField] private float raySpacing = 0.5f;
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private LayerMask detectionLayers;

    [Header("Score Settings")]
    [SerializeField] private int maxGoals = 3;

    [Header("Score Text")]
    [SerializeField] private TextMeshPro leftScoreText;
    [SerializeField] private TextMeshPro rightScoreText;

    [Header("Match Over UI")]
    [SerializeField] private TextMeshPro matchOverText;
    [SerializeField] private TextMeshPro leftWinnerText;
    [SerializeField] private TextMeshPro rightWinnerText;

    private bool ballCurrentlyInGoal = false;
    private bool matchOver = false;

    private void Update()
    {
        if (matchOver) return;

        CastGoalRays();
    }

    private void CastGoalRays()
    {
        bool ballDetectedThisFrame = false;

        for (int i = 0; i < rayCount; i++)
        {
            Vector3 offset = transform.forward * (i - (rayCount - 1) / 2f) * raySpacing;
            Vector3 origin = transform.position + offset;

            if (Physics.Raycast(origin, Vector3.up, out RaycastHit hit, rayDistance, detectionLayers))
            {
                Debug.DrawRay(origin, Vector3.up * rayDistance, Color.green);

                if (hit.collider.CompareTag("Ball"))
                {
                    ballDetectedThisFrame = true;

                    if (!ballCurrentlyInGoal)
                    {
                        Debug.Log($"GOAL scored on {goalSide} side!");
                        RegisterGoal();
                    }

                    break;
                }
            }
            else
            {
                Debug.DrawRay(origin, Vector3.up * rayDistance, Color.red);
            }
        }

        // Reset once the ball leaves the goal area
        ballCurrentlyInGoal = ballDetectedThisFrame;
    }

    private void RegisterGoal()
    {
        ballCurrentlyInGoal = true;

        int newScore;

        if (goalSide == LeftOrRight.Left)
        {
            newScore = IncrementScore(leftScoreText);
            CheckForMatchOver(newScore, LeftOrRight.Left);
        }
        else
        {
            newScore = IncrementScore(rightScoreText);
            CheckForMatchOver(newScore, LeftOrRight.Right);
        }
    }

    private int IncrementScore(TextMeshPro scoreText)
    {
        if (scoreText == null) return 0;

        int currentScore = int.Parse(scoreText.text);
        currentScore++;
        scoreText.text = currentScore.ToString();

        return currentScore;
    }

    private void CheckForMatchOver(int score, LeftOrRight scoringSide)
    {
        if (score < maxGoals) return;

        matchOver = true;

        Debug.Log("MATCH OVER");

        if (matchOverText != null)
            matchOverText.text = "MATCH OVER";

        if (scoringSide == LeftOrRight.Left && leftWinnerText != null)
            leftWinnerText.text = "LEFT TEAM WINS!";

        if (scoringSide == LeftOrRight.Right && rightWinnerText != null)
            rightWinnerText.text = "RIGHT TEAM WINS!";
    }

    /* =======================
     * Gizmos (Scene View)
     * ======================= */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        for (int i = 0; i < rayCount; i++)
        {
            Vector3 offset = transform.forward * (i - (rayCount - 1) / 2f) * raySpacing;
            Vector3 origin = transform.position + offset;
            Vector3 end = origin + Vector3.up * rayDistance;

            Gizmos.DrawLine(origin, end);
            Gizmos.DrawSphere(end, 0.05f);
        }
    }
}
