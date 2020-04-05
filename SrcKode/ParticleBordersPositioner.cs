using UnityEngine;

public class ParticleBordersPositioner : MonoBehaviour
{
    static public Vector3 planePos;
    static public Vector3 planeScale;
    static public Quaternion planeRot;
    bool startUp = true;
    static public Vector3 dir;
    static public Vector3 myScale;

    void Update()
    {
        myScale = transform.localScale;
        if(startUp && MeshCentralicer.xScale != 0){
            transform.localScale = new Vector3(MeshCentralicer.xScale,MeshCentralicer.xScale,MeshCentralicer.xScale);
            startUp = false;
        }
        planeRot = transform.rotation;
        Vector3 zPos;
        zPos = new Vector3(0,0, - transform.localScale.z/2);
        planePos = transform.position;
        planePos = planeRot * zPos + planePos;
        planeScale = new Vector3(transform.localScale.x,transform.localScale.y,0.01f);
        dir = new Vector3(0,0,1);
        dir = planeRot * dir;
        dir = dir.normalized;
    }
}