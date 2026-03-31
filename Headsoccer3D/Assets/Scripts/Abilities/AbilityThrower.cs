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



    private void FixedUpdate()
    {
        throwTime -= Time.deltaTime;

        if(throwTime < 0)
        {
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



}
