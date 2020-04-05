using UnityEngine;

public class BorderMesh : MonoBehaviour
{
    public static Mesh mesh; //giver en mesh kube på størrelse med chunksize variablen (skal bruges til en sammenstødsmesh for partiklerne)
    void Start()
    {
        float chunkSize = densityCalcParticleController.chunckSize; 
        transform.localScale = new Vector3(chunkSize,chunkSize,chunkSize);
        mesh = GetComponent<MeshFilter>().mesh;
        var verts = mesh.vertices;
        for(int i = 0; i < verts.Length; i++){
            verts[i].x *= transform.localScale.x;
            verts[i].y *= transform.localScale.y;
            verts[i].z *= transform.localScale.z;
        }
        mesh.vertices = verts;
    }
}