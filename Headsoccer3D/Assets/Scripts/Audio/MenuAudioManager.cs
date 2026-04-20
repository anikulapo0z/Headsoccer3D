using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusicSfx;
    [SerializeField] private AudioSource ambienceSfx;

    void Start()
    {
        if (backgroundMusicSfx.resource != null)
        {
            backgroundMusicSfx.Play();
        }
        else if (backgroundMusicSfx.resource != null)
        {
            Debug.Log("No audio clip assigned to backgroundMusicSfx on " + gameObject.name);
        }
        if (ambienceSfx.resource != null)
        {
            ambienceSfx.Play();
        }
        else if (ambienceSfx.resource != null)
        {
            Debug.Log("No audio clip assigned to ambienceSfx on " + gameObject.name);
        }
    }
}
