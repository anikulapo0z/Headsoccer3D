using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HowToPlayerCharacterController : MonoBehaviour, IPlayerControllable
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Jumping Settings")]
    [SerializeField] private float jumpVelocity = 8f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float groundStick = -2f;

    [Header("Kicking Settings")]
    [SerializeField] private float kickCooldown = 0.5f;
    [SerializeField] private float kickActiveTime = 0.3f;

    [Header("Heading Settings")]
    [SerializeField] private float headActiveTime = 0.5f;

    [Header("Animator")]
    [SerializeField] private Animator anim;
    //[SerializeField] private Animator kickchargeAnim;

    private CharacterController controller;
    private Vector2 moveInput;
    public float verticalVelocity;
    private float nextKickTime = 0f;

    Vector2 storedInput;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!anim)
            anim = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = groundStick;

        verticalVelocity += gravity * Time.fixedDeltaTime;

        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        Vector3 velocity = Vector3.up * verticalVelocity;
        if (controller.enabled)
            controller.Move(velocity * Time.fixedDeltaTime);

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.fixedDeltaTime);
        }

        anim.SetFloat("Velocity", Mathf.Abs(moveInput.magnitude));
        anim.SetBool("onGround", controller.isGrounded);
    }

    public void OnMove(Vector2 input)
    {
        moveInput = input;
        //anim.SetFloat("Velocity", 10f);
    }

    public void OnJump()
    {
        Debug.Log("sdfsdfgdfgdfg");
        if (controller.isGrounded)
            verticalVelocity = jumpVelocity;

        //headTrigger.TurnOnCollider();
        //StartCoroutine(DisableHeadAfterTime());
    }

    public void OnKick(bool held)
    {

        if (!held)
        {
            if (Time.time < nextKickTime) return;
            nextKickTime = Time.time + kickCooldown;

            anim.SetTrigger("Kick");
            //kickTrigger.TurnOnCollider();
            //StartCoroutine(DisableKickAfterTime());
        }
    }


    public void OnAbility() { }
    public void OnCancel() { }
    public void OnConfirm() { }
    public void OnJoin() { }
    public void OnSprint(bool held) { }
}