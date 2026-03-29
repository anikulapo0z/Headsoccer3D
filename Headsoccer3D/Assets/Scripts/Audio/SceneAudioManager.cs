using UnityEngine;

public class SceneAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource crowdAmbience;

    private void Start()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
        else
        {
            Debug.Log("Background music clip is not assigned.");
        }
        if (crowdAmbience != null)
        {
            crowdAmbience.Play();
        }
        else
        {
            Debug.Log("Crowd ambience clip is not assigned.");
        }
    }
}
