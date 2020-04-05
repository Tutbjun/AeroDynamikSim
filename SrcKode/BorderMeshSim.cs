using UnityEngine;

public class BorderMeshSim : MonoBehaviour//klasse til partikkel grænsen i simuleringen
{
    public static Mesh mesh;
    public static bool starting = true;
    void Update()
    {
        if(starting && Scaler.scaled){//eksporterer mesh kube på størrelse med chunksize variablen
            starting =  false;
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
}