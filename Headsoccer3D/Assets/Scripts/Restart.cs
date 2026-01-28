using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    [SerializeField] string sceneName;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerInputHolder.Instance.KillSingletons();

            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
