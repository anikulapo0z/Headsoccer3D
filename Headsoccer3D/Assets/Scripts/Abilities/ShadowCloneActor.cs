using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class ShadowCloneActor : MonoBehaviour
{
    private PlayerController owner;
    private PlayerController cloneController;

    private ShadowCloneAbility.CloneMode mode;
    private Vector3 outwardDirection;

    private float lifetime;
    private float kickInterval;

    private float lifeTimer;
    private float kickTimer;
    private bool previousOwnerKickHeld;
    private bool despawning;

    [Header("Visual")]
    [SerializeField] private float spawnScaleTime = 0.12f;
    [SerializeField] private float despawnScaleTime = 0.2f;
    [SerializeField] private float flashScale = 1.25f;
    [SerializeField] private Renderer meshWithMaterials;


    public void Init(
        PlayerController owner,
        ShadowCloneAbility.CloneMode mode,
        Vector3 outwardDirection,
        float lifetime,
        float kickInterval
    )
    {
        this.owner = owner;
        this.mode = mode;
        this.outwardDirection = outwardDirection.normalized;
        this.lifetime = lifetime;
        this.kickInterval = kickInterval;

        cloneController = GetComponent<PlayerController>();

        if (cloneController == null)
        {
            Debug.LogError("[SHADOW CLONE] Clone prefab needs a PlayerController if you want exact player behavior.");
            Destroy(gameObject);
            return;
        }

        DisableRealPlayerOnlyComponents();

        lifeTimer = lifetime;
        kickTimer = 0f;

        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(originalScale, spawnScaleTime).SetEase(Ease.OutBack);
    }

    private void DisableRealPlayerOnlyComponents()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null)
            input.enabled = false;

        PlayerAbility ability = GetComponent<PlayerAbility>();
        if (ability != null)
            ability.enabled = false;

        PlayerGroundMarker marker = GetComponent<PlayerGroundMarker>();
        if (marker != null)
            marker.enabled = false;

        // Stops shadowclones from being able to use abilities.
        // Keep PlayerController enabled because that is what gives us movement and animations and everything else
    }

    private void FixedUpdate()
    {
        if (despawning)
            return;

        if (owner == null || cloneController == null)
        {
            Despawn();
            return;
        }

        lifeTimer -= Time.fixedDeltaTime;

        if (lifeTimer <= 0f)
        {
            Despawn();
            return;
        }

        switch (mode)
        {
            case ShadowCloneAbility.CloneMode.MirrorControls:
                MirrorOwner();
                break;

            case ShadowCloneAbility.CloneMode.OutwardKicking:
                MoveOutwardAndKick();
                break;
        }
    }

    private void MirrorOwner()
    {
        cloneController.OnMove(owner.MoveInput);
        cloneController.OnSprint(owner.SprintHeld);

        bool ownerKickHeld = owner.KickHeld;

        if (ownerKickHeld != previousOwnerKickHeld)
        {
            cloneController.OnKick(ownerKickHeld);
            previousOwnerKickHeld = ownerKickHeld;
        }
    }

    private void MoveOutwardAndKick()
    {
        Vector2 moveInput = new Vector2(outwardDirection.x, outwardDirection.z);

        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        cloneController.OnMove(moveInput);
        cloneController.OnSprint(false);

        kickTimer -= Time.fixedDeltaTime;

        if (kickTimer <= 0f)
        {
            kickTimer = kickInterval;
            StartCoroutine(TapKick());
        }
    }

    private IEnumerator TapKick()
    {
        cloneController.OnKick(true);
        yield return null;
        cloneController.OnKick(false);
    }

    private void Despawn()
    {
        if (despawning)
            return;

        despawning = true;

        if (cloneController != null)
        {
            cloneController.OnMove(Vector2.zero);
            cloneController.OnSprint(false);

            if (previousOwnerKickHeld)
            {
                cloneController.OnKick(false);
                previousOwnerKickHeld = false;
            }
        }

        transform.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(transform.localScale * flashScale, 0.08f));
        seq.Append(transform.DOScale(Vector3.zero, despawnScaleTime));
        seq.OnComplete(() => Destroy(gameObject));
    }

    public void setShadowMaterials(Material[] _mats)
    {
        meshWithMaterials.materials = _mats;
    }
}