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

    public float yInitialPos;



    private float minZ = -7.866985f;
    private float maxZ = 2.195376f;
    private float myNormalizedZPosition;

    public bool sinking = false;
    //public float startYPos;
    [SerializeField] float lerpSpeed;


    private void Start()
    {
        yInitialPos = transform.position.y;
        myNormalizedZPosition = Mathf.Clamp01((maxZ - transform.position.z) / (maxZ - minZ));
    }

    // Update is called once per frame
    void Update()
    {
        if (!sinking)
        {
            temp = waveSample.getWaveOffsetAndRotation(myNormalizedZPosition);
            offset = temp.x;
            sign = temp.y < 0 ? -1 : 1;
            rotationLerp = temp.y;

            Vector3 targetPosition = new Vector3(transform.position.x, yInitialPos + offset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.Euler(-90 + (maxRotation * rotationLerp), 0, 0);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, lerpSpeed * Time.deltaTime);
            //transform.Rotate(sign < 0 ? Vector3.left : Vector3.right, maxRotation * rotationLerp);
        }

    }
}
