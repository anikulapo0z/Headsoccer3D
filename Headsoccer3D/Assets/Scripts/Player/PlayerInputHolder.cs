using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInputHolder : MonoBehaviour
{
    public static PlayerInputHolder Instance;
    public List<PlayerInputController> playerList = new List<PlayerInputController>();

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // kill duplicates
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
}
