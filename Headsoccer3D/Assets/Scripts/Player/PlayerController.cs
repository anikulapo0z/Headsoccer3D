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
    [SerializeField] private Collider kickTrigger;
    [SerializeField] private float kickCooldown = 0.5f;
    private bool kickUsesFacingDirection = true;
    [Header("Charge Kick")]
    [SerializeField] private bool useChargeKick = true;
    [SerializeField] private float chargeTime2 = 0.5f;
    [SerializeField] private float chargeTime3 = 1.5f;

    [SerializeField] private float kickMult1 = 1f;
    [SerializeField] private float kickMult2 = 2f;
    [SerializeField] private float kickMult3 = 4f;
    [SerializeField] private float tapTime = 0.1f;
    private bool kickHeld;
    private float kickHoldTime;
    private int kickChargeLevel = 1;

    [Header("Jumping Settings")]
    [SerializeField] private float jumpVelocity = 8f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float groundStick = -2f;

    [Header("Heading Settings")]
    [SerializeField] private Collider headTrigger;
    [SerializeField] private float headingForce = 5f;
    [SerializeField] private float headCooldown = 0.5f;

    [Header("Animator")]
    [SerializeField] private Animator anim;
    [SerializeField] private Animator kickchargeAnim;



    private CharacterController controller;
    private Vector2 moveInput;

    private float verticalVelocity;
    private float nextKickTime = 0f;
    private float nextHeadTime = 0f;
    private readonly HashSet<Rigidbody> ballsInHeadRange = new HashSet<Rigidbody>();
    private readonly HashSet<Rigidbody> ballsInKickRange = new HashSet<Rigidbody>();


    // owen vars
    bool isPlayerLocked = false;
    public Vector3 startingPos;
    [SerializeField, Range(0f, 1f)] float ballVelocityPercent;
    [SerializeField, Range(0f, 1f)] float playerVelocityPercent;

    [SerializeField] bool isHeaderAcive = false;
    [SerializeField] GameObject kickCollider;
    Material kickDisplayMat;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float staminaDrainRate = 1f;
    [SerializeField] private float staminaRegenRate = 0.8f;
    [SerializeField] private float staminaRegenDelay = 2f;
    [SerializeField] private Slider staminaBar;

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
            isHeaderAcive = false;
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
        Vector3 velocity = (moveDir * moveSpeed) + (Vector3.up * verticalVelocity);
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
        if (useChargeKick && kickHeld)
        {
            kickHoldTime += Time.fixedDeltaTime;

            if (kickHoldTime >= chargeTime3) kickChargeLevel = 3;
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
            isHeaderAcive = true;

            verticalVelocity = jumpVelocity;
            Debug.Log($"[JUMP] APPLY jumpVelocity = {jumpVelocity}");
        }
        else
        {
            Debug.Log("[JUMP] Blocked � not grounded");
        }

        HeaderBall();
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

            kickHoldTime = 0f;
            kickChargeLevel = 1;
        }
    }
    void HeaderBall()
    {
        foreach (var t in ballsInHeadRange)
        {
            Debug.Log("after func call: " + t.name);
        }


        if (!isHeaderAcive) return;
        if (ballsInHeadRange.Count == 0) return;

        if (Time.time < nextHeadTime) return;
        nextHeadTime = Time.time + headCooldown;


        foreach (var t in ballsInHeadRange)
        {
            Debug.Log(t.name);
        }
        Debug.Log("hgjkhgkj");

        //Rigidbody ball = GetClosest(ballsInHeadRange);
        Rigidbody ball = ballsInHeadRange.FirstOrDefault();


        if (ball == null) return;
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaa");


        Vector3 startingVel = ball.linearVelocity;
        Vector3 newVel = (startingVel * ballVelocityPercent) + (controller.velocity * playerVelocityPercent);
        newVel.y = 0f;


        ball.linearVelocity = Vector3.zero;
        ball.AddForce((Vector3.up * headingForce) + newVel, ForceMode.Impulse);


    }


    #region Kicking Logic
    private Rigidbody GetClosest(HashSet<Rigidbody> set)
    {
        Rigidbody best = null;
        float bestSqr = float.PositiveInfinity;

        foreach (var rb in set)
        {
            if (rb == null) continue;
            float sqr = (rb.worldCenterOfMass - transform.position).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = rb;
            }
        }
        return best;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // Which trigger fired?
        if (kickTrigger.bounds.Intersects(other.bounds))
            ballsInKickRange.Add(rb);

        if (headTrigger.bounds.Intersects(other.bounds))
        {
            Debug.Log("adding ball to head range");
            ballsInHeadRange.Add(rb);
            foreach (var t in ballsInHeadRange)
            {
                Debug.Log("after adding to list: " + t.name);
            }
            HeaderBall();

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        ballsInKickRange.Remove(rb);
        ballsInHeadRange.Remove(rb);
    }
    void ChargeKick(int level)
    {
        if (Time.time < nextKickTime) return;
        nextKickTime = Time.time + kickCooldown;
        kickCollider.SetActive(true);
        kickDisplayMat.SetFloat("_ScrollValue", 0f);
        StartCoroutine(KickVisualAndReset(0.3f));

        Rigidbody targetBall = GetClosest(ballsInKickRange);
        if (targetBall == null) return;

        Vector3 kickDirection;

        if (kickUsesFacingDirection)
        {
            kickDirection = transform.forward;
        }
        else
        {
            kickDirection = (targetBall.worldCenterOfMass - transform.position);
        }

        kickDirection.y = 0f;
        if (kickDirection.sqrMagnitude < 0.01f)
            kickDirection = transform.forward;

        kickDirection.Normalize();

        float mult = kickMult1;
        if (level == 2) mult = kickMult2;
        else if(level == 3) mult = kickMult3;
        float finalForce = kickForce * mult;

        SoccerBall ball = targetBall.GetComponent<SoccerBall>();
        if (ball != null)
        {
            ball.LaunchAtDirection(kickDirection + (Vector3.up * currentKickHeight / 0.3334f * level), finalForce);
        }
        //targetBall.AddForce(new Vector3(0, currentKickHeight, 0), ForceMode.Impulse);
        Debug.Log($"KICK! Level: {level} | Force: {finalForce} | hold: {kickHoldTime:F2}S");
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
            ApplyKnockback(hitDirection * 3f);
        }
        else
        {
            ApplyKnockback(hitDirection * 8f);
        }
    }
    private void ApplyKnockback(Vector3 force)
    {
        // Add to verticalVelocity? No.
        // Instead temporarily modify movement direction.

        controller.Move(force * Time.fixedDeltaTime);
    }
}
