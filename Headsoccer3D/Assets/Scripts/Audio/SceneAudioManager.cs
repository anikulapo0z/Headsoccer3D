using UnityEngine;

public class SceneAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource crowdAmbience;
    [SerializeField] private AudioSource crowdCheersSfx;
    [SerializeField] private AudioSource crowdBoosSfx;
    [SerializeField] private AudioSource confettiSfx;
    [SerializeField] private AudioSource whistleSfx;
    [SerializeField] private AudioSource countdownBeepSfx;

    [Header("Bus map")]
    [SerializeField] private AudioSource busSfx;
    [Header("Train map")]
    [SerializeField] private AudioSource trainHornSfx;
    [SerializeField] private AudioSource trainTracksSfx;
    [SerializeField] private AudioSource[] trainPlayerImpactSfx;
    [SerializeField] private AudioSource announcementSfx;
    [Header("Bell map")]
    [SerializeField] private AudioSource bellRingSfx;
    [SerializeField] private AudioSource bellBreakSfx;
    [SerializeField] private AudioSource chaosSequenceSfx;
    [Header("River map")]
    [SerializeField] private AudioSource[] waveSfx;
    [SerializeField] private AudioSource splashSfx;

    // general
    private void Start()
    {
        if (backgroundMusic && backgroundMusic.resource)
        {
            backgroundMusic.Play();
        }
        else
        {
            Debug.Log("Background music clip is not assigned");
        }
        if (crowdAmbience && crowdAmbience.resource)
        {
            crowdAmbience.Play();
        }
        else
        {
            Debug.Log("Crowd ambience clip is not assigned");
        }
    }
    public void PlayConfettiCheersSfx()
    {
        if (confettiSfx && confettiSfx.resource)
        {
            confettiSfx.Play();
        }
        else
        {
            Debug.Log("confetti SFX clip is not assigned");
        }
        if (crowdCheersSfx && crowdCheersSfx.resource)
        {
            crowdCheersSfx.Play();
        }
        else
        {
            Debug.Log("Crowd cheers SFX clip is not assigned");
        }
    }
    public void PlayWhistleSfx()
    {
        if (whistleSfx && whistleSfx.resource)
        {
            whistleSfx.Play();
        }
        else
        {
            Debug.Log("Whistle SFX clip is not assigned");
        }
    }
    public void PlayCountdownSfx()
    {
        if (countdownBeepSfx && countdownBeepSfx.resource)
        {
            countdownBeepSfx.Play();
        }
        else
        {
            Debug.Log("Countdown SFX clip is not assigned");
        }
    }

    // bus
    public void PlayBusSfx()
    {
        if (busSfx && busSfx.resource)
        {
            busSfx.Play();
        }
        else
        {
            Debug.Log("Bus SFX clip is not assigned");
        }
    }

    // train
    public void PlayTrainHornSfx(float pan)
    {
        if (trainHornSfx && trainHornSfx.resource)
        {
            trainHornSfx.panStereo = pan;
            trainHornSfx.Play();
        }
        else
        {
            Debug.Log("Train Horn SFX clip is not assigned");
        }
    }
    public void PlayTrainTracksSfx(float pan)
    {
        if (trainTracksSfx && trainTracksSfx.resource)
        {
            trainTracksSfx.panStereo = pan;
            trainTracksSfx.Play();
        }
        else
        {
            Debug.Log("Train Tracks SFX clip is not assigned");
        }
    }
    public void PlayTrainPlayerImpactSfx(float pan)
    {
        if (trainPlayerImpactSfx.Length > 0)
        {
            int randomIndex = Random.Range(0, trainPlayerImpactSfx.Length);
            AudioSource source = trainPlayerImpactSfx[randomIndex];
            if (source != null)
            {
                source.panStereo = pan;
                source.Play();
            }
        }
        else
        {
            Debug.Log("Train Player Impact SFX clips are not assigned.");
        }
    }
    public void PlayAnnouncementSfx(float pan)
    {
        if (announcementSfx && announcementSfx.resource)
        {
            announcementSfx.panStereo = pan; // -0.75 for left, 0.75 for right
            announcementSfx.Play();
        }
        else
        {   
            Debug.Log("Announcement SFX is not assigned.");
        }
    }

    // bell
    public void PlayBellRingSfx()
    {
        if (bellRingSfx && bellRingSfx.resource)
        {
            bellRingSfx.Play();
        }
        else
        {
            Debug.Log("Bell Ring SFX clips are not assigned.");
        }
    }
    public void PlayBellBreakSfx()
    {
        if (bellBreakSfx && bellBreakSfx.resource)
        {
            bellBreakSfx.Play();
        }
        else
        {
            Debug.Log("Bell Break SFX clip is not assigned");
        }
    }
    public void PlayChaosSfx()
    {
        if (chaosSequenceSfx && chaosSequenceSfx.resource)
        {
            chaosSequenceSfx.Play();
        }
        else
        {
            Debug.Log("chaos SFX clip is not assigned");
        }
    }
    // river splashSfx
    public void PlayWaveSfx()
    {
        if (waveSfx.Length > 0)
        {
            int randomIndex = Random.Range(0, waveSfx.Length);
            waveSfx[randomIndex].Play();
        }
        else
        {
            UnityEngine.Debug.Log("Wave SFX clips are not assigned.");
        }
    }
    public void PlayPlayerSplashSfx()
    {
        if (splashSfx && splashSfx.resource)
        {
            splashSfx.Play();
        }
        else
        {
            Debug.Log("slash SFX clip is not assigned");
        }
    }
    public void PlayBallSplashSfx()
    {
        if (splashSfx && splashSfx.resource)
        {
            splashSfx.Play();
        }
        else
        {
            Debug.Log("slash SFX clip is not assigned");
        }
    }
}