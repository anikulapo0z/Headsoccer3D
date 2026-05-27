using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SoccerBall : MonoBehaviour
{
    Rigidbody rb;

    //Additions for momentum and thresholds
    [SerializeField] private float threshold1 = 5f;
    [SerializeField] private float threshold2 = 15f;
    [SerializeField] private float threshold3 = 30f;
    [SerializeField] private float thresholdBlend = 1.5f;

    private PlayerController currentActivePlayer;

    CPUEnemy[] CPUPlayers = null;
    bool areThereCPUPlayers = true; //inital val to true is important

    [Header("Dribble and possession")]
    [SerializeField] private float possessionCooldown = 0.15f;
    private float nextPossessionTime = 0f;

    [Header("Dribble Follow")]
    [SerializeField] private float maxTweenDistance = 1.8f;
    [SerializeField] private float holdHeight = 0.0f;
    public PlayerController CurrentPossessor => currentActivePlayer;
    public bool HasPossession(PlayerController p) => currentActivePlayer == p;

    private Collider ballCol;
    private Tweener followTween;

    [SerializeField] private float followDuration = 0.06f; // small = snappy (0.04–0.10)
    [SerializeField] private Ease followEase = Ease.OutQuad;

    private bool isPossessed;

    [Header("Possession Grounding")]
    [SerializeField] private float groundRayHeight = 2.0f;   
    [SerializeField] private float groundRayDistance = 4.0f;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float groundOffset = 0.11f;
    [SerializeField] private bool forceToGroundOnClaim = true;

    [SerializeField] private float maxClaimHeightAboveGround = 0.35f;

    [Header("Audio")]
    [SerializeField] private AudioSource ballHitSoftSfx;
    [SerializeField] private AudioSource ballHitHardSfx;

    Vector3 lastTweenPosition;


    public float dribbleSpeed = 14f;
    public float ballRadius = 0.11f;

    [Header("Ball Trail")]
    [SerializeField] Transform ballTrail;
    [SerializeField] float scaleMultiplier;
    [SerializeField] float minScale;
    [SerializeField] float maxScale;
    [SerializeField] Vector3 rotationOffset;
    [SerializeField] bool useSlerp;


    public bool isFrozen = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballCol = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        float speed = rb.linearVelocity.magnitude;

        float scale = Mathf.Clamp(speed * scaleMultiplier, minScale, maxScale);
        Vector3 newScale = ballTrail.localScale;
        newScale.x = scale;
        ballTrail.localScale = newScale;

        if (speed > 0.1f)
        {
            Vector3 dir = -rb.linearVelocity.normalized;

            Quaternion targetRot = Quaternion.FromToRotation(Vector3.right, dir);
            if (!useSlerp)
            {
                ballTrail.rotation = targetRot * Quaternion.Euler(rotationOffset);
                return;
            }

            ballTrail.rotation = Quaternion.Slerp(
                ballTrail.rotation,
                targetRot * Quaternion.Euler(rotationOffset),
                15f * Time.fixedDeltaTime
            );
        }
    }



    public void LaunchAtDirection(Vector3 dir, float force)
    {
        ReleasePossession();

        dir.Normalize();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(dir * force, ForceMode.Impulse);

        if (isFrozen)
        {
            GetComponent<BallIceController>().HurtIce();

        }

    }

    public void AttractTowards(Vector3 targetPos, float force, PlayerController requester)
    {
        if (currentActivePlayer != requester) return;

        Vector3 dir = (targetPos - transform.position);
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) return;
        dir.Normalize();

        rb.AddForce(dir * force, ForceMode.Force);
    }
    public void resetBallParent()
    {
        //first time this is called, it will check if CPUPlayers are there
        //consequent calls will skip lines if not needed
        if(areThereCPUPlayers)
        {
            //here it will check
            if (CPUPlayers == null)
            {
                CPUPlayers = FindObjectsByType<CPUEnemy>(FindObjectsSortMode.None);
                areThereCPUPlayers = CPUPlayers.Length == 0;
            }
            else
            {
                for (int i = 0; i < CPUPlayers.Length; i++)
                {
                    CPUPlayers[i].holdingBall = false;
                }
            }
        }

        transform.parent = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if(collision.gameObject.tag.Contains("Team") || collision.gameObject.tag.Contains("Player"))
        //{
        //    transform.parent = collision.transform;

        //    //reset the ball
        //    resetBallParent();

        //    if (collision.gameObject.tag.Contains("CPU"))
        //        collision.gameObject.GetComponent<CPUEnemy>().holdingBall = true;

        //    //physics
        //    Vector3 _dir = (transform.position - collision.transform.position);
        //    float _playerBallDot = Vector3.Dot(collision.transform.forward, _dir);

        //    //ball is in forward of the player
        //    if(_playerBallDot > 0.2f)
        //    {
        //        rb.linearVelocity = Vector3.zero;
        //        rb.angularVelocity = Vector3.zero;
        //        rb.AddForce(Vector3.up);
        //    }
        //}
        if (isPossessed) return;
        if (!collision.gameObject.CompareTag("Player"))
            return;
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (currentActivePlayer == player)
            return;
        if (player != null && GameSceneManager.Instance.canScore)
        {
            //float momentum = rb.linearVelocity.magnitude * rb.mass * 50;
            //Vector3 hitDirection = rb.linearVelocity.normalized;
            //ResolveImpact(player, collision, momentum, hitDirection);

            float relSpeed = collision.relativeVelocity.magnitude;
            Vector3 hitDirection = (player.transform.position - transform.position);

            float momentum = rb.mass * relSpeed;

            hitDirection.y = 0f;
            if (hitDirection.sqrMagnitude < 0.0001f)
                hitDirection = collision.GetContact(0).normal; // fallback
            hitDirection.Normalize();

            ResolveImpact(player, collision, momentum, hitDirection);
            Debug.Log($"Resolve Impact {player.name} |Collision: {collision}| Momentum: {momentum} | Hit Direction: {hitDirection}");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Contains("Team") || collision.gameObject.tag.Contains("Player"))
        {
            resetBallParent();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
            ReleasePossession();

        if (!isPossessed) return;
        if (currentActivePlayer == null) return;

        if (other.transform.root == currentActivePlayer.transform.root)
            return;

        if (((1 << other.gameObject.layer) & groundMask) != 0)
            return;

        ReleasePossession(currentActivePlayer);

        Vector3 dir = (transform.position - other.ClosestPoint(transform.position)).normalized;
        rb.AddForce(dir * 1.5f, ForceMode.Impulse);
    }



    private void ResolveImpact(PlayerController player, Collision collision, float momentum, Vector3 hitDirection)
    {

        float threshold1Low = threshold1 - thresholdBlend;
        float threshold1High = threshold1 + thresholdBlend;

        float threshold2Low = threshold2 - thresholdBlend;
        float threshold2High = threshold2 + thresholdBlend;

        float threshold3Low = threshold3 - thresholdBlend;

        if (momentum > threshold1Low && currentActivePlayer != null)
            ReleasePossession();
        // ball bounces off the player
        if (momentum <= threshold1Low)
        {
            // gain control
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            TryClaimPossession(player);

            player.GetHit(this, momentum, hitDirection, threshold1Low, threshold1High);
            return;
        }

        //medium momentum, ball is deflected
        if (momentum < threshold1High)
        {
            float blend01 = Mathf.InverseLerp(threshold1Low, threshold1High, momentum);

            rb.linearVelocity *= Mathf.Lerp(0.05f, 1f, blend01);
            rb.angularVelocity *= Mathf.Lerp(0.05f, 1f, blend01);

            // Only assign control if it’s still pretty low
            if (blend01 < 0.35f)
                TryClaimPossession(player);

            player.GetHit(this, momentum, hitDirection, threshold1, threshold2);
            return;
        }

        if(momentum < threshold2High)
        {
            // Player knockback uses real tiers
            player.GetHit(this, momentum, hitDirection, threshold1, threshold2);

            float deflect01 = Mathf.InverseLerp(threshold1High, threshold2High, momentum);
            rb.AddForce(-hitDirection * deflect01 * 2f, ForceMode.Impulse);
            return;
        }

        player.GetHit(this, momentum, hitDirection, threshold1, threshold2);

        // Only pass through when it's truly high (near threshold3)
        if (momentum >= threshold3Low)
        {
            Collider ballCol = GetComponent<Collider>();
            Collider otherCol = collision.collider;

            Physics.IgnoreCollision(ballCol, otherCol, true);
            StartCoroutine(ReenableCollision(otherCol, 0.2f));
        }

        if (ballHitSoftSfx != null && ballHitHardSfx != null)
        {
            if (momentum < threshold2)
            {
                ballHitSoftSfx.pitch = Random.Range(0.9f, 1.1f);
                if (ballHitSoftSfx.resource != null)
                    ballHitSoftSfx.Play();
                else if (ballHitSoftSfx.resource == null)
                {
                    Debug.Log("No audio clip assigned to ballHitSoftSfx on " + gameObject.name);
                }
            }
            else
            {
                ballHitHardSfx.pitch = Random.Range(0.9f, 1.1f);
                if (ballHitHardSfx.resource != null)
                    ballHitHardSfx.Play();
                else if (ballHitHardSfx.resource == null)
                {
                    Debug.Log("No audio clip assigned to ballHitHardSfx on " + gameObject.name);
                }
            }
        }
    }
    private IEnumerator ReenableCollision(Collider col, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics.IgnoreCollision(GetComponent<Collider>(), col, false);
    }
    public bool TryClaimPossession(PlayerController player)
    {
        if (isFrozen) return false;
        //Debug.LogWarning($"[POSSESSION] TryClaim called | player={(player ? player.name : "NULL")} | current={(currentActivePlayer ? currentActivePlayer.name : "NULL")} | t={Time.time:F2} next={nextPossessionTime:F2}");
        if (!CanClaimHere()) return false;
        if (player == null) return false;
        if (Time.time < nextPossessionTime) return false;

        // allow re-claim by same player
        if (currentActivePlayer != null && currentActivePlayer != player) return false;

        currentActivePlayer = player;
        currentActivePlayer.OnGainedPossession(this);

        EnterPossessedMode(player);

        Debug.Log($"[POSSESSION] {player.name} GAINED possession of {name}");
        return true;
    }

    public void ReleasePossession(PlayerController byWho = null)
    {
        if (currentActivePlayer == null) return;

        var old = currentActivePlayer;
        currentActivePlayer = null;

        old.OnLostPossession(this);

        ExitPossessedMode();

        Debug.Log($"[POSSESSION] {old.name} LOST possession of {name} (by {byWho?.name ?? "none"})");

        // Only cooldown if dispossessed by someone
        nextPossessionTime = (byWho != null) ? Time.time + possessionCooldown : 0f;
    }
    

    private void EnterPossessedMode(PlayerController player)
    {
        isPossessed = true;

        followTween?.Kill();
        followTween = null;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        /*rb.isKinematic = true;
        rb.useGravity = false;

        if (ballCol) ballCol.isTrigger = true;*/

        rb.isKinematic = false;
        //rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;


        if (player != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }



    private void ExitPossessedMode()
    {
        isPossessed = false;

        followTween?.Kill();
        followTween = null;

        // Restore physics
        rb.isKinematic = false;
        rb.useGravity = true;

        if (ballCol) ballCol.isTrigger = false;
    }


    public void MoveTowardAnchor(Vector3 anchorPos, PlayerController requester)
    {
        if (!isPossessed) return;
        if (currentActivePlayer != requester) return;

        anchorPos = ClampAnchorToGround(anchorPos);
        anchorPos.y += holdHeight;

        Vector3 toTarget = anchorPos - rb.position;
        toTarget.y = 0f;

        float dist = toTarget.magnitude;
        if (dist < 0.001f) return;

        Vector3 dir = toTarget / dist;

        float step = dribbleSpeed * Time.fixedDeltaTime;
        step = Mathf.Min(step, dist);

        Vector3 start = rb.position;

        if (Physics.SphereCast(start, ballRadius, dir, out RaycastHit hit, step))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                step = hit.distance - 0.005f; // small buffer
            }
        }

        Vector3 newPos = start + dir * step;

        // lock height
        newPos.y = ClampAnchorToGround(newPos).y + holdHeight;

        rb.MovePosition(newPos);
    }

    private Vector3 ClampAnchorToGround(Vector3 anchor)
    {
        Vector3 origin = anchor + Vector3.up * groundRayHeight;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundRayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            anchor.y = hit.point.y + groundOffset;
            return anchor;
        }

        anchor.y = groundOffset;
        return anchor;
    }
    private bool CanClaimHere()
    {
        // Raycast down from ball; if ground is far below, ball is airborne too much.
        Vector3 origin = transform.position + Vector3.up * 0.25f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 2f, groundMask, QueryTriggerInteraction.Ignore))
        {
            float height = transform.position.y - hit.point.y;
            return height <= maxClaimHeightAboveGround;
        }
        return true; // if no ground found, don’t block
    }
}
