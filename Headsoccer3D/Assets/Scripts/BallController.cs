using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] float airDrag = 0.15f;
    Rigidbody rb;

    Vector3 t = new Vector3(0, 0, 60);
    [SerializeField] LayerMask layerToShowBallPositionOn;
    [SerializeField] GameObject ballPositionIndicatorPrefab;
    [SerializeField] GameObject ballPositionIndicator;
    [SerializeField] float positionIndicatorSpeed;

    [SerializeField] float predictionTime;


    LineRenderer lr;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            rb.AddForce(t, ForceMode.Impulse);
    }


    void Awake()
    {
        ballPositionIndicator = Instantiate(ballPositionIndicatorPrefab, Vector3.zero, Quaternion.identity);
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
    }
    
    Vector3 GetPredictionPosition()
    {
        Vector3 pred = rb.linearVelocity;
        //pred.y = 0f;

        return transform.position + pred * predictionTime * airDrag;

    }

    void OnDrawGizmos()
    {
        Vector3 start = transform.position;
        Vector3 end = GetPredictionPosition();

        Gizmos.color = Color.green;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.1f);
    }



    void FixedUpdate()
    {
        GetPredictionPosition();

        RaycastHit hit;

        ballPositionIndicator.transform.rotation = Quaternion.Euler(
            90f,
            transform.eulerAngles.y + Time.time * positionIndicatorSpeed,
            0f
            );
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 100, layerToShowBallPositionOn))
        {
            ballPositionIndicator.transform.position = hit.point;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, hit.point);
        }
        



        Vector3 velocity = rb.linearVelocity;

        if (velocity.sqrMagnitude > 0.001f)
        {
            Vector3 dragForce = -velocity.normalized * airDrag * velocity.sqrMagnitude;
            rb.AddForce(dragForce, ForceMode.Force);
        }
    }
}
