using System.Collections.Generic;
using UnityEngine;

public class SetupBusMap : MonoBehaviour
{
    [SerializeField] GameObject[] flippers;


    public void SetupBusGame(List<GameObject> players, List<PlayerInputController> inputControllers)
    {

        if (players.Count == 2)
        {
            inputControllers[0].SetControlledObject(flippers[0].GetComponentInChildren<PinballFlipper>(), flippers[0], false);
            inputControllers[0].SetControlledObject(flippers[1].GetComponentInChildren<PinballFlipper>(), flippers[1], false);
            players[0].GetComponent<PlayerGroundMarker>().controlledFlippers.Add(flippers[0]);
            players[0].GetComponent<PlayerGroundMarker>().controlledFlippers.Add(flippers[1]);

            inputControllers[1].SetControlledObject(flippers[2].GetComponentInChildren<PinballFlipper>(), flippers[2], false);
            inputControllers[1].SetControlledObject(flippers[3].GetComponentInChildren<PinballFlipper>(), flippers[3], false);
            players[1].GetComponent<PlayerGroundMarker>().controlledFlippers.Add(flippers[2]);
            players[1].GetComponent<PlayerGroundMarker>().controlledFlippers.Add(flippers[3]);

        }
        else if (players.Count == 4)
        {

            inputControllers[0].SetControlledObject(flippers[0].GetComponentInChildren<PinballFlipper>(), flippers[0], false);
            players[0].GetComponent<PlayerGroundMarker>().controlledFlippers.Add(flippers[0]);

            inputControllers[1].SetControlledObject(flippers[1].GetComponentInChildren<PinballFlipper>(), flippers[1], false);
            players[1].GetComponent<PlayerGroundMarker>().controlledFlippers.Add(flippers[1]);

            inputControllers[2].SetControlledObject(flippers[2].GetComponentInChildren<PinballFlipper>(), flippers[2], false);
            players[2].GetComponent<PlayerGroundMarker>().controlledFlippers.Add(flippers[2]);

            inputControllers[3].SetControlledObject(flippers[3].GetComponentInChildren<PinballFlipper>(), flippers[3], false);
            players[3].GetComponent<PlayerGroundMarker>().controlledFlippers.Add(flippers[3]);


        }


        foreach (GameObject p in players)
        {

            p.GetComponent<PlayerGroundMarker>().controllingFlipper = true;


        }


    }
}
