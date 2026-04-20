using UnityEngine;

public class SceneAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource crowdAmbience;
    [SerializeField] private AudioSource whistleSfx;

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
}