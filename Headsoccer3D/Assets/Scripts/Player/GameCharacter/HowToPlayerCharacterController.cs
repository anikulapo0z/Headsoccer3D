using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
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
    [SerializeField] private Animator kickchargeAnim;


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


    [SerializeField] ParticleSystem sprint;
    //[SerializeField] HowToPlayHighlights jumpHighlight;
    //[SerializeField] HowToPlayHighlights kickHighlight;
    //[SerializeField] HowToPlayHighlights moveHighlight;
    //[SerializeField] HowToPlayHighlights abilityHighlight;

    [Space]
    [SerializeField] GameObject[] objectsToTurnBackOn;
    //[SerializeField] Transform cursorHolder;


    [Space]
    [Header("kick + jump")]
    [SerializeField] float combineButtonPressTime;
    float pressedKickTime = 0f;
    float pressedJumpTime = 0f;


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
        anim.SetInteger("Taunt", (10));

        bool held;

        if(input.magnitude == 0f)
            held = false;
        else
            held = true;
        MenuManager.Instance.moveHighlight_controller.SetHighlight(held, false, input.x, input.y);
        MenuManager.Instance.moveHighlight_arcade.SetHighlight(held, false, input.x, input.y);

        moveInput = input;
        //anim.SetFloat("Velocity", 10f);
    }

    public void OnJump()
    {
        anim.SetInteger("Taunt", (10));
        MenuManager.Instance.jumpHighlight1_controller.SetHighlight(true, true);
        MenuManager.Instance.jumpHighlight2_controller.SetHighlight(true, true);
        MenuManager.Instance.jumpHighlight_exit_controller.SetHighlight(true, true);

        MenuManager.Instance.jumpHighlight_arcade.SetHighlight(true, true);
        MenuManager.Instance.jumpHighlight_exit_arcade.SetHighlight(true, true);

        //Debug.Log("sdfsdfgdfgdfg");
        if (controller.isGrounded)
            verticalVelocity = jumpVelocity;

        //headTrigger.TurnOnCollider();
        //StartCoroutine(DisableHeadAfterTime());

        pressedJumpTime = Time.time;
        if (Mathf.Abs(pressedJumpTime - pressedKickTime) < combineButtonPressTime) ExitHowToPlay();

    }

    public void OnKick(bool held)
    {
        anim.SetInteger("Taunt", (10));
        MenuManager.Instance.kickHighlight_controller.SetHighlight(held, false);
        MenuManager.Instance.kickHighlight_exit_controller.SetHighlight(held, false);

        MenuManager.Instance.kickHighlight_arcade.SetHighlight(held, false);
        MenuManager.Instance.kickHighlight_exit_arcade.SetHighlight(held, false);

        kickchargeAnim.gameObject.SetActive(held);
        kickchargeAnim.SetBool("charge", held);


        if (held)
        {
            if (Time.time < nextKickTime) return;
            nextKickTime = Time.time + kickCooldown;

            anim.SetTrigger("Charge");

            //kickTrigger.TurnOnCollider();
            //StartCoroutine(DisableKickAfterTime());
        }
        else
        {
            anim.ResetTrigger("Charge");

            anim.SetTrigger("Kick");

        }

        pressedKickTime = Time.time;
        if(Mathf.Abs(pressedJumpTime - pressedKickTime) < combineButtonPressTime) ExitHowToPlay();
    }


    public void OnAbility()
    {
        anim.SetInteger("Taunt", (10));
        MenuManager.Instance.abilityHighlight_controller.SetHighlight(true, true);
        MenuManager.Instance.abilityHighlight_arcade.SetHighlight(true, true);
    }
    public void OnCancel() { }
    public void OnConfirm() { }
    public void OnStart()
    {


    }
    public void OnSprint(bool held)
    {
        anim.SetInteger("Taunt", (10));
        MenuManager.Instance.sprintHighlight_controller.SetHighlight(held, false);
        MenuManager.Instance.sprintHighlight_arcade.SetHighlight(held, false);

        if (held && moveInput.magnitude > 0)
        {
            var emission = sprint.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = 5f;
        }
        else
        {
            var emission = sprint.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = 0;
        }



    }

    public void OnPoseTaunt()
    {
        anim.SetInteger("Taunt", Random.Range(1, 6));
    }
    public void OnTextTaunt() { }


    void ExitHowToPlay()
    {
        anim.SetInteger("Taunt", (10));
        MenuManager.Instance.CloseHowToPlay();
        MenuManager.Instance.joinManager.playerSlots[playerIndex].GetComponent<PlayerInputController>().RemoveControlledObject(this, false);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        anim = null;
        controller = null;
    }
}