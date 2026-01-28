using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentKickHeight = startingKickHeight;
        if(!anim)
            anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //Grounding and gravity logic
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundStick;
            isHeaderAcive = false;
        }
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);

        // Prevent faster diagonal speed
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        // Apply movement
        Vector3 velocity = (moveDir * moveSpeed) + (Vector3.up * verticalVelocity);
        anim.SetFloat("Velocity", Mathf.Abs(velocity.x) + Mathf.Abs(velocity.z));
        anim.SetBool("onGround", controller.isGrounded);

        if (controller.enabled)
            controller.Move(velocity * Time.deltaTime);

        // Face movement direction
        if (rotateToMovement && moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        
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
            Debug.Log("[JUMP] Blocked – not grounded");
        }

        HeaderBall();
    }
    public void OnMove(Vector2 input)
    {
        //Debug.Log("Moving: " + input);
        moveInput = input;
        //throw new System.NotImplementedException();
    }
    void ResetKickVisual()
    {
        kickCollider.SetActive(false);
    }
    public void OnKick()
    {
        if (Time.time < nextKickTime) return;
        nextKickTime = Time.time + kickCooldown;
        kickCollider.SetActive(true);
        Invoke("ResetKickVisual", 0.3f);
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
        if (kickDirection.sqrMagnitude < 0.0001f)
            kickDirection = transform.forward;

        kickDirection.Normalize();

        targetBall.linearVelocity = Vector3.zero;
        targetBall.AddForce(kickDirection * kickForce, ForceMode.Impulse);
        targetBall.AddForce(new Vector3(0, currentKickHeight, 0), ForceMode.Impulse);
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
    #endregion

    public void LockPlayerMove()
    {
        GetComponent<CharacterController>().enabled = false;
    }
    public void UnlockPlayerMove()
    {
        GetComponent<CharacterController>().enabled = true;
    }

}
