using UnityEngine;

public class ShadowCloneAbility : MonoBehaviour
{
    public enum CloneMode
    {
        MirrorControls,
        OutwardKicking
    }

    public enum SpawnPattern
    {
        Cardinal,
        Diagonal
    }

    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject clonePrefab;

    [HideInInspector] public float lifetime;
    [HideInInspector] public float spawnDistance;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float kickInterval;
    [HideInInspector] public float kickRadius;
    [HideInInspector] public float kickForce;
    [HideInInspector] public LayerMask kickMask;

    [HideInInspector] public CloneMode mode;
    [HideInInspector] public SpawnPattern spawnPattern;

    private Material[] spawnerDudeMaterials;

    private bool usedAbility;

    public void UseAbility()
    {
        if (usedAbility) return;
        usedAbility = true;

        if (clonePrefab == null)
        {
            Debug.LogError("[SHADOW CLONE] No clonePrefab assigned on ShadowCloneAbility.");
            GetComponent<PlayerAbility>().ResetAbilityUse();
            return;
        }

        PlayerController ownerController = GetComponent<PlayerController>();

        if (ownerController == null)
        {
            Debug.LogError("[SHADOW CLONE] Player using ability has no PlayerController.");
            GetComponent<PlayerAbility>().ResetAbilityUse();
            return;
        }

        Vector3[] offsets = GetSpawnOffsets();

        foreach (Vector3 offset in offsets)
        {
            Vector3 spawnPos = transform.position + offset;

            GameObject cloneObj = Instantiate(
                clonePrefab,
                spawnPos,
                transform.rotation
            );

            ShadowCloneActor clone = cloneObj.GetComponent<ShadowCloneActor>();

            if (clone == null)
            {
                Debug.LogError("Clone prefab is missing ShadowCloneActor dattebayo!!!");
                Destroy(cloneObj);
                continue;
            }

            clone.Init(
                owner: ownerController,
                mode: mode,
                outwardDirection: offset.normalized,
                lifetime: lifetime,
                kickInterval: kickInterval
            );

            clone.setShadowMaterials(spawnerDudeMaterials);
        }

        GetComponent<PlayerAbility>().ResetAbilityUse();
    }

    private Vector3[] GetSpawnOffsets()
    {
        if (spawnPattern == SpawnPattern.Cardinal)
        {
            return new Vector3[]
            {
                transform.forward * spawnDistance,
                -transform.forward * spawnDistance,
                transform.right * spawnDistance,
                -transform.right * spawnDistance
            };
        }

        Vector3 frontRight = (transform.forward + transform.right).normalized;
        Vector3 frontLeft = (transform.forward - transform.right).normalized;
        Vector3 backRight = (-transform.forward + transform.right).normalized;
        Vector3 backLeft = (-transform.forward - transform.right).normalized;

        return new Vector3[]
        {
            frontRight * spawnDistance,
            frontLeft * spawnDistance,
            backRight * spawnDistance,
            backLeft * spawnDistance
        };
    }

    public void setMaterialsToBeUsed(Material[] _mats)
    {
        spawnerDudeMaterials = _mats;
    }
}
