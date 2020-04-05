using UnityEngine;

public class MeshCentralicer : MonoBehaviour //centraliseringsfunktion til meshen
{
    public static float xScale;
    public static float scaler = 2;
    public static void Centralise()
    {
        GameObject meshObject = GameObject.FindGameObjectWithTag("Mesh");
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh; //finder mesh objektet

        Vector3[] verticies = mesh.vertices;
        long verticeSumX = 0;
        long verticeSumY = 0;
        long verticeSumZ = 0;
        foreach(var e in verticies){
            verticeSumX += (int) (e.x + 0.5f);
            verticeSumY += (int) (e.y + 0.5f);
            verticeSumZ += (int) (e.z + 0.5f);
        }
        float floatLimit = Mathf.Pow(10,38);
        Vector3 avgPos;//gennemsnitspos i mesh vertice koordinaterne
        if(verticeSumX > floatLimit && verticeSumY > floatLimit && verticeSumZ > floatLimit)
            avgPos = new Vector3(verticeSumX/verticies.Length, verticeSumY/verticies.Length, verticeSumZ/verticies.Length);
        else{
            float floatVerticeSumX = 0;
            float floatVerticeSumY = 0;
            float floatVerticeSumZ = 0;
            foreach(var e in verticies){
                floatVerticeSumX += e.x*meshObject.transform.localScale.x;
                floatVerticeSumY += e.y*meshObject.transform.localScale.y;
                floatVerticeSumZ += e.z*meshObject.transform.localScale.z;
            }
            avgPos = new Vector3(floatVerticeSumX/verticies.Length, floatVerticeSumY/verticies.Length, floatVerticeSumZ/verticies.Length);
        }
        meshObject.transform.position = new Vector3();
        meshObject.transform.Translate(-avgPos);
        

        int highestVal = 0;
        int lowestVal = 0;
        int yLowestVal = 0;
        for(int i = 0; i < verticies.Length; i++){
            if(verticies[i].x > verticies[highestVal].x){
                highestVal = i;
            }
            if(verticies[i].x < verticies[lowestVal].x){
                lowestVal = i;
            }
            if(verticies[i].y < verticies[yLowestVal].y){
                yLowestVal = i;
            }
        }
        xScale = verticies[highestVal].x*meshObject.transform.localScale.x - verticies[lowestVal].x*meshObject.transform.localScale.x;
        xScale *= scaler; //laver en skalar fx til kamera, så den kan zoome ift. meshstørrelsen
    }
}