using UnityEngine;

public class BusCrowdParallax : MonoBehaviour
{
    public float multiplier = 10f;
    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;

        Bounds b = mesh.bounds;
        b.extents *= multiplier;
        mesh.bounds = b;
    }
}
