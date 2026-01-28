using System.Collections.Generic;
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
    [SerializeField] private Collider feetTrigger;
    [SerializeField] private float headingForce = 5f;
    [SerializeField] private float headCooldown = 0.5f;

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


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentKickHeight = startingKickHeight;
    }

    void Update()
    {
        //Grounding and gravity logic
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = groundStick;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);

        // Prevent faster diagonal speed
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        // Apply movement
        if(controller.enabled)
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
        if (controller.isGrounded)
        {
            verticalVelocity = jumpVelocity;
        }
        if(Time.time < nextHeadTime) return;
        nextHeadTime = Time.time + headCooldown;

        Rigidbody ball = GetClosest(ballsInHeadRange);
        if (ball == null) return;

        ball.linearVelocity = Vector3.zero;
        ball.AddForce(Vector3.up * headingForce, ForceMode.Impulse);
    }
    public void OnMove(Vector2 input)
    {
        //Debug.Log("Moving: " + input);
        moveInput = input;
        //throw new System.NotImplementedException();
    }

    public void OnKick()
    {
        if (Time.time < nextKickTime) return;
        nextKickTime = Time.time + kickCooldown;

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
            ballsInHeadRange.Add(rb);

        if (feetTrigger.bounds.Intersects(other.bounds))
            ballsInHeadRange.Add(rb);
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
