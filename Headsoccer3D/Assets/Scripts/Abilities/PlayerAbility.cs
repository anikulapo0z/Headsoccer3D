using UnityEngine;
using DG.Tweening;

public class PlayerAbility : MonoBehaviour
{
    public AbilityTrigger.AbilityTypes currentAbility;
    private PlayerAudioManager audioManager;

    [Space(5)]
    [Header("Ability Stats")]
    [Space(3)]
    [Header("Empowered Kick")]
    [SerializeField] float empoweredKickStrength;
    [SerializeField] float scaleTime;
    [SerializeField] Vector3 growSize;

    public GameObject kickWave;


    Vector3 originalScale;

    [Space(3)]
    [Header("Multi Ball")]
    [SerializeField] float upAmount;
    [SerializeField] float outAmount;
    [SerializeField] int ballAmount;
    [SerializeField] GameObject ball;

    [Space(5)]
    [Header("Earthquake")]
    [SerializeField] GameObject earthquakePrefab;
    [SerializeField] float earthquakeRadius;
    [SerializeField] float earthquakeDuration;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float earthquakeYForce;
    [SerializeField] float earthquakeOutForce;
    [SerializeField] float earthquakePlayerForce;
    [SerializeField] AudioSource earthquakeSound;
    [Space(5)]
    [Header("Shadow Clone")]
    [SerializeField] GameObject shadowClonePrefab;
    [SerializeField] float shadowCloneLifetime = 3f;
    [SerializeField] float shadowCloneSpawnDistance = 2f;
    [SerializeField] float shadowCloneMoveSpeed = 7f;
    [SerializeField] float shadowCloneKickInterval = 0.5f;
    [SerializeField] float shadowCloneKickRadius = 1.5f;
    [SerializeField] float shadowCloneKickForce = 10f;
    [SerializeField] private LayerMask shadowCloneKickMask;
    [SerializeField] private ShadowCloneAbility.CloneMode shadowCloneMode;
    [SerializeField] private ShadowCloneAbility.SpawnPattern shadowCloneSpawnPattern;

    public void Awake()
    {
        audioManager = GetComponent<PlayerAudioManager>();
    }
    public void TryTriggerAbility()
    {
        switch (currentAbility)
        {
            case AbilityTrigger.AbilityTypes.None:
                break;

            case AbilityTrigger.AbilityTypes.EmpoweredKick:
                GetComponent<EmpoweredKick>().UseAbility();
                break;

            case AbilityTrigger.AbilityTypes.MultiBall:
                GetComponent<MultiBall>().UseAbility();
                break;

            case AbilityTrigger.AbilityTypes.Earthquake:
                GetComponent<Earthquake>().UseAbility();
                break;

            case AbilityTrigger.AbilityTypes.ShadowClone:
                GetComponent<ShadowCloneAbility>().UseAbility();
                break;

        }
    }

    public void SetAbility(AbilityTrigger.AbilityTypes ability)
    {
        audioManager.PlayPickupSfx();

        currentAbility = ability;

        switch (currentAbility)
        {
            case AbilityTrigger.AbilityTypes.None:
                break;

            case AbilityTrigger.AbilityTypes.EmpoweredKick:
                if (GetComponent<EmpoweredKick>() == null)
                    gameObject.AddComponent<EmpoweredKick>();

                GetComponent<EmpoweredKick>().empoweredKickStrength = empoweredKickStrength;
                GetComponent<EmpoweredKick>().player = gameObject;

                //GetComponent<PlayerController>().hasEmpoweredKick = true;
                //GetComponent<PlayerController>().empoweredKickStrength = empoweredKickStrength;
                //GetComponent<PlayerController>().empoweredKickPlayerMultiplier = empoweredKickStrength * 0.6f;
                GetComponent<PlayerGroundMarker>().ToggleEKActive();

                originalScale = transform.localScale;
                transform.DOScale(growSize, scaleTime);
                break;

            case AbilityTrigger.AbilityTypes.MultiBall:
                if (GetComponent<MultiBall>() == null)
                    gameObject.AddComponent<MultiBall>();
                GetComponent<MultiBall>().SetVars(upAmount, outAmount, ballAmount, ball);
                GetComponent<PlayerGroundMarker>().ToggleMBActive();
                break;

            case AbilityTrigger.AbilityTypes.Earthquake:
                if (GetComponent<Earthquake>() == null)
                    gameObject.AddComponent<Earthquake>();
                Earthquake eq = GetComponent<Earthquake>();
                eq.aliveTime = earthquakeDuration;
                eq.ground = groundLayer;
                eq.radius = earthquakeRadius;
                eq.earthquakeRef = earthquakePrefab;
                eq.yKick = earthquakeYForce;
                eq.ballKickForce = earthquakeOutForce;
                eq.playerKickForce = earthquakePlayerForce;
                eq.player = gameObject;
                eq.source = earthquakeSound;

                GetComponent<PlayerGroundMarker>().ToggleEarthquakeActive();

                break;

            case AbilityTrigger.AbilityTypes.ShadowClone:
                if (GetComponent<ShadowCloneAbility>() == null)
                    gameObject.AddComponent<ShadowCloneAbility>();
                ShadowCloneAbility sc = GetComponent<ShadowCloneAbility>();
                sc.player = gameObject;
                sc.clonePrefab = shadowClonePrefab;
                sc.lifetime = shadowCloneLifetime;
                sc.spawnDistance = shadowCloneSpawnDistance;
                sc.moveSpeed = shadowCloneMoveSpeed;
                sc.kickInterval = shadowCloneKickInterval;
                sc.kickRadius = shadowCloneKickRadius;
                sc.kickForce = shadowCloneKickForce;
                sc.kickMask = shadowCloneKickMask;
                sc.mode = shadowCloneMode;
                sc.spawnPattern = shadowCloneSpawnPattern;

                GetComponent<PlayerGroundMarker>().ToggleShadowCloneActive();


                break;

        }
    }

    public void ResetAbilityUse()
    {
        switch (currentAbility)
        {
            case AbilityTrigger.AbilityTypes.None:
                break;

            case AbilityTrigger.AbilityTypes.EmpoweredKick:
                GetComponent<EmpoweredKick>().ResetAbilityUse(originalScale, scaleTime);
                //GetComponent<PlayerController>().hasEmpoweredKick = false;
                //GetComponent<PlayerController>().empoweredKickStrength = 1f;
                GetComponent<PlayerGroundMarker>().ToggleEKActive();
                Destroy(GetComponent<EmpoweredKick>());
                break;

            case AbilityTrigger.AbilityTypes.MultiBall:
                Destroy(GetComponent<MultiBall>());
                GetComponent<PlayerGroundMarker>().ToggleMBActive();
                break;

            case AbilityTrigger.AbilityTypes.Earthquake:
                GetComponent<PlayerGroundMarker>().ToggleEarthquakeActive();

                Destroy(GetComponent<Earthquake>());
                break;

            case AbilityTrigger.AbilityTypes.ShadowClone:
                Destroy(GetComponent<ShadowCloneAbility>());

                GetComponent<PlayerGroundMarker>().ToggleShadowCloneActive();

                break;

        }
        currentAbility = AbilityTrigger.AbilityTypes.None;

    }

    public void StopEarthquake()
    {
        if(GetComponent<Earthquake>() != null && GetComponent<Earthquake>().earthQuakeActive)
            Destroy(GetComponent<Earthquake>());
    }
}