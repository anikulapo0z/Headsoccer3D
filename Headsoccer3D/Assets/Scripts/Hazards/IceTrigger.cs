using Unity.Cinemachine;
using UnityEngine;

public class IceTrigger : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerIceController>().SetFrozen();
        }
        else if (other.CompareTag("Ball"))
            other.GetComponent<BallIceController>().SetFrozen();
    }



}
