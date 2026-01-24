using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTestScene : MonoBehaviour
{
    [SerializeField] string testSceneName;
    [SerializeField] GameObject player;
    PlayerInputController playerInputController;
    [SerializeField] Vector3 spawnPosition;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(testSceneName);
    }
    public void CreatePlayer()
    {
        GameObject t = Instantiate(player, spawnPosition, Quaternion.identity);
        PlayerController p = t.GetComponent<PlayerController>();
        playerInputController = FindObjectOfType<PlayerInputController>();
        playerInputController.SetControlledObject(p);
    }

}
