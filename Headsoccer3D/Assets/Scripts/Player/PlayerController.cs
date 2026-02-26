using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPlayerControllable
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 12f;

    [SerializeField] private bool rotateToMovement = true;

    [Header("Kicking Settings")]
    [SerializeField] private float kickForce = 10f;
    [SerializeField] private float startingKickHeight = 1f;
    [SerializeField] private float currentKickHeight;
    //[SerializeField] private Collider kickTrigger;
    [SerializeField] private float kickCooldown = 0.5f;
    private bool kickUsesFacingDirection = true;
    [Header("Charge Kick")]
    [SerializeField] private bool useChargeKick = true;
    [SerializeField] private float chargeTime2 = 0.5f;
    [SerializeField] private float chargeTime3 = 1.5f;

    [SerializeField] private float kickMult1 = 1f;
    [SerializeField] private float kickMult2 = 2f;
    [SerializeField] private float kickMult3 = 4f;
    [SerializeField] private float kickHeightMult1;
    [SerializeField] private float kickHeightMult2;
    [SerializeField] private float kickHeightMult3;




    [SerializeField] private float tapTime = 0.1f;
    private bool kickHeld;
    private float kickHoldTime;
    public int kickChargeLevel = 1;

    [Header("Jumping Settings")]
    [SerializeField] private float jumpVelocity = 8f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float groundStick = -2f;

    [Header("Heading Settings")]
    //[SerializeField] private Collider headTrigger;
    [SerializeField] private float headingForce = 5f;
    [SerializeField] private float headerUpForce;
    [SerializeField] private float headCooldown = 0.5f;

    [Header("Animator")]
    [SerializeField] private Animator anim;
    [SerializeField] private Animator kickchargeAnim;



    private CharacterController controller;
    private Vector2 moveInput;

    private float verticalVelocity;
    private float nextKickTime = 0f;
    private float nextHeadTime = 0f;

    // owen vars
    bool isPlayerLocked = false;
    public Vector3 startingPos;
    [SerializeField, Range(0f, 1f)] float ballVelocityPercent;
    [SerializeField] float playerVelocityPercent;

    //[SerializeField] bool isHeaderAcive = false;
    [SerializeField] GameObject kickCollider;
    Material kickDisplayMat;



    // new kick vars
    [SerializeField] PlayerTriggers kickTrigger;
    [SerializeField] PlayerTriggers headTrigger;
    [SerializeField] float kickActiveTime;
    [SerializeField] float headActiveTime;
    [SerializeField] float kickPlayerCooldown = 0.12f;
    private float nextKickPlayerTime = 0f;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float staminaDrainRate = 1f;
    [SerializeField] private float staminaRegenRate = 0.8f;
    [SerializeField] private float staminaRegenDelay = 2f;
    [SerializeField] private Slider staminaBar;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float knockbackDrag = 18f;
    [SerializeField] private float knockbackForceMultiplier1 = 15f;
    [SerializeField] private float knockbackForceMultiplier2 = 30f;
    private Vector3 knockbackVelocity;
    private float knockbackTimer;

    private float currentStamina;
    private float regenTimer;
    private bool isSprinting = false;
    private bool sprintHeld;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentKickHeight = startingKickHeight;
        if(!anim)
            anim = GetComponentInChildren<Animator>();

        kickDisplayMat = kickCollider.GetComponent<Renderer>().material;

        currentStamina = maxStamina;

    }

    public void SetStaminaBar(Slider bar)
    {
        if (bar)
        {
            staminaBar = bar;
            staminaBar.minValue = 0f;
            staminaBar.maxValue = 1f;
            staminaBar.value = 1f;
        }
    }



    void FixedUpdate()
    {
        //Grounding and gravity logic
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundStick;
        }
        verticalVelocity += gravity * Time.fixedDeltaTime;

        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);

        // Prevent faster diagonal speed
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        bool hasMoveInput = moveDir.sqrMagnitude > 0.001f;
        isSprinting = sprintHeld && hasMoveInput && controller.isGrounded && currentStamina > 0.1f;
        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
            regenTimer = staminaRegenDelay;
            if (currentStamina < 0f) currentStamina = 0f;
        }
        else
        {
            if (regenTimer > 0f)
            {
                regenTimer -= Time.deltaTime;
            }
            else
            {
                currentStamina += staminaRegenRate * Time.fixedDeltaTime;
                if (currentStamina > maxStamina) currentStamina = maxStamina;
            }
        }
        // Apply movement
        float moveSpeed = isSprinting ? this.moveSpeed * sprintMultiplier : this.moveSpeed;

        //Apply knockback if active
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            knockbackVelocity = Vector3.Slerp(knockbackVelocity, Vector3.zero, knockbackDrag * Time.fixedDeltaTime);

            // remove player control while being knocked
            moveDir = Vector3.zero;
        }
        else
        {
            knockbackVelocity = Vector3.zero;
        }

        // Apply movement + knockback
        Vector3 velocity = (moveDir * moveSpeed) + knockbackVelocity + (Vector3.up * verticalVelocity);

        anim.SetFloat("Velocity", Mathf.Abs(velocity.x) + Mathf.Abs(velocity.z));
        anim.SetBool("onGround", controller.isGrounded);

        if (staminaBar)
            staminaBar.value = currentStamina / maxStamina;

        if (controller.enabled)
            controller.Move(velocity * Time.fixedDeltaTime);

        // Face movement direction
        if (rotateToMovement && moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.fixedDeltaTime);
        }
        //Debug.LogError(kickHeld);
        if (useChargeKick && kickHeld)
        {
            kickHoldTime += Time.deltaTime;

            if (kickHoldTime >= chargeTime3)
            {
                Debug.LogError("-----------------------");
                kickChargeLevel = 3;
            }
            else if (kickHoldTime >= chargeTime2) kickChargeLevel = 2;
            else kickChargeLevel = 1;
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
    public void OnSprint(bool held)
    {
        sprintHeld = held;
    }
    public void OnJump()
    {
        //throw new System.NotImplementedException();
        Debug.Log
            (
        $"[JUMP] Fired | grounded: {controller.isGrounded} | verticalVelocity before: {verticalVelocity}"
        );

        if (controller.isGrounded)
        {
            // setting header active
            //isHeaderAcive = true;

            verticalVelocity = jumpVelocity;
            Debug.Log($"[JUMP] APPLY jumpVelocity = {jumpVelocity}");
        }
        else
        {
            Debug.Log("[JUMP] Blocked � not grounded");
        }

        headTrigger.TurnOnCollider();
        StartCoroutine(DisableHeadAfterTime());
    }
    public void OnMove(Vector2 input)
    {
        //Debug.Log("Moving: " + input);
        moveInput = input;
        //throw new System.NotImplementedException();
    }
    IEnumerator KickVisualAndReset(float _time)
    {
        float timer = 0;
        do
        {
            kickDisplayMat.SetFloat("_ScrollValue", timer);
            //Debug.Log(Mathf.Lerp(0f, 0.92f, timer));

            yield return null;

            timer += Time.deltaTime;

        } while (timer < _time);

        //Reset
        kickCollider.SetActive(false);
        kickDisplayMat.SetFloat("_ScrollValue", 0f);
    }
    public void OnKick(bool held)
    {
        kickchargeAnim.SetBool("charge", held);
        if (!useChargeKick)
        {
            if (held) ChargeKick(1);
            return;
        }

        if (held)
        {
            // start charging
            kickHeld = true;
            kickHoldTime = 0f;
            kickChargeLevel = 1;
        }
        else
        {
            // release -> perform kick
            kickHeld = false;

            int levelToUse = (kickHoldTime <= tapTime) ? 1 : kickChargeLevel;
            ChargeKick(levelToUse);


        }
    }


    public void HitBall(SoccerBall ball)
    {

    }



    #region Kicking Logic
    void ChargeKick(int level)
    {
        if(Time.time < nextKickTime) return;

        nextKickTime = Time.time + kickCooldown;
        kickTrigger.TurnOnCollider();
        StartCoroutine(DisableKickAfterTime());

        kickDisplayMat.SetFloat("_ScrollValue", 0f);
        StartCoroutine(KickVisualAndReset(0.3f));
        Debug.Log($"KICK! Level: {level} | hold: {kickHoldTime:F2}S");


    }

    private IEnumerator DisableKickAfterTime()
    {
        yield return new WaitForSeconds(kickActiveTime);
        kickHoldTime = 0f;
        kickChargeLevel = 1;
        kickTrigger.TurnOffCollider();
    }
    private IEnumerator DisableHeadAfterTime()
    {
        yield return new WaitForSeconds(headActiveTime);
        headTrigger.TurnOffCollider();
    }

    public void OnKickTrigger(SoccerBall ball)
    {
        Vector3 kickDirection;

        if(kickUsesFacingDirection)
            kickDirection = transform.forward;
        else
            kickDirection = (ball.transform.position - transform.position);

        kickDirection.y = 0f;
        kickDirection.Normalize();

        float mult = kickMult1;
        float currentKickHeight = kickHeightMult1;
        if (kickChargeLevel == 2)
        {
            mult = kickMult2;
            currentKickHeight = kickHeightMult2;
        }
        else if (kickChargeLevel == 3)
        {
            mult = kickMult3;
            currentKickHeight = kickHeightMult3;
        }

        float finalForce = kickForce * mult;

        Debug.LogError(kickChargeLevel);
        ball.LaunchAtDirection(kickDirection + (Vector3.up * currentKickHeight), finalForce);



    }

    public void OnHeadTrigger(SoccerBall ball)
    {
        if (Time.time < nextHeadTime) return;

        nextHeadTime = Time.time + headCooldown;

        Vector3 headerDirection = transform.forward;

        headerDirection += Vector3.up * headerUpForce;

        //headerDirection.Normalize();

        Vector3 movementInfluence = controller.velocity * playerVelocityPercent;
        movementInfluence.y = 0f;

        headerDirection += movementInfluence;
        headerDirection.Normalize();

        float finalForce = headingForce;

        ball.LaunchAtDirection(headerDirection, finalForce);
    }
     public float GetKickPlayerMomentum()
     {
        float mult = kickMult1;
        if (kickChargeLevel == 2)
        {
            mult = kickMult2;
        }
        else if (kickChargeLevel == 3)
        {
            mult = kickMult3;
        }

        float moveBonus = controller.velocity.magnitude * playerVelocityPercent;
        return (kickForce * mult) + moveBonus;
     }
    public bool CanApplyKickPlayerHit()
    {
        if (Time.time < nextKickPlayerTime) return false;
        nextKickPlayerTime = Time.time + kickPlayerCooldown;
        return true;
    }

    #endregion

    public void LockPlayerMove()
    {
        GetComponent<CharacterController>().enabled = false;
    }
    public void UnlockPlayerMove()
    {
        GetComponent<CharacterController>().enabled = true;
    }
    public void GetHit(SoccerBall ball, float momentum, Vector3 hitDirection)
    {
        if (momentum < 5f)
        {
            // gained control
            return;
        }
        else if (momentum < 15f)
        {
            ApplyKnockback(hitDirection * knockbackForceMultiplier1);
        }
        else
        {
            ApplyKnockback(hitDirection * knockbackForceMultiplier2);
        }
    }
    private void ApplyKnockback(Vector3 force)
    {
        // Add to verticalVelocity? No.
        // Instead temporarily modify movement direction.

        //controller.Move(force * Time.fixedDeltaTime);
        knockbackVelocity = force;
        knockbackTimer = knockbackDuration;
    }
}
