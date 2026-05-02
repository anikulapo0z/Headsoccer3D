using UnityEngine;

public class SceneAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource crowdAmbience;
    [SerializeField] private AudioSource crowdCheers;
    [SerializeField] private AudioSource crowdGasp;
    
    [SerializeField] private AudioSource crowdBoos;
    [SerializeField] private AudioSource whistleSfx;
    [SerializeField] private AudioSource trainHornSfx;
    [SerializeField] private AudioSource[] trainPlayerImpactSfx;
    [SerializeField] private AudioSource trainAnnouncementSfx;
    [SerializeField] private AudioSource[] bellRingSfx;
    [SerializeField] private AudioSource bellBreakSfx;

    // general
    private void Start()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
        else
        {
            Debug.Log("Background music clip is not assigned");
        }
        if (crowdAmbience != null)
        {
            crowdAmbience.Play();
        }
        else
        {
            Debug.Log("Crowd ambience clip is not assigned");
        }
    }
    public void PlayCheersSfx()
    {
        if (crowdCheers.resource)
        {
            crowdCheers.Play();
        }
        else
        {
            Debug.Log("Crowd cheers SFX clip is not assigned");
        }
    }
    public void PlayGaspSfx()
    {
        if (crowdGasp.resource)
        {
            crowdGasp.Play();
        }
        else
        {
            Debug.Log("Crowd gasp SFX clip is not assigned");
        }
    }
    public void PlayWhistleSfx()
    {
        if (whistleSfx.resource)
        {
            whistleSfx.Play();
        }
        else
        {
            Debug.Log("Whistle SFX clip is not assigned");
        }
    }

    // bus
    public void PlayBusSfx()
    {
        if (busHornSfx.resource)
        {
            busSfx.Play();
        }
        else
        {
            Debug.Log("Bus SFX clip is not assigned");
        }
    }

    // train
    public void PlayTrainHornSfx()
    {
        if (trainHornSfx.resource)
        {
            trainHornSfx.Play();
        }
        else
        {
            Debug.Log("Train Horn SFX clip is not assigned");
        }
    }
    public void PlayTrainHornSfx()
    {
        if (trainHornSfx.resource)
        {
            trainHornSfx.Play();
        }
        else
        {
            Debug.Log("Train Horn SFX clip is not assigned");
        }
    }

    // bell
    public void PlayBellRingSfx()
    {
        if (bellRingSfx.resource)
        {
            bellRingSfx.Play();
        }
        else
        {
            Debug.Log("Bell Ring SFX clip is not assigned");
        }
    }
    public void PlayBellBreakSfx()
    {
        if (bellBreakSfx.resource)
        {
            bellBreakSfx.Play();
        }
        else
        {
            Debug.Log("Bell Break SFX clip is not assigned");
        }
    }
}