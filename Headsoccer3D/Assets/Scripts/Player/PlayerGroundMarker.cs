using UnityEngine;

public class PlayerGroundMarker : MonoBehaviour
{

    [SerializeField] LayerMask groundLayer;
    public GameObject playerPositionIndicatorPrefab;
    GameObject playerPositionIndicator;
    [SerializeField] float canvasOffset;
    [SerializeField] float lerpSpeed;

    private void Start()
    {
        playerPositionIndicator = Instantiate(playerPositionIndicatorPrefab);
    }

    private void FixedUpdate()
    {
        if (!playerPositionIndicator) return;

        RaycastHit hit;

        playerPositionIndicator.transform.rotation = transform.rotation;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, groundLayer))
        {
            playerPositionIndicator.transform.position = Vector3.Lerp(
                playerPositionIndicator.transform.position,
                hit.point + new Vector3(0, canvasOffset, 0),
                Time.deltaTime * lerpSpeed);
        }
    }
}
