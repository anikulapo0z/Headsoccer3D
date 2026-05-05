using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

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

    [SerializeField] AnimationCurve getHitSpeedCurve;
    [SerializeField] float multi;
    [SerializeField] float reduceKnockBackTime;
    [SerializeField] float reduceKnockBackTimer;
    [SerializeField] float reduceKnockBackAmount;


    [SerializeField] private float tapTime = 0.1f;
    private bool kickHeld;
    private float kickHoldTime;
    public int kickChargeLevel = 1;
    float kickHeldSpeedMultiplier;
    [SerializeField] float kickHeldSpeedMultiplierVal;


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
    Vector2 moveInput;

    public float verticalVelocity;
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

    private Vector3 initialKnockbackVelocity;

    private float currentStamina;
    private float regenTimer;
    private bool isSprinting = false;
    private bool sprintHeld;

    [Header("Kick-Player Impact Thresholds")]
    [SerializeField] private float kickPlayerThreshold1 = 5f;
    [SerializeField] private float kickPlayerThreshold2 = 15f;

    [Space(5)]
    [Header("Particles")]
    [SerializeField] GameObject[] jumpParticles;
    [SerializeField] Transform jumpParticlesParent;
    [SerializeField] GameObject sprintParticles;

    [SerializeField] float playerKnockbackForceMultiplier;
    [SerializeField] float playerKnockbackDurationMultiplier;
    [SerializeField] float maxPlayerKnockbackDuration;


    public bool hasEmpoweredKick = false;
    [HideInInspector] public float empoweredKickStrength;
    [HideInInspector] public float empoweredKickPlayerMultiplier = 1f;

    public float KickPlayerThreshold1 => kickPlayerThreshold1;
    public float KickPlayerThreshold2 => kickPlayerThreshold2;

    [Header("Dribble Settings")]
    [SerializeField] private bool dribbleEnabled = true;
    [SerializeField] private Key dribbleToggleKey = Key.T; // simple keyboard toggle
    [SerializeField] private float dribbleForce = 35f;
    [SerializeField] private Vector3 dribbleOffset = new Vector3(0f, 0f, 0.9f);
    [SerializeField] private float maxDribbleDistance = 5f;
    private SoccerBall possessedBall;

    private PlayerAudioManager audioManager;

    public Vector3 DribbleOffset => dribbleOffset;

    [SerializeField] GameObject crown;


    // get pancake bitch
    [Space]
    [SerializeField] float goToYVal;
    [SerializeField] float flatenedDuration;
    [SerializeField] float currentFlatenedTime;
    [SerializeField] float goToFlatTime;
    [SerializeField] bool isFlat = false;
    [SerializeField] float flatSpeed;


    // original values
    float originalYScale;
    float originalSpeed;


    // ground / parent check
    [SerializeField] LayerMask groundLayer;

    [Space]
    [Header("Base Camera Shake Stats")]
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] int shakeVibrato;
    [Tooltip("Divide shake amount by this value")]
    [SerializeField] float shakeReduceAmount;

    bool playerLocked = false;

    //KageBushin no Jutsu
    public Vector2 MoveInput => moveInput;
    public bool KickHeld => kickHeld;
    public bool SprintHeld => sprintHeld;
    public Vector3 FacingDirection => transform.forward;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        audioManager = GetComponent<PlayerAudioManager>();
        currentKickHeight = startingKickHeight;
        if(!anim)
            anim = GetComponentInChildren<Animator>();

        //kickDisplayMat = kickCollider.GetComponent<Renderer>().material;

        currentStamina = maxStamina;
        originalYScale = transform.localScale.y;
        originalSpeed = moveSpeed;
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

    /*    void Update()
        {
            if (Keyboard.current != null && Keyboard.current[dribbleToggleKey].wasPressedThisFrame)
            {
                dribbleEnabled = !dribbleEnabled;
                Debug.Log($"[DRIBBLE] {name} dribbleEnabled={dribbleEnabled}");
            }

            // VFX TESTING ONLY – comment out before shipping
            if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
            {
                OnJump();
            }

        }*/


    // At the top of your class, add:
    private Vector3 lastPlatformPosition;
    private Transform currentPlatform;
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


        float moveSpeed;
        // Apply movement

        if (kickHeld)
            moveSpeed = this.moveSpeed;
        else
            moveSpeed = isSprinting ? this.moveSpeed * sprintMultiplier : this.moveSpeed;


        moveSpeed *= kickHeldSpeedMultiplier;


        //Apply knockback if active
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;

            float normalizedTime = 1f - (knockbackTimer / knockbackDuration);
            float curveValue = getHitSpeedCurve.Evaluate(normalizedTime);

            knockbackVelocity = initialKnockbackVelocity * curveValue;

            // remove player control while being knocked
            moveDir = Vector3.zero;
        }
        else
        {
            knockbackVelocity = Vector3.zero;
        }

        if (reduceKnockBackTimer > 0f)
        {
            reduceKnockBackTimer -= Time.fixedDeltaTime;
        }



        // Apply movement + knockback
        Vector3 velocity = (moveDir * moveSpeed) + knockbackVelocity + (Vector3.up * verticalVelocity);

        anim.SetFloat("Velocity", Mathf.Abs(velocity.x) + Mathf.Abs(velocity.z));
        anim.SetBool("onGround", controller.isGrounded);

        if (staminaBar)
            staminaBar.value = currentStamina / maxStamina;





        /*Vector3 platformDelta = Vector3.zero;
        if (currentPlatform != null)
        {
            platformDelta = currentPlatform.position - lastPlatformPosition;
            lastPlatformPosition = currentPlatform.position;
        }

        if (controller.enabled)
            controller.Move((velocity * Time.fixedDeltaTime) + platformDelta);*/


        // Face movement direction
        if (rotateToMovement && moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.fixedDeltaTime);
        }

        if (useChargeKick && kickHeld)
        {
            kickHoldTime += Time.deltaTime;

            kickHeldSpeedMultiplier = kickHeldSpeedMultiplierVal;

            if (kickHoldTime >= chargeTime3)
            {
                kickChargeLevel = 3;
            }
            else if (kickHoldTime >= chargeTime2) kickChargeLevel = 2;
            else kickChargeLevel = 1;
        }
        else
            kickHeldSpeedMultiplier = 1f;

        if (dribbleEnabled && possessedBall != null)
        {
            Vector3 anchor = transform.TransformPoint(dribbleOffset);
            //possessedBall.TweenToAnchor(anchor, this);
            possessedBall.MoveTowardAnchor(anchor, this);
        }


        Debug.DrawRay(transform.position, Vector3.down * 100, Color.blue);

        // ground / parent check
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, groundLayer) && controller.isGrounded)
        {
            if (currentPlatform != hit.transform)
            {
                currentPlatform = hit.transform;
                lastPlatformPosition = hit.transform.position;
            }
        }

        Vector3 platformDelta = Vector3.zero;
        if (currentPlatform != null)
        {
            platformDelta = currentPlatform.position - lastPlatformPosition;
            lastPlatformPosition = currentPlatform.position;
        }

        if (controller.enabled)
            controller.Move((velocity * Time.fixedDeltaTime) + platformDelta);


        // flattened by train
        if (isFlat)
        {
            currentFlatenedTime -= Time.deltaTime;

            if(currentFlatenedTime < 0)
            {
                transform.DOScaleY(originalYScale, goToFlatTime);
                this.moveSpeed = originalSpeed;
                isFlat = false;
            }
        }



        //VFX Test
