using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MenuMusic : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] float fadeInRate;
    [SerializeField] float fadeOutDuration;

    public void Start()
    {
        StartCoroutine(FadeInRoutine(source, fadeInRate));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine(source, fadeOutDuration));
    }

    IEnumerator FadeInRoutine(AudioSource audioSource, float t)
    {
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 1)
        {
            audioSource.volume += t * Time.deltaTime;
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


}
