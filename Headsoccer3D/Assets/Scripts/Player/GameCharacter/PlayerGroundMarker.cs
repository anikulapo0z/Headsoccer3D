using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    [SerializeField] GameObject earthquakeText;
    GameObject earthquakeObject;
    [SerializeField] GameObject shadowCloneText;
    GameObject shadowCloneObject;

    [SerializeField] Vector3 mbOffset;
    [SerializeField] Vector3 mbRotation;
    [SerializeField] Vector3 ekOffset;
    [SerializeField] Vector3 ekRotation;
    [SerializeField] Vector3 earthquakeOffset;
    [SerializeField] Vector3 earthquakeRotation;
    [SerializeField] Vector3 shadowCloneOffset;
    [SerializeField] Vector3 shadowCloneRotation;
    [SerializeField] bool mbActive = false;
    [SerializeField] bool ekActive = false;
    [SerializeField] bool earthquakeActive = false;
    [SerializeField] bool shadowCloneActive = false;

    public bool controllingFlipper = false;
    public List<GameObject> controlledFlippers = new List<GameObject>();
    [SerializeField] float lrYHeight;




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
        mats[0] = playerMat;
        rend.materials = mats;

    }

    public void DestroyGroundMarker()
    {
        Destroy(playerPositionIndicator);
    }

    private void FixedUpdate()
    {
        UpdateFloatingUIPosition();
        UpdateGroundIndicatorPosition();
        if (mbActive)
            UpdateMBText();
        else if (ekActive)
            UpdateEKText();
        else if (earthquakeActive)
            UpdateEarthquakeText();
        else if (shadowCloneActive)
            UpdateShadowCloneText();
        if (controllingFlipper)
            UpdateFlipperLine();
    }

    void UpdateFlipperLine()
    {
        foreach(GameObject t in controlledFlippers)
        {
            t.GetComponent<LineRenderer>().SetPosition(0, new Vector3(t.transform.position.x, lrYHeight, t.transform.position.z));
            t.GetComponent<LineRenderer>().SetPosition(1, new Vector3(transform.position.x, lrYHeight, transform.position.z));
        }
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
        //multiBallObject.transform.rotation = Quaternion.Euler(mbRotation);

        multiBallObject.transform.forward = mainCam.transform.forward;
        multiBallObject.transform.rotation = Quaternion.Euler(multiBallObject.transform.rotation.x + mbRotation.x, multiBallObject.transform.rotation.y + mbRotation.y, multiBallObject.transform.rotation.z + mbRotation.z);


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
        //empoweredKickObject.transform.rotation = Quaternion.Euler(ekRotation);

        empoweredKickObject.transform.forward = mainCam.transform.forward;
        empoweredKickObject.transform.rotation = Quaternion.Euler(empoweredKickObject.transform.rotation.x + ekRotation.x, empoweredKickObject.transform.rotation.y + ekRotation.y, empoweredKickObject.transform.rotation.z + ekRotation.z);
    }
    void UpdateEarthquakeText()
    {
        if (earthquakeObject == null)
            earthquakeObject = Instantiate(earthquakeText);

        Vector3 targetPos =
            transform.position
            + (mainCam.transform.right * ekOffset.z)
            + (Vector3.up * ekOffset.y);

        earthquakeObject.transform.position = targetPos;
        //empoweredKickObject.transform.rotation = Quaternion.Euler(ekRotation);

        earthquakeObject.transform.forward = mainCam.transform.forward;
        earthquakeObject.transform.rotation = Quaternion.Euler(earthquakeObject.transform.rotation.x + earthquakeRotation.x, earthquakeObject.transform.rotation.y + earthquakeRotation.y, earthquakeObject.transform.rotation.z + earthquakeRotation.z);
    }
    void UpdateShadowCloneText()
    {
        if (shadowCloneObject == null)
            shadowCloneObject = Instantiate(shadowCloneText);

        Vector3 targetPos =
            transform.position
            + (mainCam.transform.right * shadowCloneOffset.z)
            + (Vector3.up * shadowCloneOffset.y);

        shadowCloneObject.transform.position = targetPos;
        //empoweredKickObject.transform.rotation = Quaternion.Euler(ekRotation);

        shadowCloneObject.transform.forward = mainCam.transform.forward;
        shadowCloneObject.transform.rotation = Quaternion.Euler(shadowCloneObject.transform.rotation.x + shadowCloneRotation.x, shadowCloneObject.transform.rotation.y + shadowCloneRotation.y, shadowCloneObject.transform.rotation.z + shadowCloneRotation.z);
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
        if (ekActive)
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
    public void ToggleEarthquakeActive()
    {
        if (earthquakeActive)
        {
            earthquakeActive = false;
            if (earthquakeObject != null)
                Destroy(earthquakeObject);
        }
        else
        {
            earthquakeActive = true;
            if (earthquakeObject == null)
                earthquakeObject = Instantiate(earthquakeText);

        }
    }
    public void ToggleShadowCloneActive()
    {
        if (shadowCloneActive)
        {
            shadowCloneActive = false;
            if (shadowCloneObject != null)
                Destroy(shadowCloneObject);
        }
        else
        {
            shadowCloneActive = true;
            if (shadowCloneObject == null)
                shadowCloneObject = Instantiate(shadowCloneText);

        }
    }

}