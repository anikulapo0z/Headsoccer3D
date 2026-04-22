using System;
using UnityEngine;

public class IceShard : MonoBehaviour
{
    //public bool console;
    [SerializeField] WaterWaveSample waveSample;

    [SerializeField] float maxRotation = 30f;

    private float offset = 0;
    private float rotationLerp = 0;
    private float sign = -1;
    Vector2 temp;

    private float yInitialPos;



    private float minZ = -7.866985f;
    private float maxZ = 2.195376f;
    private float myNormalizedZPosition;

    private void Start()
    {
        yInitialPos = transform.position.y;
        myNormalizedZPosition = Mathf.Clamp01((maxZ - transform.position.z) / (maxZ - minZ));
    }

    // Update is called once per frame
    void Update()
    {
        temp = waveSample.getWaveOffsetAndRotation(myNormalizedZPosition);
        offset = temp.x;
        sign = temp.y < 0 ? -1 : 1;
        rotationLerp = temp.y;

        transform.position = new Vector3(transform.position.x, yInitialPos + offset , transform.position.z);
        transform.localEulerAngles = new Vector3(-90 + ( maxRotation * rotationLerp), 0, 0);
        //transform.Rotate(sign < 0 ? Vector3.left : Vector3.right, maxRotation * rotationLerp);
    }
}
