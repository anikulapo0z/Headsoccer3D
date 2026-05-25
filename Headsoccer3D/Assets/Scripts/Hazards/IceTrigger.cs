using Unity.Cinemachine;
using UnityEngine;

public class IceTrigger : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("ice trigger");

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerIceController>().SetFrozen();
        }

        // splash sfx

    }



}
