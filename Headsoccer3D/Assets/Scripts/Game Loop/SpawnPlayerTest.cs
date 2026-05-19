using UnityEngine;

public class SpawnPlayerTest : MonoBehaviour
{
    LoadTestScene lts;
    public void SpawnPlayer()
    {
        lts = FindObjectOfType<LoadTestScene>();
        lts.CreatePlayer();
    }
}
