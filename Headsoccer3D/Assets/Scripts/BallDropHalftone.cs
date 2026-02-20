using UnityEngine;

public class BallDropHalftone : MonoBehaviour
{
    [SerializeField] private Transform ballTransform;

    private Renderer targetRenderer;
    private Material targetMaterial;
    //if we change in the shader graph, we wil have to here too. 
    private static readonly int BallPositionID = Shader.PropertyToID("_Ball_Position");

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        targetMaterial = targetRenderer.material;
    }

    private void Update()
    {
        //incase we destory the ball
        if (targetMaterial != null && ballTransform != null)
        {
            targetMaterial.SetVector(BallPositionID, ballTransform.position);
        }
    }
}
