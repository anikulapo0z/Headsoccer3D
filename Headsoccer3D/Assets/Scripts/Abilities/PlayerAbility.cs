using UnityEngine;
using DG.Tweening;

public class PlayerAbility : MonoBehaviour
{
    public AbilityTrigger.AbilityTypes currentAbility;



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

        }
    }

    public void SetAbility(AbilityTrigger.AbilityTypes ability)
    {
        currentAbility = ability;

        switch (currentAbility)
        {
            case AbilityTrigger.AbilityTypes.None:
                break;

            case AbilityTrigger.AbilityTypes.EmpoweredKick:
                if (GetComponent<EmpoweredKick>() == null)
                    gameObject.AddComponent<EmpoweredKick>();

                GetComponent<EmpoweredKick>().empoweredKickStrength = empoweredKickStrength;
                GetComponent<PlayerController>().hasEmpoweredKick = true;
                GetComponent<PlayerController>().empoweredKickStrength = empoweredKickStrength;
                GetComponent<PlayerController>().empoweredKickPlayerMultiplier = empoweredKickStrength * 0.6f;
                GetComponent<PlayerGroundMarker>().ToggleEKActive();

                originalScale = transform.localScale;
                transform.DOScale(growSize, scaleTime);
                break;

            case AbilityTrigger.AbilityTypes.MultiBall:
                if(GetComponent<MultiBall>() == null)
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
                GetComponent<PlayerController>().hasEmpoweredKick = false;
                GetComponent<PlayerController>().empoweredKickStrength = 1f;
                GetComponent<PlayerGroundMarker>().ToggleEKActive();
                Destroy(GetComponent<EmpoweredKick>());
                break;

            case AbilityTrigger.AbilityTypes.MultiBall:
                Destroy(GetComponent<MultiBall>());
                GetComponent<PlayerGroundMarker>().ToggleMBActive();
                break;

        }
        currentAbility = AbilityTrigger.AbilityTypes.None;

    }
}
