using DG.Tweening;
using UnityEngine;

public class BallIceController : MonoBehaviour
{
    [SerializeField] GameObject iceBlock;
    [SerializeField] int maxIceHP;
    [SerializeField] int currentIceHP;
    [SerializeField] bool allowHurtIce = false;

    [SerializeField] float iceHitCooldown;
    [SerializeField] bool canHitIce = true;
    Rigidbody rb;
    SoccerBall soccerBall;
    [SerializeField] float frozenMass;
    [SerializeField] float defaultMass;
    [SerializeField] float yVal;
    [SerializeField] float moveYSpeed;

    [SerializeField] AnimationCurve iceTossSpeed;




    void Start()
    {
        currentIceHP = maxIceHP;
        rb = GetComponent<Rigidbody>();
        soccerBall = GetComponent<SoccerBall>();
    }

    public void SetFrozen()
    {
        iceBlock.GetComponent<Renderer>().material.SetFloat("_Break_Intensity", 0);
        iceBlock.SetActive(true);

        soccerBall.isFrozen = true;
        rb.mass = frozenMass;
        //rb.constraints.FreezeRotationX;
        GameSceneManager.Instance.GetComponent<IceController>().ResetIce();
        MoveBlock();
    }

    void MoveBlock()
    {
        transform.DOMove(new Vector3(transform.position.x, yVal, transform.position.z), moveYSpeed).SetEase(iceTossSpeed).OnComplete(() => SetVals());
    }
    void SetVals()
    {
        allowHurtIce = true;
        canHitIce = true;
    }


    public void HurtIce()
    {
        if (!allowHurtIce || !canHitIce) return;

        currentIceHP--;
        if (currentIceHP <= 0)
        {
            BreakIce();
        }

        float amount = ((float)maxIceHP - (float)currentIceHP) / (float)maxIceHP;
        //Debug.Log("amount: " + amount);
        iceBlock.GetComponent<Renderer>().material.SetFloat("_Break_Intensity", amount);

        if (CameraController.Instance != null)
            CameraController.Instance.ShakeCamera();

        canHitIce = false;
        Invoke("ResetIceHit", iceHitCooldown);
    }
    void ResetIceHit()
    {
        canHitIce = true;
    }

    void BreakIce()
    {
        iceBlock.SetActive(false);
        rb.mass = defaultMass;
        canHitIce = false;
        allowHurtIce = false;
        soccerBall.isFrozen = false;

    }

}
