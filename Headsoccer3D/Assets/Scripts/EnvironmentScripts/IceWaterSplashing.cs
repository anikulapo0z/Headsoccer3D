using UnityEngine;

public class IceWaterSplashing : MonoBehaviour
{
    public GameObject splashVFX_IN;
    public GameObject splashVFX_OUT;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag.Contains("Ball"))
        {
            Instantiate(other.transform.position.y > transform.position.y ? splashVFX_IN : splashVFX_OUT
                                ,new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z)
                                , Quaternion.identity);
        }
    }
}
