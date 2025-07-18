using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    private Mesh deformingMesh;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;
    private float deformTimer;
    private float deformDuration = 0.4f;
    private bool isDeforming;

    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = deformingMesh.vertices;
    }

    public void Deform(Vector3 worldPoint, float radius, float intensity)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            float dist = Vector3.Distance(displacedVertices[i], localPoint);
            if (dist < radius)
            {
                float falloff = 1f - (dist / radius);
                Vector3 offset = (displacedVertices[i] - localPoint).normalized * intensity * falloff;
                displacedVertices[i] += offset;
            }
        }

        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
        deformTimer = 0f;
        isDeforming = true;
    }

    void Update()
    {
        if (!isDeforming) return;

        deformTimer += Time.deltaTime;
        float t = deformTimer / deformDuration;
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            displacedVertices[i] = Vector3.Lerp(displacedVertices[i], originalVertices[i], t);
        }

        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();

        if (t >= 1f) isDeforming = false;
    }
}