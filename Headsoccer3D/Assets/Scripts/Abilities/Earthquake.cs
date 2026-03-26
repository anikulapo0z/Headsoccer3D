using Autodesk.Fbx;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class Earthquake : MonoBehaviour
{

    public LayerMask ground;
    public GameObject earthquakeRef;
    public float radius;
    public float aliveTime;

    public float yKick;
    public float ballKickForce;
    public float playerKickForce;


    float currentTime;
    GameObject obj;

    public void UseAbility()
    {
        currentTime = aliveTime;
        obj = Instantiate(earthquakeRef, transform.position, Quaternion.identity);
        obj.transform.localScale = new Vector3(radius, obj.transform.localScale.y, radius);
        StartCoroutine(EarthquakingInMyBoots());
    }


    IEnumerator EarthquakingInMyBoots()
    {
        Vector3 pos;
        RaycastHit hit;


        while (currentTime > 0) {
            if (Physics.Raycast(transform.position, -transform.up, out hit, 100, ground))
            {
                obj.transform.position = hit.transform.position;
            }
            currentTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
        yield return null;
    }

    public void OnTriggerEnter(Collider other)
    {
        Vector3 kickDirection;
        kickDirection = (other.transform.position - transform.position);

        kickDirection.y = 0f;
        kickDirection.Normalize();

        if (other.CompareTag("Ball") || other.CompareTag("FakeBall"))
        {

            other.GetComponent<SoccerBall>().LaunchAtDirection(kickDirection + (Vector3.up * yKick), ballKickForce);
        }

        PlayerController otherPlayer = other.GetComponent<PlayerController>();
        if (otherPlayer == null) return;

        otherPlayer.GetHitFromPlayer(playerKickForce, kickDirection);
    }

}
