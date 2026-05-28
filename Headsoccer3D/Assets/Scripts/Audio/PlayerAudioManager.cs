using System.Diagnostics;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] kickBallSfx;
    [SerializeField] private AudioSource[] chargeSfx;
    [SerializeField] private AudioSource[] kickWhooshSfx;
    [SerializeField] private AudioSource[] hitBallSfx;
    [SerializeField] private AudioSource[] footstepsSfx;
    [SerializeField] private AudioSource jumpSfx;
    [SerializeField] private AudioSource cloneKickSfx;
    [SerializeField] private AudioSource cloneJumpSfx;
    [SerializeField] private AudioSource landSfx;
    [SerializeField] private AudioSource pickupSfx;
    [SerializeField] private AudioSource empoweredKickSfx;
    [SerializeField] private AudioSource multiBallSfx;
    [SerializeField] private AudioSource freezeSfx;
    [SerializeField] private AudioSource[] chipIceSfx;
    [SerializeField] private AudioSource breakIceSfx;

    public void PlayKickBallSfx()
    {
        if (kickBallSfx.Length > 0)
        {
            int randomIndex = Random.Range(0, kickBallSfx.Length);
            kickBallSfx[randomIndex].Play();
        }
        else
        {
            UnityEngine.Debug.Log("Kick Ball SFX clips are not assigned.");
        }
    }
    public void PlayChargeSfx()
    {
        if (chargeSfx.Length > 0)
        {
            int randomIndex = Random.Range(0, chargeSfx.Length);
            chargeSfx[randomIndex].Play();
        }
        else
        {
            UnityEngine.Debug.Log("Charge SFX clips are not assigned.");
        }
    }
    public void PlayWhooshSfx()
    {
        if (kickWhooshSfx.Length > 0)
        {
            int randomIndex = Random.Range(0, kickWhooshSfx.Length);
            kickWhooshSfx[randomIndex].Play();
        }
        else
        {
            UnityEngine.Debug.Log("Kick Whoosh SFX clips are not assigned.");
        }
    }
    public void PlayHitBall()
    {
        if (hitBallSfx.Length > 0)
        {
            int randomIndex = Random.Range(0, hitBallSfx.Length);
            hitBallSfx[randomIndex].Play();
        }
        else
        {
            UnityEngine.Debug.Log("Hit Ball SFX clips are not assigned.");
        }
    }
    public void PlayJumpSfx()
    {
        if (jumpSfx && jumpSfx.resource)
        {
            jumpSfx.Play();
        }
        else
        {
            UnityEngine.Debug.Log("Jump SFX clip is not assigned.");
        }
        if (GetComponent<PlayerAbility>().currentAbility == AbilityTrigger.AbilityTypes.ShadowClone)
        {
            UnityEngine.Debug.Log("shadow clone yes");
            if (cloneJumpSfx && cloneJumpSfx.resource)
            {
                cloneJumpSfx.Play();
            }
            else
            {
                UnityEngine.Debug.Log("Clone Jump SFX clip is not assigned.");
            }
        }
    }
    public void PlayLandSfx()
    {
        if (landSfx && landSfx.resource)
        {
            landSfx.Play();
        }
        else
        {
            UnityEngine.Debug.Log("Land SFX clip is not assigned.");
        }
    }

    public void PlayFootstepSfx()
    {
        if (footstepsSfx.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepsSfx.Length);
            footstepsSfx[randomIndex].Play();
        }
        else
        {
            UnityEngine.Debug.Log("Footstep SFX clips are not assigned.");
        }
    }
    public void PlayPickupSfx()
    {
        if (pickupSfx && pickupSfx.resource)
        {
            pickupSfx.Play();
        }
        else
        {
            UnityEngine.Debug.Log("empoweredKickSfx is not assigned.");

        }
    }
    public void PlayEmpoweredKickKickSfx()
    {
        if (empoweredKickSfx)
        {
            if (empoweredKickSfx.resource)
            {
                empoweredKickSfx.Play();
            }
            else
            {
                UnityEngine.Debug.Log("empoweredKickSfx clip is not assigned.");
            }
        }
        else
        {
            UnityEngine.Debug.Log("empoweredKickSfx is not assigned.");

        }
    }
    public void PlayMultiBallSfx()
    {
        if (multiBallSfx)
        {
            if (multiBallSfx.resource)
            {
                multiBallSfx.Play();
            }
            else
            {
                UnityEngine.Debug.Log("multiBallSfx clip is not assigned.");
            }
        }
        else
        {
            UnityEngine.Debug.Log("multiBallSfx is not assigned.");

        }

    }

    //river map
    public void PlayPlayerFreezeSfx()
    {
        if (freezeSfx && freezeSfx.resource)
        {
            freezeSfx.Play();
        }
        else
        {
            UnityEngine.Debug.Log("Freeze SFX clip is not assigned");
        }
    }
    public void PlayChipIceSfx()
    {
        if (chipIceSfx.Length > 0)
        {
            int randomIndex = Random.Range(0, chipIceSfx.Length);
            chipIceSfx[randomIndex].Play();
        }
        else
        {
            UnityEngine.Debug.Log("Chip Ice SFX clips are not assigned.");
        }
    }
    public void PlayBreakIceSfx()
    {
        if (breakIceSfx && breakIceSfx.resource)
        {
            breakIceSfx.Play();
        }
        else
        {
            UnityEngine.Debug.Log("Break Ice SFX clip is not assigned");
        }
    }
}
