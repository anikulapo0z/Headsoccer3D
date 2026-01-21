using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("1 = WASD, 2 = Arrow Keys")]
    public int playerIndex = 1;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float turnSpeed = 12f;

    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 input = GetMoveInput();
        Vector3 moveDir = new Vector3(input.x, 0f, input.y);

        // Prevent faster diagonal movement
        if (moveDir.magnitude > 1f)
            moveDir.Normalize();

        // Move
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // Face movement direction
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                turnSpeed * Time.deltaTime
            );
        }
    }

    Vector2 GetMoveInput()
    {
        float x = 0f;
        float y = 0f;

        if (playerIndex == 1)
        {
            if (Input.GetKey(KeyCode.A)) x = -1;
            if (Input.GetKey(KeyCode.D)) x = 1;
            if (Input.GetKey(KeyCode.S)) y = -1;
            if (Input.GetKey(KeyCode.W)) y = 1;
        }
        else // Player 2
        {
            
            if (Input.GetKey(KeyCode.LeftArrow)) x = -1;
            if (Input.GetKey(KeyCode.RightArrow)) x = 1;
            if (Input.GetKey(KeyCode.DownArrow)) y = -1;
            if (Input.GetKey(KeyCode.UpArrow)) y = 1;

            
        }
        return new Vector2(x, y);
    }
}
