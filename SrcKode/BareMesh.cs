using UnityEngine;

public class BareMesh : MonoBehaviour //offentliggører bare meshen på en måde, hvor man så ikke behøver at skrive BareMesh.GetComponent<MeshFilter>().mesh men i stedet BareMesh.mesh
{
    public static Mesh mesh;
    bool firstTry = true;
    void Update()
    {
        if(firstTry){
            mesh = GetComponent<MeshFilter>().mesh;
            firstTry = false;
        }
    }
}