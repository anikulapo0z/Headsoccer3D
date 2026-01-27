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

    void FixedUpdate()
    {
        RaycastHit hit;

        ballPositionIndicator.transform.rotation = Quaternion.Euler(
            90f,
            transform.eulerAngles.y + Time.time * positionIndicatorSpeed,
            0f
            );
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 100, layerToShowBallPositionOn))
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
