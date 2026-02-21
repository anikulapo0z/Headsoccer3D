using UnityEngine;

public class CameraAnglesTester : MonoBehaviour
{

    public GameObject[] cameraObjects;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ActivateObject(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ActivateObject(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ActivateObject(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            ActivateObject(3);
    }

    void ActivateObject(int index)
    {
        for (int i = 0; i < cameraObjects.Length; i++)
        {
            if (cameraObjects[i] != null)
                cameraObjects[i].SetActive(i == index);
        }
    }
}
