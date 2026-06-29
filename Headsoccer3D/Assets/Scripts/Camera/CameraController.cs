using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

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

    [Space]
    [Header("For Debugging")]
    [Tooltip("How long it moves for")]
    public float shakeDuration;
    [Tooltip("How far it moves")]
    public float shakeStrength;
    [Tooltip("How fast it moves")]
    public int shakeVibrato;
    public bool shakeActive = false;

    [Space]
    [Header("Bus Map Framing")]
    [SerializeField] bool isBusMap = false;
    [SerializeField] List<Transform> targets;

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
        Instance = this;
        cam = GetComponent<Camera>();
        startRotation = transform.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            ShakeCamera(shakeDuration, shakeStrength, shakeVibrato);
    }

    void FixedUpdate()
    {
        if (!isBusMap)
        {
            HandleRotation();
            return;
        }

        if (targets == null || targets.Count < 2) return;

        // Compute bounds across all targets
        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (Transform t in targets)
        {
            if (t != null)
                bounds.Encapsulate(t.position);
        }

        Vector3 midpoint = bounds.center;

        float horizontalDist = Mathf.Sqrt(
            bounds.size.x * bounds.size.x +
            bounds.size.z * bounds.size.z
        );

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
        if (target == null) return;
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

    public void ShakeCamera(float duration = 0.1f, float strength = 0.01f, int vibrato = 1)
    {
        if (!cameraPivot || shakeActive) return;

        shakeTween?.Kill();

        cameraPivot.localPosition = Vector3.zero;

        shakeActive = true;
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
            shakeActive = false;
        });
    }

}


