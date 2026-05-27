using DG.Tweening;
using UnityEngine;

public class PlayerIceController : MonoBehaviour
{
    [SerializeField] GameObject iceBlock;
    

    [SerializeField] float yVal;
    [SerializeField] float moveYSpeed;
    [SerializeField] int maxIceHP;
    [SerializeField] int currentIceHP;
    [SerializeField] bool allowHurtIce = false;

    [SerializeField] float iceHitCooldown;
    [SerializeField] bool canHitIce = true;
    Rigidbody rb;

    [Header("Block Break")]
    [SerializeField] GameObject iceBlockFracture;
    [SerializeField] float forceStrength;
    [SerializeField] AnimationCurve iceTossSpeed;


    private void Start()
    {
        currentIceHP = maxIceHP;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            HurtIce();
        //Debug.LogWarning("linear vel: " + GetComponent<Rigidbody>().linearVelocity);
        //Debug.LogWarning("angular vel: " + GetComponent<Rigidbody>().angularVelocity);
    }

    public void SetFrozen()
    {



        iceBlock.GetComponent<Renderer>().material.SetFloat("_Break_Intensity", 0);
        iceBlock.SetActive(true);

        GetComponent<PlayerController>().isFrozen = true;
        rb.isKinematic = true;

        allowHurtIce = false;

        GetComponent<PlayerController>().LockPlayerMove();
        GameSceneManager.Instance.GetComponent<IceController>().ResetIce();

        MoveBlock();

        if (iceBlockFracture == null) return;
        foreach (Transform child in iceBlockFracture.transform)
        {
            child.GetComponent<Rigidbody>().isKinematic = true;
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.Euler(-89.98f, 0, 0);
            child.transform.localScale = Vector3.one;
        }
    }

    void MoveBlock()
    {
        transform.DOMove(new Vector3(transform.position.x, yVal, transform.position.z), moveYSpeed).SetEase(iceTossSpeed).OnComplete(()
            => UnlockPlayers());
    }

    void UnlockPlayers()
    {
        allowHurtIce = true;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        //GetComponent<PlayerController>().UnlockPlayerMove();
    }

    public void HurtIce()
    {
        if (!allowHurtIce || !canHitIce) return;

        currentIceHP--;
        if(currentIceHP <= 0)
        {
            BreakIce();
        }

        float amount = ((float)maxIceHP - (float)currentIceHP) / (float)maxIceHP;
        Debug.Log("amount: " + amount);
        iceBlock.GetComponent<Renderer>().material.SetFloat("_Break_Intensity", amount);

        if(CameraController.Instance != null)
            CameraController.Instance.ShakeCamera();

        canHitIce = false;
        Invoke("ResetIceHit", iceHitCooldown);
    }

    void BreakIce()
    {
        GetComponent<PlayerController>().isFrozen = false;
        GetComponent<PlayerController>().UnlockPlayerMove();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        currentIceHP = maxIceHP;
        allowHurtIce = false;
        iceBlock.SetActive(false);
        if (CameraController.Instance != null)
            CameraController.Instance.ShakeCamera(0.5f, 0.1f, 25);

        if (iceBlockFracture == null) return;

        iceBlockFracture.SetActive(true);
        foreach (Transform child in iceBlockFracture.transform)
        {
            child.GetComponent<Rigidbody>().isKinematic = false;
            child.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            child.parent = null;
            child.GetComponent<Rigidbody>().AddForce((transform.position - child.position).normalized * forceStrength, ForceMode.Impulse);
            child.DOScale(0.001f, 5).OnComplete(() => iceBlockFracture.SetActive(false));
        }

    }

    void ResetIceHit()
    {
        canHitIce = true;
    }
}
