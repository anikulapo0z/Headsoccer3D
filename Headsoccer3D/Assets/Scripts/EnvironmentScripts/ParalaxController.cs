using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;



public class ParalaxController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public Transform target; // when object reaches this transform it will respawn
    public Transform spawn; // spawn position
    //public float offset = 0.5f; // how far to the left of the target to consider "reached"

   

    void Start()
    {
        //rend = GetComponentInChildren<Renderer>();
        //transform.position = new Vector3(offset, transform.position.y, transform.position.z); 
    }
    void Update()
    {
        if (target == null || spawn == null) return;

        // Move toward the left target but only on the x axis
        transform.position = Vector3.MoveTowards(transform.position , target.position, speed * Time.deltaTime);

        // If reached the left target, snap to the right spawn and change material
        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            MoveFlag();
        }
    }

    private void MoveFlag(){
        transform.position = spawn.position;
    }
}
