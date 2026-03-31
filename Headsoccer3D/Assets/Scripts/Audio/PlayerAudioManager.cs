using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource kickSfx;
    [SerializeField] private AudioSource jumpSfx;
    [SerializeField] private AudioSource landSfx;
    [SerializeField] private AudioSource getHitSfx;

    [SerializeField] private AudioSource[] kickBallSfx;
    [SerializeField] private AudioSource[] footstepsSfx;

    public void PlayKickSfx()
    {
        if(kickSfx)
        {
            if (kickSfx.resource)
            {
                kickSfx.Play();
            }
            else
            {
                Debug.Log("Kick SFX clip is not assigned.");
            }
        }
        else
        {
            Debug.Log("Kick SFX is not assigned.");

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
    public void PlayGetHitSfx()
    {
        if (getHitSfx.resource)
        {
            getHitSfx.Play();
        }
        else
        {
            Debug.Log("Get Hit SFX clip is not assigned.");
        }
    }

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
}
