using UnityEngine;

public class BallController : MonoBehaviour
{
    public bool grounded = false;

    [SerializeField] float xz_Drag;
    [SerializeField] float y_Drag;
    [SerializeField] float ground_Drag;
    Rigidbody rb;

    Vector3 t = new Vector3(0, 0, 60);
    [SerializeField] LayerMask layerToShowBallPositionOn;
    [SerializeField] GameObject ballPositionIndicatorPrefab;
    public GameObject ballPositionIndicator;
    [SerializeField] float positionIndicatorSpeed;

    [SerializeField] float predictionTime;
    [SerializeField] Vector3 canvasOffset;
    [SerializeField] float lerpSpeed = 12f;

    //LineRenderer lr;

    Vector3 previousBallPos;
    Vector3 previousHitPos;

    [SerializeField] float fakeBallAliveTime;


    void Awake()
    {
        if(gameObject.name.Contains("Fake"))
            Invoke("DestroyBall", fakeBallAliveTime);
        //ballPositionIndicator = Instantiate(ballPositionIndicatorPrefab, Vector3.zero, Quaternion.identity);
        //lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        previousBallPos = transform.position;
        previousHitPos = transform.position;
    }
    
    Vector3 GetPredictionPosition()
    {
        Vector3 pred = rb.linearVelocity;
        //pred.y = 0f;

        return transform.position + pred * predictionTime * new Vector3(xz_Drag, y_Drag, xz_Drag).magnitude;

    }

    // predicting balls future position
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

        //ballPositionIndicator.transform.rotation = Quaternion.Euler(
            //90f,
            //ballPositionIndicator.transform.eulerAngles.y + positionIndicatorSpeed,
            //0f
            //);

        if(Physics.Raycast(transform.position, Vector3.down, out hit, 100, layerToShowBallPositionOn))
        {
            //ballPositionIndicator.transform.position = hit.point + canvasOffset;
            //lr.SetPosition(0, transform.position);
            //lr.SetPosition(1, hit.point);

            previousBallPos = transform.position;
            previousHitPos = hit.point;

        }

        Vector3 velocity = rb.linearVelocity;


        if (grounded && velocity.sqrMagnitude > 0.001f)
        {
            Vector3 dragForce = -velocity.normalized * ground_Drag * velocity.sqrMagnitude;

            rb.AddForce(dragForce, ForceMode.Force);
        }

        if (velocity.sqrMagnitude > 0.001f)
        {
            Vector3 dragForce = new Vector3(
                -velocity.normalized.x * xz_Drag * velocity.sqrMagnitude,
                -velocity.normalized.y * y_Drag * velocity.sqrMagnitude,
                -velocity.normalized.z * xz_Drag * velocity.sqrMagnitude);
            rb.AddForce(dragForce, ForceMode.Force);
        }

    }

    void DestroyBall()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //Destroy(ballPositionIndicator);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            grounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            grounded = false;

    }

}
