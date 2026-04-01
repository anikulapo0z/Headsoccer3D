using System.Collections;
using UnityEngine;

public class Earthquake : MonoBehaviour
{

    public LayerMask ground;
    public GameObject earthquakeRef;
    public GameObject player;
    public float radius;
    public float aliveTime;

    public float yKick;
    public float ballKickForce;
    public float playerKickForce;

    public AudioSource source;


    float currentTime;
    GameObject obj;
    bool usedEarthquake = false;

    public void UseAbility()
    {
        if(usedEarthquake) return;
        usedEarthquake = true;
        currentTime = aliveTime;
        obj = Instantiate(earthquakeRef, transform.position, Quaternion.identity);
        obj.transform.localScale = new Vector3(radius, obj.transform.localScale.y, radius);

        EarthquakeObject eqo = obj.GetComponent<EarthquakeObject>();
        eqo.yKick = yKick;
        eqo.ballKickForce = ballKickForce;
        eqo.playerKickForce = playerKickForce;
        eqo.controllingPlayer = player;

        StartCoroutine(EarthquakingInMyBoots());
    }


    IEnumerator EarthquakingInMyBoots()
    {
        source.Play();
        Vector3 pos;
        RaycastHit hit;


        while (currentTime > 0) {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, ground))
            {
                obj.transform.position = hit.point;
            }
            currentTime -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(obj);
        GetComponent<PlayerAbility>().ResetAbilityUse();
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, -transform.up * 100);
    }

}