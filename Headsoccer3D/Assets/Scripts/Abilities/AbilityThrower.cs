using DG.Tweening;
using UnityEngine;

public class AbilityThrower : MonoBehaviour
{
    [SerializeField] private GameObject[] abilities;
    [SerializeField] private Transform[] spawnPoint;

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


    public void StartThrow()
    {
        if (GameSceneManager.Instance.inputControllers.Count < 3)
        {
            minThrowTime = 6;
            maxThrowTime = 9;
        }
        start = true;
    }

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.M)) start = true;
        if (!start) return;

        throwTime -= Time.deltaTime;

        if(throwTime < 0)
        {
            if(isBusMap)
                ThrowRandomAbilityOnBus();
            else
                ThrowRandomAbility();
            throwTime = Random.Range(minThrowTime, maxThrowTime);
        }
    }

    void ThrowRandomAbility()
    {
        int point = Random.Range(0, spawnPoint.Length);


        GameObject abilityPrefab = abilities[Random.Range(0, abilities.Length)];

        GameObject abilityInstance = Instantiate(abilityPrefab, spawnPoint[point].position, Quaternion.identity);

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


}
