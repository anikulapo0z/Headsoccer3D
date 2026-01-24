using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPlayerControllable
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 12f;

    [SerializeField] private bool rotateToMovement = true;

    private CharacterController controller;
    private Vector2 moveInput;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);

        // Prevent faster diagonal speed
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        // Apply movement
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // Face movement direction
        if (rotateToMovement && moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }

    // ===== IPlayerControllable =====
    public void OnMove(Vector2 input)
    {
        moveInput = input;
        Debug.Log($"P Move: {moveInput}"); // optional
    }

    public void OnJump() { }     // handled elsewhere
    public void OnKick() { }
    public void OnAbility() { }
    public void OnConfirm() { }
    public void OnCancel() { }
    public void OnJoin() { }

    void OnDisable()
    {
        moveInput = Vector2.zero;
    }

}
