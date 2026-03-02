using UnityEngine;

public class MultiBall : MonoBehaviour
{
    float upAmount;
    float outAmount;
    int ballAmount;
    GameObject ball;

    public void UseAbility()
    {
        float angleStep = 360f / ballAmount;

        for (int i = 0; i < ballAmount; i++)
        {
            float angle = angleStep * i;

            float rad = angle * Mathf.Deg2Rad;
            Vector3 outwardDir = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)).normalized;

            GameObject spawnedBall = Instantiate(ball, transform.position + new Vector3(0f, 2f, 0f), Quaternion.identity);

            Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 force = (outwardDir * outAmount) + (Vector3.up * upAmount);
                rb.AddForce(force, ForceMode.Impulse);
            }
            GameSceneManager.Instance.fakeballList.Add(spawnedBall);
            //GameSceneManager.Instance.fakeballList.Add(spawnedBall.GetComponent<BallController>().ballPositionIndicator);
        }
        GetComponent<PlayerAbility>().ResetAbilityUse();
    }
    public void SetVars(float upA, float outA, int amount, GameObject ballToSpawn)
    {
        upAmount = upA;
        outAmount = outA;
        ballAmount = amount;
        ball = ballToSpawn;
    }
}
