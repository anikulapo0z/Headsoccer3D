using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    [SerializeField] Transform cameraPivot;

    [Header("Rotation")]
    [SerializeField] float rotationLerpSpeed = 5f;
    [SerializeField] float maxLeftRightAngle;
    [SerializeField] float maxUpDownAngle;

    Quaternion startRotation;

    [Space(10)]
    [Header("Camera Shake")]
    Tween shakeTween;
    public float shakeDuration;
    public float shakeStrength;
    public int shakeVibrato;


    [Header("bus map framing")]
    [SerializeField] bool isBusMap = false;
    [SerializeField] Transform targetA;
    [SerializeField] Transform targetB;

    [SerializeField] float padding;
    [SerializeField] float speed;
    [SerializeField] float xOffset;
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    [SerializeField] float xOffsetAtMin;
    [SerializeField] float xOffsetAtMax;

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        startRotation = transform.rotation;
    }

    /*    private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
                ShakeCamera(shakeDuration, shakeStrength, shakeVibrato);

        }*/

    void FixedUpdate()
    {
        if (!isBusMap)
        {
            HandleRotation();
            return;
        }

        if (!target) return;

        Vector3 midpoint = (targetA.position + targetB.position) * 0.5f;

        float xSep = Mathf.Abs(targetA.position.x - targetB.position.x);
        float zSep = Mathf.Abs(targetA.position.z - targetB.position.z);

        float horizontalDist = Mathf.Sqrt(xSep * xSep + zSep * zSep);

        float fov = cam.fieldOfView * Mathf.Deg2Rad;
        float aspect = cam.aspect;
        float requiredDist = (horizontalDist * padding) / (2f * Mathf.Tan(fov / 2f) * aspect);

        requiredDist = Mathf.Clamp(requiredDist, minDistance, maxDistance);

        float distT = Mathf.InverseLerp(minDistance, maxDistance, requiredDist);
        xOffset = Mathf.Lerp(xOffsetAtMin, xOffsetAtMax, distT);

        Vector3 goToPosition = new Vector3(midpoint.x + xOffset, transform.position.y, midpoint.z - requiredDist);

        transform.position = Vector3.Lerp(transform.position, goToPosition, Time.deltaTime * speed);
    }


    void HandleRotation()
    {
        Vector3 dirToTarget = target.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dirToTarget);

        Quaternion localRot = Quaternion.Inverse(startRotation) * lookRot;
        Vector3 euler = localRot.eulerAngles;

        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        
        euler.x = Mathf.Clamp(euler.x, -maxUpDownAngle, maxUpDownAngle);
        euler.y = Mathf.Clamp(euler.y, -maxLeftRightAngle, maxLeftRightAngle);
        euler.z = 0f;

        Quaternion clampedRot = startRotation * Quaternion.Euler(euler);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            clampedRot,
            Time.deltaTime * rotationLerpSpeed
        );
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    public void ShakeCamera(
        float duration = -1f,
        float strength = -1f,
        int vibrato = -1
    )
    {
        if (!cameraPivot) return;

        shakeTween?.Kill();

        cameraPivot.localPosition = Vector3.zero;

        shakeTween = cameraPivot.DOShakePosition(
            duration,
            strength,
            vibrato,
            randomness: 90f,
            snapping: false,
            fadeOut: true
        ).OnComplete(() =>
        {
            cameraPivot.localPosition = Vector3.zero;
        });
    }

}


