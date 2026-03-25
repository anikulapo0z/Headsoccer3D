using UnityEngine;
using DG.Tweening;

public class EmpoweredKick : MonoBehaviour
{
    public float empoweredKickStrength;
    //public GameObject kickWave;
    PlayerAbility playerAbility;


    private void Start()
    {
        playerAbility = GetComponent<PlayerAbility>();
    }

    public void UseAbility()
    {
        //Debug.LogError("try use ablity");
        Instantiate(playerAbility.kickWave, transform.position + transform.forward, transform.rotation);
        GetComponent<PlayerAbility>().ResetAbilityUse();

    }

    public void ResetAbilityUse(Vector3 originalScale, float time)
    {
        Debug.Log("try reset scale");
        transform.DOScale(originalScale, time);
    }
}
