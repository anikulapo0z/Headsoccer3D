using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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

    public int playerIndex;

    //[SerializeField] HowToPlayHighlights jumpHighlight;
    //[SerializeField] HowToPlayHighlights kickHighlight;
    //[SerializeField] HowToPlayHighlights moveHighlight;
    //[SerializeField] HowToPlayHighlights abilityHighlight;

    [Space]
    [SerializeField] GameObject[] objectsToTurnBackOn;
    //[SerializeField] Transform cursorHolder;




    void Awake()
    {
        if(!controller)
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
        bool held;

        if(input.magnitude == 0f)
            held = false;
        else
            held = true;
        MenuManager.Instance.moveHighlight.SetHighlight(held, false, input.x, input.y);

        moveInput = input;
        //anim.SetFloat("Velocity", 10f);
    }

    public void OnJump()
    {
        MenuManager.Instance.jumpHighlight1.SetHighlight(true, true);
        MenuManager.Instance.jumpHighlight2.SetHighlight(true, true);

        //Debug.Log("sdfsdfgdfgdfg");
        if (controller.isGrounded)
            verticalVelocity = jumpVelocity;

        //headTrigger.TurnOnCollider();
        //StartCoroutine(DisableHeadAfterTime());
    }

    public void OnKick(bool held)
    {
        MenuManager.Instance.kickHighlight.SetHighlight(held, false);
        if (!held)
        {
            if (Time.time < nextKickTime) return;
            nextKickTime = Time.time + kickCooldown;

            anim.SetTrigger("Kick");
            //kickTrigger.TurnOnCollider();
            //StartCoroutine(DisableKickAfterTime());
        }
    }


    public void OnAbility()
    {
        MenuManager.Instance.abilityHighlight.SetHighlight(true, true);
    }
    public void OnCancel() { }
    public void OnConfirm() { }
    public void OnJoin()
    {
        MenuManager.Instance.CloseHowToPlay();
        MenuManager.Instance.joinManager.playerSlots[playerIndex].GetComponent<PlayerInputController>().RemoveControlledObject(this, false);
        Destroy(gameObject);

    }
    public void OnSprint(bool held)
    {
        MenuManager.Instance.sprintHighlight.SetHighlight(held, false);


    }

    void OnDestroy()
    {
        anim = null;
        controller = null;
    }
}