/*        Vector2 testMove = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) testMove.y += 1f;
        if (Keyboard.current.sKey.isPressed) testMove.y -= 1f;
        if (Keyboard.current.aKey.isPressed) testMove.x -= 1f;
        if (Keyboard.current.dKey.isPressed) testMove.x += 1f;
        if (testMove != Vector2.zero) OnMove(testMove);
        else OnMove(Vector2.zero);*/

    }

    public void OnAbility()
    {
        GetComponent<PlayerAbility>().TryTriggerAbility();
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
        if (playerLocked || isFlat)
            return;
        sprintHeld = held;

        if (held)
        {
            var emission = sprintParticles.GetComponent<ParticleSystem>().emission;
            emission.rateOverDistance = new ParticleSystem.MinMaxCurve(2f, 4f);
        }
        else
        {
            var emission = sprintParticles.GetComponent<ParticleSystem>().emission;
            emission.rateOverDistance = new ParticleSystem.MinMaxCurve(0f, 0f);
        }
    }
    public void OnJump()
    {

        if (controller.isGrounded)
        {
            // setting header active
            //isHeaderAcive = true;
            audioManager.PlayJumpSfx();

            verticalVelocity = jumpVelocity;

            foreach (var p in jumpParticles)
            {
                p.SetActive(true);
                p.transform.parent = null; //world space
            }
            //jumpParticles.SetActive(true);
            Invoke("TurnOffJumpParticles", .6f);

            Debug.Log($"jump jumpVelocity = {jumpVelocity}");
        }
        else
        {
            Debug.Log("jump Blocked, not grounded");
        }

        headTrigger.TurnOnCollider();
        StartCoroutine(DisableHeadAfterTime());

    }
    void TurnOffJumpParticles()
    {
        foreach(var p in jumpParticles)
        {
            p.SetActive(false);
            p.transform.parent = jumpParticlesParent;
            p.transform.localPosition = Vector3.zero;   
        }
        //jumpParticles.SetActive(false);
    }

    public void SetFalling()
    {
        anim.SetTrigger("StartFall");
        anim.SetBool("Falling", true);
    }


    public void OnMove(Vector2 input)
    {
        //Debug.LogWarning("Moving: " + input);
        moveInput = input;
        //throw new System.NotImplementedException();
    }
    IEnumerator KickVisualAndReset(float _time)
    {
        float timer = 0;
        do
        {
            //kickDisplayMat.SetFloat("_ScrollValue", timer);
            //Debug.Log(Mathf.Lerp(0f, 0.92f, timer));

            yield return null;

            timer += Time.deltaTime;

        } while (timer < _time);

        //Reset
        kickCollider.SetActive(false);
        //kickDisplayMat.SetFloat("_ScrollValue", 0f);
    }
    public void OnKick(bool held)
    {
        kickchargeAnim.SetBool("charge", held);
        if (!useChargeKick)
        {
            if (held)
            {
                ChargeKick(1);
                
            }
            return;
        }

        if (held)
        {
            anim.SetTrigger("Charge");
            // start charging
            kickHeld = true;
            kickHoldTime = 0f;
            kickChargeLevel = 1;
        }
        else
        {
            // release -> perform kick
            kickHeld = false;
            anim.ResetTrigger("Charge");

            int levelToUse = (kickHoldTime <= tapTime) ? 1 : kickChargeLevel;
            ChargeKick(levelToUse);


        }
        audioManager?.PlayKickSfx();
    }


    public void HitBall(SoccerBall ball)
    {

    }

    public void SetReadyForEndArea()
    {
        GetComponent<PlayerGroundMarker>().DestroyGroundMarker();
    }

    #region Kicking Logic
    void ChargeKick(int level)
    {
        if(Time.time < nextKickTime) return;

        nextKickTime = Time.time + kickCooldown;
        kickTrigger.TurnOnCollider();
        StartCoroutine(DisableKickAfterTime());

        //kickDisplayMat.SetFloat("_ScrollValue", 0f);

        anim.SetTrigger("Kick");

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

        //Debug.LogError(kickChargeLevel);

        //if (!hasEmpoweredKick)
        //{
            ball.LaunchAtDirection(kickDirection + (Vector3.up * currentKickHeight), finalForce);
        //}
        //else
        //{
            //ball.LaunchAtDirection(kickDirection + (Vector3.up * currentKickHeight), finalForce * empoweredKickStrength);
            //GetComponent<PlayerAbility>().ResetAbilityUse();
        //}



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
            mult = kickMult2;
        else if (kickChargeLevel == 3)
            mult = kickMult3;

        float moveBonus = controller.velocity.magnitude * playerVelocityPercent;

        //float empoweredMult = hasEmpoweredKick ? empoweredKickPlayerMultiplier : 1f;

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
        sprintHeld = false;
        playerLocked = true;
        GetComponent<PlayerAbility>().StopEarthquake();
        GetComponent<CharacterController>().enabled = false;
    }
    public void UnlockPlayerMove()
    {
        playerLocked = false;
        GetComponent<CharacterController>().enabled = true;
    }
    public void GetHit(SoccerBall ball, float momentum, Vector3 hitDirection, float threshold1, float threshold2)
    {
        if (momentum < threshold1)
        {
            // gained control
            return;
        }
        else if (momentum < threshold2)
        {
            ApplyKnockback(hitDirection * knockbackForceMultiplier1);
        }
        else
        {
            ApplyKnockback(hitDirection * knockbackForceMultiplier2);
        }
    }


    public void GetHitFromPlayer(float momentum, Vector3 hitDirection)
    {


        Debug.Log(hitDirection);
        float kickForce = momentum * playerKnockbackForceMultiplier;

        float kickDuration = momentum * playerKnockbackDurationMultiplier;
        kickDuration = Mathf.Clamp(kickDuration, knockbackDuration, maxPlayerKnockbackDuration);

        CameraController.Instance.ShakeCamera(shakeDuration * (momentum / shakeReduceAmount), shakeStrength * (momentum / shakeReduceAmount), (int)(shakeVibrato * (momentum / shakeReduceAmount)));
        ApplyPlayerKickKnockback(hitDirection * kickForce, kickDuration);
    }

    private void ApplyPlayerKickKnockback(Vector3 force, float duration)
    {
        if (reduceKnockBackTimer > 0f)
        {
            force *= reduceKnockBackAmount;
        }
        verticalVelocity = force.y;

        initialKnockbackVelocity = force;
        knockbackVelocity = force;

        knockbackDuration = duration;
        knockbackTimer = duration;

        reduceKnockBackTimer = reduceKnockBackTime;
    }

    private void ApplyKnockback(Vector3 force)
    {
        if (reduceKnockBackTimer > 0f)
        {
            force *= reduceKnockBackAmount;
        }

        initialKnockbackVelocity = force;
        knockbackVelocity = force;

        knockbackTimer = knockbackDuration;

        // start reduction window
        reduceKnockBackTimer = reduceKnockBackTime;
    }
    public void OnGainedPossession(SoccerBall ball)
    {
        possessedBall = ball;
        Debug.Log($"[PLAYER] {name} OnGainedPossession ball={ball.name}");
    }

    public void OnLostPossession(SoccerBall ball)
    {
        if (possessedBall == ball) possessedBall = null;
        Debug.Log($"[PLAYER] {name} OnLostPossession ball={ball.name}");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = dribbleEnabled ? Color.green : Color.red;
        Vector3 anchor = transform.TransformPoint(dribbleOffset);
        Gizmos.DrawSphere(anchor, 0.12f);

        if (possessedBall != null)
            Gizmos.DrawLine(possessedBall.transform.position, anchor);
    }

    public void SetWin()
    {
        crown.SetActive(true);
    }


    public void GetFlattened()
    {
        sprintHeld = false;
        isFlat = true;
        transform.DOScaleY(goToYVal, goToFlatTime);
        moveSpeed = flatSpeed;
        currentFlatenedTime = flatenedDuration;

    }
}
