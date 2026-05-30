using Unity.Cinemachine;
using UnityEngine;

public class SplashTrigger : MonoBehaviour
{
    [SerializeField] private SceneAudioManager audioManager;

    public void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            audioManager.PlayPlayerSplashSfx();
        }
        else if (other.CompareTag("Ball"))
        {
            audioManager.PlayPlayerSplashSfx();
        }
    }
}