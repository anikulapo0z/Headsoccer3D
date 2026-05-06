using System.Collections;
using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] float fadeInRate;
    [SerializeField] float fadeOutDuration;
    [SerializeField] float maxVolume;
    [SerializeField] private AudioSource characterJoinSfx;
    [SerializeField] private AudioSource uiBackSfx;
    
    void Start()
    {
        if (backgroundMusic.resource != null)
        {
            StartCoroutine(FadeInRoutine(backgroundMusic, fadeInRate));
        }
        else if (backgroundMusic.resource != null)
        {
            Debug.Log("No audio clip assigned to backgroundMusic on " + gameObject.name);
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine(backgroundMusic, fadeOutDuration));
    }

    IEnumerator FadeInRoutine(AudioSource source, float duration)
    {
        source.volume = 0;
        source.Play();

        while (source.volume < maxVolume)
        {
            source.volume += duration * Time.deltaTime;
            yield return null;
        }

    }

    IEnumerator FadeOutRoutine(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
    }

    public void PlayCharacterJoinSfx()
    {
        if(characterJoinSfx)
        {
            if (characterJoinSfx.resource)
            {
                characterJoinSfx.Play();
            }
            else
            {
                Debug.Log("characterJoin SFX clip is not assigned.");
            }
        }
        else
        {
            Debug.Log("characterJoin SFX is not assigned.");

        }
    }
    
    public void PlayUIBackSfx()
    {
        if(uiBackSfx)
        {
            if (uiBackSfx.resource)
            {
                uiBackSfx.Play();
            }
            else
            {
                Debug.Log("uiBack SFX clip is not assigned.");
            }
        }
        else
        {
            Debug.Log("uiBack SFX is not assigned.");

        }
    }
}
