using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AbilityThrower : MonoBehaviour
{
    [SerializeField] private GameObject[] abilities;
    [SerializeField] private Transform[] spawnPoint;
    [Tooltip("The order of the chars must be the same as the order of the spawnPoints for the anim to match up. Leave empty if no animation needed.")]
    [SerializeField] private GameObject[] throwerChars;
    private Animator[] throwerAnims;

    [Header("Destination Range")]
    [SerializeField] float minX;
    [SerializeField] float maxX;
    [SerializeField] float minZ;
    [SerializeField] float maxZ;
    [SerializeField] float destinationY;

    [Header("Arc")]
    [SerializeField] float minArcHeight;
    [SerializeField] float maxArcHeight;
    [SerializeField] float minDuration;
    [SerializeField] float maxDuration;

    [Header("Throw Timing")]
    [SerializeField] float minThrowTime;
    [SerializeField] float maxThrowTime;
    [SerializeField] float throwTime = 5f;

    bool start = false;

    [Space]
    [Header("Bus Map Specific")]
    [SerializeField] bool isBusMap = false;
    [SerializeField] Transform bus1;
    [SerializeField] Collider bus1Collider;
    [Space]
    [SerializeField] Transform bus2;
    [SerializeField] Collider bus2Collider;

    [Space]
    [Header("Max Abilities")]
    [SerializeField] int maxAbilitiesOnField;
    [SerializeField] int currentAbilitiesOnField = 0;



    public void StartThrow()
    {
        
        if (GameSceneManager.Instance.inputControllers.Count < 3)
        {
            minThrowTime = 6;
            maxThrowTime = 9;
        }
        start = true;

        //if we have animation, set the animator refs
        if(throwerChars.Length > 0)
        {
            throwerAnims = new Animator[throwerChars.Length];
            for(int i = 0; i < throwerChars.Length; i++)
            {
                throwerAnims[i] = throwerChars[i].GetComponentInChildren<Animator>();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!start) return;

        throwTime -= Time.deltaTime;

        if(throwTime < 0)
        {
            throwTime = Random.Range(minThrowTime, maxThrowTime);

            if (currentAbilitiesOnField >= maxAbilitiesOnField) return;

            currentAbilitiesOnField++;
            if (isBusMap)
                ThrowRandomAbilityOnBus();
            else
                StartCoroutine(ThrowRandomAbility());
        }
    }

    IEnumerator ThrowRandomAbility()
    {
        int point = Random.Range(0, spawnPoint.Length);

        //if there is anim, wait a bit, other wise wait less
        yield return new WaitForSeconds(throwerAnims.Length > 0 ? 0.67458f : 0.1186f);

        if(throwerAnims.Length > 0 )
        {
            throwerAnims[point].SetTrigger("Throw");
            //throwerChars[point].transform.DOMoveZ(throwerChars[point].transform.position.z + 0.8f, 0.1f);
        }


        GameObject abilityPrefab = abilities[Random.Range(0, abilities.Length)];

        GameObject abilityInstance = Instantiate(abilityPrefab, spawnPoint[point].position, Quaternion.identity);
        abilityInstance.GetComponent<AbilityPickup>().thrower = this;


        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);
        Vector3 destination = new Vector3(randomX, destinationY, randomZ);

        float arcHeight = Random.Range(minArcHeight, maxArcHeight);
        float duration = Random.Range(minDuration, maxDuration);

        Vector3 midPoint = (spawnPoint[point].position + destination) / 2f;
        midPoint.y += arcHeight;

        Vector3[] path = new Vector3[]
        {
            spawnPoint[point].position,
            midPoint,
            destination
        };
        abilityInstance.transform.DOJump(destination, arcHeight, 1, duration); ;

    }

    void ThrowRandomAbilityOnBus()
    {
        int point = Random.Range(0, spawnPoint.Length);

        GameObject abilityPrefab = abilities[Random.Range(0, abilities.Length)];

        GameObject abilityInstance = Instantiate(abilityPrefab, spawnPoint[point].position, Quaternion.identity);
        abilityInstance.GetComponent<AbilityPickup>().thrower = this;


        float randomX;
        float randomZ;

        int x = Random.Range(0, 2);
        if (x == 0)
        {
            randomX = Random.Range(bus1Collider.bounds.min.x, bus1Collider.bounds.max.x);
            randomZ = Random.Range(bus1Collider.bounds.min.z, bus1Collider.bounds.max.z);
            abilityInstance.transform.SetParent(bus1);
        }
        else
        {
            randomX = Random.Range(bus2Collider.bounds.min.x, bus2Collider.bounds.max.x);
            randomZ = Random.Range(bus2Collider.bounds.min.z, bus2Collider.bounds.max.z);
            abilityInstance.transform.SetParent(bus2);
        }



        //float randomX = Random.Range(minX, maxX);
        //float randomZ = Random.Range(minZ, maxZ);
        Vector3 destination = new Vector3(randomX, destinationY, randomZ);

        float arcHeight = Random.Range(minArcHeight, maxArcHeight);
        float duration = Random.Range(minDuration, maxDuration);

        Vector3 midPoint = (spawnPoint[point].position + destination) / 2f;
        midPoint.y += arcHeight;

        Vector3[] path = new Vector3[]
        {
            spawnPoint[point].position,
            midPoint,
            destination
        };
        abilityInstance.transform.DOJump(destination, arcHeight, 1, duration); ;

    }

    public void ItemPickedUp()
    {
        currentAbilitiesOnField--;
    }

}
