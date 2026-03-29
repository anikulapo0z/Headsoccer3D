using UnityEngine;
using DG.Tweening;

public class EmpoweredKick : MonoBehaviour
{
    public float empoweredKickStrength;
    public GameObject player;
    PlayerAbility playerAbility;


    private void Start()
    {
        playerAbility = GetComponent<PlayerAbility>();
    }

    public void UseAbility()
    {
        //Debug.LogError("try use ablity");
        GetComponent<PlayerController>().OnKick(false);

        //GameObject wave = Instantiate(playerAbility.kickWave, transform.position + transform.forward, transform.rotation);
        //wave.GetComponent<EmpoweredKickWave>().player = player;
        GetComponent<PlayerAbility>().ResetAbilityUse();

    }

    public void ResetAbilityUse(Vector3 originalScale, float time)
    {
        Debug.Log("try reset scale");
        transform.DOScale(originalScale, time);
    }
}
