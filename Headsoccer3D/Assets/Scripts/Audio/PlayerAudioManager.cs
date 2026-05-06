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
    [SerializeField] private AudioSource landSfx;
    [SerializeField] private AudioSource pickupSfx;
    [SerializeField] private AudioSource empoweredKickSfx;
    [SerializeField] private AudioSource multiBallSfx;

    public void PlayKickBallSfx()
    {
        if (kickBallSfx.Length > 0)
        {
            int randomIndex = Random.Range(0, kickBallSfx.Length);
            kickBallSfx[randomIndex].Play();
        }
        else
        {
            Debug.Log("Kick Ball SFX clips are not assigned.");
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
            Debug.Log("Charge SFX clips are not assigned.");
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
            Debug.Log("Kick Whoosh SFX clips are not assigned.");
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
            Debug.Log("Hit Ball SFX clips are not assigned.");
        }
    }
    public void PlayJumpSfx()
    {
        if (jumpSfx.resource)
        {
            jumpSfx.Play();
        }
        else
        {
            Debug.Log("Jump SFX clip is not assigned.");
        }
    }
    public void PlayLandSfx()
    {
        if (landSfx.resource)
        {
            landSfx.Play();
        }
        else
        {
            Debug.Log("Land SFX clip is not assigned.");
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
            Debug.Log("Footstep SFX clips are not assigned.");
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
            Debug.Log("empoweredKickSfx is not assigned.");

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
                Debug.Log("empoweredKickSfx clip is not assigned.");
            }
        }
        else
        {
            Debug.Log("empoweredKickSfx is not assigned.");

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
                Debug.Log("multiBallSfx clip is not assigned.");
            }
        }
        else
        {
            Debug.Log("multiBallSfx is not assigned.");

        }

    }
}
