using UnityEngine;
using UnityEngine.UI;

public class PlayerGroundMarker : MonoBehaviour
{

    [SerializeField] LayerMask groundLayer;
    public GameObject playerPositionIndicatorPrefab;
    [SerializeField] GameObject playerFloatingUIPrefab;
    GameObject playerPositionIndicator;
    GameObject playerFloatingUI;
    [SerializeField] float canvasOffset;
    [SerializeField] float lerpSpeed;

    Camera mainCam;
    [SerializeField] float uiSideOffset;
    [SerializeField] float uiHeightOffset;


    private void Start()
    {
        mainCam = Camera.main;
        playerPositionIndicator = Instantiate(playerPositionIndicatorPrefab);
        playerFloatingUI = Instantiate(playerFloatingUIPrefab);
        GetComponent<PlayerController>().SetStaminaBar(playerFloatingUI.GetComponentInChildren<Slider>());
    }

    private void FixedUpdate()
    {
        UpdateFloatingUIPosition();
        UpdateGroundIndicatorPosition();
    }

    void UpdateFloatingUIPosition()
    {
        if (!playerFloatingUI || !mainCam) return;

        Vector3 targetPos =
            transform.position
            + (mainCam.transform.right * uiSideOffset)
            + (Vector3.up * uiHeightOffset);

        playerFloatingUI.transform.position = targetPos;

        playerFloatingUI.transform.forward = mainCam.transform.forward;
    }

    void UpdateGroundIndicatorPosition()
    {
        if (!playerPositionIndicator) return;

        RaycastHit hit;

        playerPositionIndicator.transform.rotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, groundLayer))
        {
            playerPositionIndicator.transform.position = hit.point + new Vector3(0, canvasOffset, 0);
        }
    }

}
