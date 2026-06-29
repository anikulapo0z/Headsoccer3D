using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHolder : MonoBehaviour
{
    public static PlayerInputHolder Instance;
    public List<PlayerInputController> playerList = new List<PlayerInputController>();
    public GameObject scene;
    public InputActionAsset sourceInputActions;
    public string actionMapName;

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public bool IsDeviceAssigned(InputDevice device)
    {
        foreach (var p in playerList)
            if (p != null && p.IsConnected && p.AssignedDevice == device)
                return true;
        return false;
    }

    public void KillSingletons()
    {
        foreach (var i in playerList)
            if (i != null) Destroy(i.gameObject);
        playerList.Clear();
        if (scene != null) Destroy(scene);
        Instance = null;
        Destroy(gameObject);
    }
}