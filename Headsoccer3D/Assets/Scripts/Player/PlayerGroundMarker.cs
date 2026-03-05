using UnityEngine;
using UnityEngine.UI;

public class PlayerGroundMarker : MonoBehaviour
{

    [SerializeField] LayerMask groundLayer;
    public GameObject playerPositionIndicatorPrefab;
    [SerializeField] GameObject playerFloatingUIPrefab;
    GameObject playerPositionIndicator;
    GameObject playerFloatingUI;

    public GameObject AbilityText;

    [SerializeField] float canvasOffset;
    [SerializeField] float lerpSpeed;

    Camera mainCam;
    [SerializeField] float uiSideOffset;
    [SerializeField] float uiHeightOffset;

    [SerializeField] GameObject matObj;



    [SerializeField] GameObject multiBallText;
    GameObject multiBallObject;
    [SerializeField] GameObject empoweredKickText;
    GameObject empoweredKickObject;
    [SerializeField] Vector3 mbOffset;
    [SerializeField] Vector3 ekOffset;
    [SerializeField] bool mbActive = false;
    [SerializeField] bool ekActive = false;








    private void Start()
    {
        mainCam = Camera.main;

    }

    public void SetPlayerWorldUIAndColor(GameObject posInd, Material playerMat)
    {
        playerPositionIndicator = Instantiate(posInd);
        playerFloatingUI = Instantiate(playerFloatingUIPrefab);
        GetComponent<PlayerController>().SetStaminaBar(playerFloatingUI.GetComponentInChildren<Slider>());


        Renderer rend = matObj.GetComponent<Renderer>();
        Material[] mats = rend.materials;
        mats[1] = playerMat;
        rend.materials = mats;

    }


    private void FixedUpdate()
    {
        UpdateFloatingUIPosition();
        UpdateGroundIndicatorPosition();
        if (mbActive)
            UpdateMBText();
        else if (ekActive)
            UpdateEKText();

    }

    void UpdateMBText()
    {
        if (multiBallObject == null)
            multiBallObject = Instantiate(multiBallText);


        Vector3 targetPos =
            transform.position
            + (mainCam.transform.right * mbOffset.z)
            + (Vector3.up * mbOffset.y);

        multiBallObject.transform.position = targetPos;

        multiBallObject.transform.forward = mainCam.transform.forward;

    }
    void UpdateEKText()
    {
        if (empoweredKickObject == null)
            empoweredKickObject = Instantiate(empoweredKickText);


        Vector3 targetPos =
            transform.position
            + (mainCam.transform.right * ekOffset.z)
            + (Vector3.up * ekOffset.y);

        empoweredKickObject.transform.position = targetPos;

        empoweredKickObject.transform.forward = mainCam.transform.forward;

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


    public void ToggleMBActive()
    {
        if (mbActive)
        {
            mbActive = false;
            if (multiBallObject != null)
                Destroy(multiBallObject);
        }
        else
        {
            mbActive = true;
            if (multiBallObject == null)
                multiBallObject = Instantiate(multiBallText);

        }
    }
    public void ToggleEKActive()
    {
        if (mbActive)
        {
            ekActive = false;
            if (empoweredKickObject != null)
                Destroy(empoweredKickObject);
        }
        else
        {
            ekActive = true;
            if (empoweredKickObject == null)
               empoweredKickObject = Instantiate(empoweredKickText);

        }
    }



}