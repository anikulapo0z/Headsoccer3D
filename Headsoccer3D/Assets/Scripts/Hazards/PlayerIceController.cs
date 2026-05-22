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

    private void Start()
    {
        currentIceHP = maxIceHP;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            HurtIce();
    }

    public void SetFrozen()
    {
        iceBlock.GetComponent<Renderer>().material.SetFloat("_Break_Intensity", 0);
        iceBlock.SetActive(true);

        GetComponent<PlayerController>().isFrozen = true;
        GetComponent<Rigidbody>().isKinematic = true;

        allowHurtIce = false;

        GetComponent<PlayerController>().LockPlayerMove();
        MoveBlock();
    }

    void MoveBlock()
    {
        transform.DOMove(new Vector3(transform.position.x, yVal, transform.position.z), moveYSpeed).OnComplete(()
            => UnlockPlayers());
    }

    void UnlockPlayers()
    {
        allowHurtIce = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
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
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        currentIceHP = maxIceHP;
        allowHurtIce = false;
        iceBlock.SetActive(false);
        if (CameraController.Instance != null)
            CameraController.Instance.ShakeCamera(0.5f, 0.1f, 25);
    }

    void ResetIceHit()
    {
        canHitIce = true;
    }
}
