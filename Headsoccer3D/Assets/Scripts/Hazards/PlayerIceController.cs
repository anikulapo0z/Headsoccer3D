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


    private void Start()
    {
        currentIceHP = maxIceHP;
    }

    public void SetFrozen()
    {
        iceBlock.GetComponent<Renderer>().material.SetFloat("_Break_Intensity", 0);
        iceBlock.SetActive(true);

        GetComponent<PlayerController>().isFrozen = true;
        GetComponent<PlayerController>().LockPlayerMove();
        MoveBlock();
    }

    void MoveBlock()
    {
        transform.DOMoveY(yVal, moveYSpeed).OnComplete(()
            => UnlockPlayers());
    }

    void UnlockPlayers()
    {
        allowHurtIce = true;
        GetComponent<PlayerController>().UnlockPlayerMove();
    }

    public void HurtIce()
    {
        if (!allowHurtIce) return;

        currentIceHP--;
        if(currentIceHP <= 0)
        {
            BreakIce();
        }

        float amount = (maxIceHP - currentIceHP) / maxIceHP;
        iceBlock.GetComponent<Renderer>().material.SetFloat("_Break_Intensity", amount);

    }
    void BreakIce()
    {
        GetComponent<PlayerController>().isFrozen = false;
        currentIceHP = maxIceHP;
        allowHurtIce = false;
        iceBlock.SetActive(false);
    }

}
