using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CircularCharacterMover : MonoBehaviour
{
    [Header("Circle Settings")]
    [SerializeField] private float radius = 2f;
    [SerializeField] private float angularSpeed = 90f; // degrees per second

    private CharacterController controller;
    private float currentAngle;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        currentAngle += angularSpeed * Time.deltaTime;

        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 desiredPosition = new Vector3(
            Mathf.Cos(radians) * radius,
            0f,
            Mathf.Sin(radians) * radius
        );

        Vector3 moveDelta = desiredPosition - transform.localPosition;
        controller.Move(moveDelta);
    }
}
