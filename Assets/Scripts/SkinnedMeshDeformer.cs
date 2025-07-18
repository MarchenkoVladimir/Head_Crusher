using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SkinnedMeshDeformer : MonoBehaviour
{
    private SkinnedMeshRenderer smr;
    private Mesh bakedMesh;
    private Vector3[] originalVertices;
    private Vector3[] currentVertices;
    private bool isRecovering = false;

    public float radius = 0.3f;
    public float intensity = 0.1f;
    public float recoverSpeed = 5f;

    void Start()
    {
        smr = GetComponent<SkinnedMeshRenderer>();
        bakedMesh = new Mesh();
        smr.BakeMesh(bakedMesh);

        originalVertices = bakedMesh.vertices;
        currentVertices = (Vector3[])originalVertices.Clone();
    }

    public void Deform(Vector3 worldHit)
    {
        smr.BakeMesh(bakedMesh); // обновляем позу

        Vector3 localHit = transform.InverseTransformPoint(worldHit);
        currentVertices = bakedMesh.vertices;

        for (int i = 0; i < currentVertices.Length; i++)
        {
            float dist = Vector3.Distance(currentVertices[i], localHit);
            if (dist < radius)
            {
                float falloff = 1f - dist / radius;
                Vector3 dir = (currentVertices[i] - localHit).normalized;
                currentVertices[i] -= dir * intensity * falloff;
            }
        }

        Mesh newMesh = new Mesh();
        newMesh.vertices = currentVertices;
        newMesh.triangles = bakedMesh.triangles;
        newMesh.normals = bakedMesh.normals;
        newMesh.uv = bakedMesh.uv;

        smr.sharedMesh = newMesh;

        if (!isRecovering)
            StartCoroutine(Recover());
    }

    IEnumerator Recover()
    {
        isRecovering = true;

        while (true)
        {
            bool done = true;
            for (int i = 0; i < currentVertices.Length; i++)
            {
                currentVertices[i] = Vector3.Lerp(currentVertices[i], originalVertices[i], Time.deltaTime * recoverSpeed);
                if ((currentVertices[i] - originalVertices[i]).sqrMagnitude > 0.0001f)
                    done = false;
            }

            Mesh newMesh = new Mesh();
            newMesh.vertices = currentVertices;
            newMesh.triangles = bakedMesh.triangles;
            newMesh.normals = bakedMesh.normals;
            newMesh.uv = bakedMesh.uv;

            smr.sharedMesh = newMesh;

            if (done) break;
            yield return null;
        }

        isRecovering = false;
    }
}