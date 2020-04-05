using UnityEngine;

public class CameraUI : MonoBehaviour //klasse på kameraet, som sørger for position mm.
{
    public static float zoom; //zoom skalar til camOffset
    public float camZoomSpeed;
    public Vector2 camRotSpeed;
    static public Vector3 camOffset = new Vector3(1,1,1); //offset ift objektet der skal følges
    public GameObject objectToFollow; //siger sig selv. bliver sat til partikkelgrænsen
    public static bool dontTurnCamera;
    bool startUp = true;
    bool startUpZoom = true;
    bool scaled = false;
    bool externalCamPosUpdate = false;
    Quaternion camAngle; //bruges til rotering af kameraet rundt om objektet

    void Update()
    {
        if(Scaler.scaled && !scaled){//til skaleringen lige før simuleringen
            zoom *= Scaler.publicScaler;
            scaled = true;
        }
        if(startUpZoom && MeshCentralicer.xScale != 0){//zoomer til objektets størrelse
            zoom = Mathf.Abs(MeshCentralicer.xScale)*2  ;
            startUpZoom = false;
        }
        if(camOffset.magnitude != zoom){//hvis camPositionen er outdated ift. zoom inputtet
            if(camOffset.magnitude == 0){//til starten
                camOffset = new Vector3(1,1,1);
            }
            camOffset = camOffset.normalized * zoom;//zoomer
            if(!Input.GetMouseButton(0) && !Input.GetMouseButton(3))//hvis det ikke er brugeren som flyttede kameraet
                externalCamPosUpdate = true;
        }
        if(((Input.GetMouseButton(0) && !dontTurnCamera) || (startUp && Mathf.Abs(MeshCentralicer.xScale) > 0) || Input.mouseScrollDelta.y != 0 || externalCamPosUpdate) && !UIHandler.waitingForFileExplorer){//når kameraet af diverse grunde skal ompdateres
            if(!(zoom + Input.mouseScrollDelta.y * camZoomSpeed < 0.5f))//zoomer igen
            zoom += Input.mouseScrollDelta.y * camZoomSpeed;

            if(!externalCamPosUpdate){
                camAngle = Quaternion.AngleAxis(camRotSpeed.x * Input.GetAxis("Mouse X"), Vector3.up);//flytter kameraet efter mussebevægelser
                camOffset = camAngle * camOffset;
                float camOffsetLength = camOffset.magnitude;
                camOffset.y -= camRotSpeed.y * Input.GetAxis("Mouse Y") * GameObject.FindGameObjectWithTag("Mesh").transform.localScale.x;
                camOffset = camOffset.normalized * camOffsetLength;
            }

            Vector3 camPos = objectToFollow.transform.position + camOffset;//opdaterer kamera pos
            transform.position = camPos;
            transform.LookAt(objectToFollow.transform);//drejer kameraet til at peje på figuren
            startUp = false;
            externalCamPosUpdate = false;
        }
        if(!Input.GetMouseButton(0)){
            dontTurnCamera = false;
        }
        
    }
}