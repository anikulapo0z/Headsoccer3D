using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerControllable
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

    // Replace because we are using Input System 
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
    public void OnAbility()
    {
        //throw new System.NotImplementedException();
    }

    public void OnCancel()
    {
        //throw new System.NotImplementedException();
    }

    public void OnConfirm()
    {
        //throw new System.NotImplementedException();
    }

    public void OnJoin()
    {
        //throw new System.NotImplementedException();
    }

    public void OnJump()
    {
        //throw new System.NotImplementedException();
    }

    public void OnKick()
    {
        //throw new System.NotImplementedException();
    }

    public void OnMove(Vector2 input)
    {
        Debug.Log("Moving: " + input);
        moveInput = input;
        //throw new System.NotImplementedException();
    }
}
