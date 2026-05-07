using UnityEngine;

public class FaceCameraSprite : MonoBehaviour
{
    [Header("Optional: assign manually, otherwise uses main camera")]
    [SerializeField] private Camera targetCamera;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;


        Vector3 direction = targetCamera.transform.position - transform.position;


        transform.rotation = Quaternion.LookRotation(direction);
    }
}