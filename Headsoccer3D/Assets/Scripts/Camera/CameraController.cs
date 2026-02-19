using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

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


    void Start()
    {
        startRotation = transform.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            ShakeCamera(shakeDuration, shakeStrength, shakeVibrato);

    }

    void FixedUpdate()
    {
        if (!target) return;
        HandleRotation();
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
