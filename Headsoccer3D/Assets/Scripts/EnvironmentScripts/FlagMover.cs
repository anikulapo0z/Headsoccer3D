using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;



public class FlagMover : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public Transform leftTarget; // when object reaches this transform it will respawn
    public Transform rightSpawn; // spawn position
    //public float offset = 0.5f; // how far to the left of the leftTarget to consider "reached"

    [Header("Materials")]
    public Material[] materials; // materials to cycle through on respawn
    public int startMaterialIndex = 0;
    public Renderer flagRenderer; // optional, if not set will try to get from this object or children

    Renderer rend;
    int currentMaterialIndex = 0;

    void Start()
    {
        //rend = GetComponentInChildren<Renderer>();
        if (rend == null) rend = GetComponentInChildren<Renderer>();
        Debug.Log("Renderer found: " + (rend != null ? rend.name : "None"));
        currentMaterialIndex = Mathf.Clamp(startMaterialIndex, 0, Mathf.Max(0, (materials != null ? materials.Length - 1 : 0)));
        if (materials != null && materials.Length > 0 && rend != null)
        {
            flagRenderer.material = materials[currentMaterialIndex];
        }
        //transform.position = new Vector3(offset, transform.position.y, transform.position.z); 
    }
    void Update()
    {
        if (leftTarget == null || rightSpawn == null) return;

        // Move toward the left target
        transform.position = Vector3.MoveTowards(transform.position, leftTarget.position, speed * Time.deltaTime);

        // If reached the left target, snap to the right spawn and change material
        if (Vector3.Distance(transform.position, leftTarget.position) < 0.05f)
        {
            MoveFlag();
        }
    }

    private void MoveFlag(){
        transform.position = rightSpawn.position;

        if (materials != null && materials.Length > 0 && rend != null)
        {
            currentMaterialIndex = (currentMaterialIndex + 1) % materials.Length;
            flagRenderer.material = materials[currentMaterialIndex];
        }
    }
}
