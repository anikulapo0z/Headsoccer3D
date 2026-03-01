using UnityEngine;
using DG.Tweening;

public class EmpoweredKick : MonoBehaviour
{
    public float empoweredKickStrength;



    public void UseAbility()
    {
        Debug.LogError("try use ablity");
        GetComponent<PlayerController>().OnKick(false);
    }

    public void ResetAbilityUse(Vector3 originalScale, float time)
    {
        Debug.Log("try reset scale");
        transform.DOScale(originalScale, time);
    }
}
