using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInputHolder : MonoBehaviour
{
    public static PlayerInputHolder Instance;
    public List<PlayerInputController> playerList = new List<PlayerInputController>();
    public GameObject scene;



    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
    public void KillSingletons()
    {
        foreach (var i in playerList)
        {
            Destroy(i.gameObject);
        }
        if(scene != null)
            Destroy(scene);

        Destroy(gameObject);
    }
}
