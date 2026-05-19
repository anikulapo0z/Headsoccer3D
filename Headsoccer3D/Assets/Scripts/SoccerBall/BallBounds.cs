using UnityEngine;

public class BallBounds : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
            other.gameObject.transform.position = new Vector3(0, 3, 0);
    }

}
