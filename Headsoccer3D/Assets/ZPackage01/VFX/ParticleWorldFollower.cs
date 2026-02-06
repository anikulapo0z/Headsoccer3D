
using UnityEngine;

public class ParticleWorldFollower : MonoBehaviour
{
    [Tooltip("Transform to follow for distance-based emission")]
    [SerializeField] private Transform followTarget;

    private Vector3 lastPosition;

    private void Start()
    {
        if (followTarget == null)
        {
            Debug.LogError("ParticleWorldFollower: No follow target assigned.");
            enabled = false;
            return;
        }

        transform.position = followTarget.position;
        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        transform.position = followTarget.position;
        lastPosition = transform.position;
    }
}
