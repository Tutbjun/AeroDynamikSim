using UnityEngine;
using System.IO;

public class MainMesh : MonoBehaviour //klasse til den mesh der bliver simuleret
{
    public Mesh mesh;
    public static bool scaled = false;
    public static GameObject meshDuplicate;
    public static float[] area;
    public static Gradient preasureGradient;
    public static float maxPreassure;
    public static float[] momentums;
    public static bool meshColored = false;
    void Update()
    {
        preasureGradient = Inputs.publicPreasureGradient;
        if(EventHandler.evaluateSim && !meshColored){
            float[] preassures = new float[mesh.triangles.Length/3]; //trykudregning
            for(int i = 0; i < momentums.Length; i++){
                float force = momentums[i] * Inputs .momentumToSIFactor / Inputs.publicDensityCalcTime * 2;
                float picometer = Mathf.Pow(10,-12);
                float Area = area[i] * Mathf.Pow(picometer,2);
                preassures[i] = force / Area;
                if(preassures[i] > maxPreassure)
                    maxPreassure = preassures[i];
            }
            using (StreamWriter sw = new StreamWriter("preassureData.dat"))//tryk bliver overført til fil
            foreach(var e in preassures){
                sw.Write(e);
                sw.Write(",");
            }
            Color[] meshColor = new Color[meshDuplicate.GetComponent<MeshFilter>().mesh.vertices.Length];
            for(int i = 0; i < meshColor.Length; i++){                                                      //farvelægning :P
                float input = 0;
                int inputs = 0;
                for(int i2 = 0; i2 < mesh.triangles.Length; i2++){
                    if (mesh.triangles[i2] == i){
                        input += preassures[(int)i2/3]/maxPreassure;
                        inputs++;
                    }
                }
                input /= (float)inputs;
                if(input <= 1 && input >= 0)
                    meshColor[i] = preasureGradient.Evaluate(input);
            }
            meshDuplicate.GetComponent<MeshFilter>().mesh.colors = meshColor;
            meshColored = true;
        }
        if(!scaled && Scaler.scaled){
            meshDuplicate = Instantiate(GameObject.FindGameObjectWithTag("Mesh"));//laver visuel duplikant, da originalen visuelt bliver skaleret
            var duplicateScript1 = meshDuplicate.GetComponent<MainMesh>();
            GameObject.Destroy(duplicateScript1);
            var duplicateScript2 = meshDuplicate.GetComponent<BareMesh>();
            GameObject.Destroy(duplicateScript2);
            for(int i = 0; i < meshDuplicate.GetComponent<MeshFilter>().mesh.vertices.Length; i++){
                meshDuplicate.GetComponent<MeshFilter>().mesh.vertices[i].x /= transform.localScale.x;
                meshDuplicate.GetComponent<MeshFilter>().mesh.vertices[i].y /= transform.localScale.y;
                meshDuplicate.GetComponent<MeshFilter>().mesh.vertices[i].z /= transform.localScale.z;
            }
            scaled = true;
            mesh = GetComponent<MeshFilter>().mesh;//forstørrer meshen, da partiklerne er store
            var verts = mesh.vertices;
            for(int i = 0; i < verts.Length; i++){
                verts[i].x *= transform.localScale.x;
                verts[i].y *= transform.localScale.y;
                verts[i].z *= transform.localScale.z;
                verts[i] += transform.position;
            }
            mesh.vertices = verts;
            var meshVisibility = GetComponent<MeshRenderer>();
            GameObject.Destroy(meshVisibility);//gør den usynlig, da den visuelt er skaleret

            area = new float[mesh.triangles.Length/3];//udregner arealer af trekanter til senere brug til udregning af tryk
            for(int i = 0; i < mesh.triangles.Length/3; i++){
                Vector3[] points = new Vector3[3];
                points[0] = mesh.vertices[mesh.triangles[i*3]];
                points[1] = mesh.vertices[mesh.triangles[i*3+1]];
                points[2] = mesh.vertices[mesh.triangles[i*3+2]];
                float a = (points[1] - points[0]).magnitude;
                float b = (points[2] - points[0]).magnitude;
                float v = Mathf.Acos(Vector3.Dot((points[1] - points[0]),(points[2] - points[0]))/((points[1] - points[0]).magnitude*(points[2] - points[0]).magnitude));
                area[i] = a*b*Mathf.Sin(v)/2;
            }
            momentums = new float[mesh.triangles.Length/3];
        }
    }
}