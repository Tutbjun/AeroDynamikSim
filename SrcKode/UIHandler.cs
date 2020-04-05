using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dummiesman;//library til runtime import af .obj filer
using System.IO;
using SimpleFileBrowser;//stifinder library

public class UIHandler : MonoBehaviour //klasse til opsætning og styring af UI elementer
{
    public GameObject xInput;
    public GameObject yInput;
    public GameObject zInput;
    public GameObject translate;
    public GameObject rotate;
    public GameObject scale; 
    public GameObject startButton;
    public GameObject airSpeedField;
    public GameObject simulationDurationField;
    public GameObject airSpeedText;
    public GameObject simulationDurationText;
    public GameObject addObject;
    public GameObject targetPreassureText;
    public GameObject targetPreassureField;
    public static bool translateTriggered;
    public static bool rotateTriggered;
    public static bool scaleTriggered;
    public static Vector3 transformVal;

    void Start(){//sætter bare file explorer op
		fileExplorerSetup();
    }
    void Update()
    {
        position(); 
        unTrigger();
        googleTranslate();
        if(waitingForFileExplorer)
            placeLoadedObject();
    }
    void position(){ //sætter position af UI
        startButton.transform.position = new Vector3(Screen.width,0,0);
        Vector2 sizex = xInput.GetComponent<RectTransform>().sizeDelta;
        Vector2 sizey = yInput.GetComponent<RectTransform>().sizeDelta;
        Vector2 sizez = zInput.GetComponent<RectTransform>().sizeDelta;

        rotate.GetComponent<RectTransform>().sizeDelta = new Vector2(sizez.x/3,sizez.x/3);
        translate.GetComponent<RectTransform>().sizeDelta = rotate.GetComponent<RectTransform>().sizeDelta;
        scale.GetComponent<RectTransform>().sizeDelta = rotate.GetComponent<RectTransform>().sizeDelta;

        Vector2 sizeTranslate = translate.GetComponent<RectTransform>().sizeDelta;
        Vector2 sizeRotate = rotate.GetComponent<RectTransform>().sizeDelta;
        Vector2 sizeScale = scale.GetComponent<RectTransform>().sizeDelta;

        rotate.transform.position = new Vector2(xInput.transform.position.x,sizeRotate.y/2);
        translate.transform.position = new Vector2(xInput.transform.position.x/3,rotate.transform.position.y);
        scale.transform.position = new Vector2(xInput.transform.position.x*5/3,rotate.transform.position.y);
        
        zInput.transform.position = new Vector2(sizez.x/2,sizeTranslate.y + sizez.y/2);
        yInput.transform.position = new Vector2(zInput.transform.position.x,zInput.transform.position.y+(sizez.y/2 + sizey.y/2));
        xInput.transform.position = new Vector2(yInput.transform.position.x,yInput.transform.position.y+(sizey.y/2 + sizex.y/2));

        Vector2 sizeAirSpeed = airSpeedField.GetComponent<RectTransform>().sizeDelta;
        Vector2 sizeSimulationDuration = simulationDurationField.GetComponent<RectTransform>().sizeDelta;
        Vector2 sizeAirSpeedText = airSpeedText.GetComponent<RectTransform>().sizeDelta;
        Vector2 sizeSimulationDurationText = simulationDurationText.GetComponent<RectTransform>().sizeDelta;
        Vector2 sizeTargetPreassure = targetPreassureField.GetComponent<RectTransform>().sizeDelta;
        Vector2 sizeTargetPreassureText = targetPreassureText.GetComponent<RectTransform>().sizeDelta;

        airSpeedField.transform.position = new Vector2(Screen.width-sizeAirSpeed.x/2,Screen.height/2);
        airSpeedText.transform.position = new Vector2(Screen.width,airSpeedField.transform.position.y+(sizeAirSpeedText.y/2-2));
        simulationDurationField.transform.position = new Vector2(Screen.width-sizeSimulationDuration.x/2,airSpeedText.transform.position.y+(sizeAirSpeedText.y+sizeSimulationDuration.y/2));
        simulationDurationText.transform.position = new Vector2(Screen.width,simulationDurationField.transform.position.y+(sizeSimulationDurationText.y/2-2));
        targetPreassureField.transform.position = new Vector2(Screen.width-sizeTargetPreassure.x/2,simulationDurationText.transform.position.y+(sizeSimulationDurationText.y+sizeTargetPreassure.y/2));
        targetPreassureText.transform.position = new Vector2(Screen.width,targetPreassureField.transform.position.y+(sizeSimulationDurationText.y/2-2));

        addObject.transform.position = new Vector2(addObject.GetComponent<RectTransform>().sizeDelta.x/2,Screen.height-addObject.GetComponent<RectTransform>().sizeDelta.y/2);
    }
    void fileExplorerSetup(){
        FileBrowser.SetFilters( true, new FileBrowser.Filter( "Mesh Files", ".obj" ) );
		FileBrowser.SetDefaultFilter( ".obj" );
		
    }
    IEnumerator ShowLoadDialogCoroutine()
	{
		yield return FileBrowser.WaitForLoadDialog( false, null, "Load File", "Load" );
		
		if( FileBrowser.Success )
		{
			byte[] bytes = FileBrowserHelpers.ReadBytesFromFile( FileBrowser.Result );
		}
        objectFilePath = FileBrowser.Result;
	}
    static public string objectFilePath;
    public static bool waitingForFileExplorer = false;
    public void loadObject(){
        StartCoroutine( ShowLoadDialogCoroutine() );
        waitingForFileExplorer = true;
    }
    public void placeLoadedObject(){//importerer meshobjekt, som der lige er fundet en sti til
        if(File.Exists(objectFilePath)){
            waitingForFileExplorer = false;
            GameObject meshObject = GameObject.FindGameObjectWithTag("Mesh");//finder nuværende mesh
            GameObject loadedMesh = new OBJLoader().Load(objectFilePath);//laver et nyt objekt med den loadede mesh
            MeshFilter[] meshFilters = loadedMesh.GetComponentsInChildren<MeshFilter>();//laver et array med alle undermeshes i den nye mesh (fordi man nogle gange ser opdelte meshes)
            CombineInstance[] meshParts = new CombineInstance[meshFilters.Length];//laver et kompinationsinstans array
            for(int i = 0; i < meshFilters.Length; i++){
                meshParts[i].mesh = meshFilters[i].sharedMesh;//overfører mesh mellem arraysne
                meshParts[i].transform = meshFilters[i].transform.localToWorldMatrix;//sørger for rigtig position af meshstykkerne
            }
            meshObject.GetComponent<MeshFilter>().mesh = new Mesh();//nulstiller mesh på det gamle objekt
            meshObject.GetComponent<MeshFilter>().mesh.CombineMeshes(meshParts);//sætter meshen til en sammenspaltning af den nye med alle dele
            GameObject.Destroy(loadedMesh);//fjerner loadedmesh objekt, da meshen er overført, og den er ikke nødvendig mere
            MeshCentralicer.Centralise();//kører en centraliseringsprocedure på meshen
        }
    }
    public void translateTrigger(){ //metoder som knapperne kører
        unTrigger();
        translateTriggered = true;
    }
    public void rotateTrigger(){
        unTrigger();
        rotateTriggered = true;
    }    
    public void scaleTrigger(){
        unTrigger();
        scaleTriggered = true;
    }
    void unTrigger(){ //fjerner alle knaptryk
        if(Input.GetMouseButton(0)){
            translateTriggered = false;
            rotateTriggered = false;
            scaleTriggered = false;
        }
    }
    void googleTranslate(){ //oversætter string input til floats
        float xMultiplier = 1;
        float yMultiplier = 1;
        float zMultiplier = 1;
        string inputStringx = "0";
        foreach(var e in inputx){
            if(checkIfValid(e))
                inputStringx += e;
            if(e == "-")
                xMultiplier = -xMultiplier;
        }
        string inputStringy = "0";
        foreach(var e in inputy){
            if(checkIfValid(e))
                inputStringy += e;
            if(e == "-")
                yMultiplier = -yMultiplier;
        }
        string inputStringz = "0";
        foreach(var e in inputz){
            if(checkIfValid(e))
                inputStringz += e;
            if(e == "-")
                zMultiplier = -zMultiplier;
        }
        
        Vector3 transformOutput;
        transformOutput = new Vector3(xMultiplier * float.Parse(inputStringx),yMultiplier * float.Parse(inputStringy),zMultiplier * float.Parse(inputStringz));
        if(scaleTriggered){
            if(transformOutput.x == 0)
                transformOutput.x = 1;
            if(transformOutput.y == 0)
                transformOutput.y = 1;
            if(transformOutput.z == 0)
                transformOutput.z = 1;
        }
        transformVal = transformOutput;

        string inputStringAirSpeed = "0";
        foreach(var e in inputAirSpeed){
            if(checkIfValid(e))
                inputStringAirSpeed += e;
        }
        if(float.Parse(inputStringAirSpeed) != 0)
            Inputs.publicAirSpeed = float.Parse(inputStringAirSpeed);
        if(Inputs.publicAirSpeed == 0)
            Inputs.publicAirSpeed = Inputs.publicStandartAirSpeed;

        string inputStringSimDuration = "0";
        foreach(var e in inputSimDuration){
            if(checkIfValid(e))
                inputStringSimDuration += e;
        }
        if(float.Parse(inputStringSimDuration) != 0)
            Inputs.publicSimCalcTime = float.Parse(inputStringSimDuration);
        if(Inputs.publicSimCalcTime == 0)
            Inputs.publicSimCalcTime = Inputs.publicStandartSimSeconds;
        
        string inputStringTargetPreassure = "0";
        foreach(var e in inputTargetPreassure){
            if(checkIfValid(e))
                inputStringTargetPreassure += e;
        }
        if(float.Parse(inputStringTargetPreassure) != 0)
            Inputs.publicTargetPreassure = float.Parse(inputStringTargetPreassure);
        if(Inputs.publicTargetPreassure == 0)
            Inputs.publicTargetPreassure = Inputs.publicStandartTargetPreassure;
    }
    bool checkIfValid(string input){//tjekker om string input er gyldig for float
        if (input == "1" || input == "2" || input == "3" || input == "4" || input == "5" || input == "6" || input == "7" || input == "8" || input == "9" || input == "0" || input == ".")
            return true;
        else
            return false;
    }
    public static List<string> inputx = new List<string>();
    public static List<string> inputy = new List<string>();
    public static List<string> inputz = new List<string>();
    public static List<string> inputAirSpeed = new List<string>();
    public static List<string> inputSimDuration = new List<string>();
    public static List<string> inputTargetPreassure = new List<string>();
    public static int cursorPosx = -1;
    public static int cursorPosy = -1;
    public static int cursorPosz = -1;
    public static int cursorPosAirSpeed = -1;
    public static int cursorPosSimDuration = -1;
    public static int cursorPosTargetPreassure = -1;
    public void StringInput(string field){//metode som et felt kører, når der bliver skrevet i den, som så giver en liste med alle karakterer i feltet. metoden går efter computer taster i stedet for feltet (fordi Unity)
        bool unsuportedKey = true;
        int inputLengthx = inputx.Count;
        int inputLengthy = inputy.Count;
        int inputLengthz = inputz.Count;
        int inputLengthAirSpeed = inputAirSpeed.Count;
        int inputLengthSimDuration = inputSimDuration.Count;
        int inputLengthTargetPreassure = inputTargetPreassure.Count;
        string input = "";
        
        if(Input.GetKey(KeyCode.LeftArrow)){
            unsuportedKey = false;
            if(field == "x" && cursorPosx > 0) cursorPosx--;
            if(field == "y" && cursorPosy > 0) cursorPosy--;
            if(field == "z" && cursorPosz > 0) cursorPosz--;
        }
        else if(Input.GetKey(KeyCode.RightArrow)){
            unsuportedKey = false;
            if(cursorPosx != inputx.Count-1 && field == "x")
                cursorPosx++;
            if(cursorPosy != inputy.Count-1 && field == "x")
                cursorPosy++;
            if(cursorPosz != inputz.Count-1 && field == "x")
                cursorPosz++;
        }
        else if(Input.GetKey(KeyCode.Backspace)){
            unsuportedKey = false;
            if(!(inputx.Count == 0) && field == "x"){
                inputx.RemoveAt(cursorPosx);
                cursorPosx--;
            }
            if(!(inputy.Count == 0) && field == "y"){
                inputy.RemoveAt(cursorPosy);
                cursorPosy--;
            }
            if(!(inputz.Count == 0) && field == "z"){
                inputz.RemoveAt(cursorPosz);
                cursorPosz--;
            }
            if(!(inputAirSpeed.Count == 0) && field == "airSpeed"){
                inputAirSpeed.RemoveAt(cursorPosAirSpeed);
                cursorPosAirSpeed--;
            }
            if(!(inputSimDuration.Count == 0) && field == "simDuration"){
                inputSimDuration.RemoveAt(cursorPosSimDuration);
                cursorPosSimDuration--;
            }
            if(!(inputTargetPreassure.Count == 0) && field == "targetPreassure"){
                inputTargetPreassure.RemoveAt(cursorPosTargetPreassure);
                cursorPosTargetPreassure--;
            }
        }
        
        if(Input.GetKey(KeyCode.A))
            input = "a";
        if(Input.GetKey(KeyCode.B))
            input = "b";
        if(Input.GetKey(KeyCode.C))
            input = "c";
        if(Input.GetKey(KeyCode.D))
            input = "d";
        if(Input.GetKey(KeyCode.E))
            input = "e";
        if(Input.GetKey(KeyCode.F))
            input = "f";
        if(Input.GetKey(KeyCode.G))
            input = "g";
        if(Input.GetKey(KeyCode.H))
            input = "h";
        if(Input.GetKey(KeyCode.I))
            input = "i";
        if(Input.GetKey(KeyCode.J))
            input = "j";
        if(Input.GetKey(KeyCode.K))
            input = "k";
        if(Input.GetKey(KeyCode.L))
            input = "l";
        if(Input.GetKey(KeyCode.M))
            input = "m";
        if(Input.GetKey(KeyCode.N))
            input = "n";
        if(Input.GetKey(KeyCode.O))
            input = "o";
        if(Input.GetKey(KeyCode.P))
            input = "p";
        if(Input.GetKey(KeyCode.Q))
            input = "q";
        if(Input.GetKey(KeyCode.R))
            input = "r";
        if(Input.GetKey(KeyCode.S))
            input = "s";
        if(Input.GetKey(KeyCode.T))
            input = "t";
        if(Input.GetKey(KeyCode.U))
            input = "u";
        if(Input.GetKey(KeyCode.V))
            input = "v";
        if(Input.GetKey(KeyCode.W))
            input = "w";
        if(Input.GetKey(KeyCode.X))
            input = "x";
        if(Input.GetKey(KeyCode.Y))
            input = "y";
        if(Input.GetKey(KeyCode.Z))
            input = "z";
        if(Input.GetKey(KeyCode.Space))
            input = " ";
        if(Input.GetKey(KeyCode.Period))
            input = ".";
        if(Input.GetKey(KeyCode.Comma))
            input = ",";
        if(Input.GetKey(KeyCode.Keypad0)){
            unsuportedKey = false;
            input = "0";
        }
        if(Input.GetKey(KeyCode.Keypad1)){
            unsuportedKey = false;
            input = "1";
        }
        if(Input.GetKey(KeyCode.Keypad2)){
            unsuportedKey = false;
            input = "2";
        }
        if(Input.GetKey(KeyCode.Keypad3)){
            unsuportedKey = false;
            input = "3";
        }
        if(Input.GetKey(KeyCode.Keypad4)){
            unsuportedKey = false;
            input = "4";
        }
        if(Input.GetKey(KeyCode.Keypad5)){
            unsuportedKey = false;
            input = "5";
        }
        if(Input.GetKey(KeyCode.Keypad6)){
            unsuportedKey = false;
            input = "6";
        }
        if(Input.GetKey(KeyCode.Keypad7)){
            unsuportedKey = false;
            input = "7";
        }
        if(Input.GetKey(KeyCode.Keypad8)){
            unsuportedKey = false;
            input = "8";
        }
        if(Input.GetKey(KeyCode.Keypad9)){
            unsuportedKey = false;
            input = "9";
        }
        if(Input.GetKey(KeyCode.Alpha0)){
            unsuportedKey = false;
            input = "0";
        }
        if(Input.GetKey(KeyCode.Alpha1)){
            unsuportedKey = false;
            input = "1";
        }
        if(Input.GetKey(KeyCode.Alpha2)){
            unsuportedKey = false;
            input = "2";
        }
        if(Input.GetKey(KeyCode.Alpha3)){
            unsuportedKey = false;
            input = "3";
        }
        if(Input.GetKey(KeyCode.Alpha4)){
            unsuportedKey = false;
            input = "4";
        }
        if(Input.GetKey(KeyCode.Alpha5)){
            unsuportedKey = false;
            input = "5";
        }
        if(Input.GetKey(KeyCode.Alpha6)){
            unsuportedKey = false;
            input = "6";
        }
        if(Input.GetKey(KeyCode.Alpha7)){
            unsuportedKey = false;
            input = "7";
        }
        if(Input.GetKey(KeyCode.Alpha8)){
            unsuportedKey = false;
            input = "8";
        }
        if(Input.GetKey(KeyCode.Alpha9)){
            unsuportedKey = false;
            input = "9";
        }
        if(Input.GetKey(KeyCode.Minus)){
            unsuportedKey = false;
            input = "-";
        }
        if(Input.GetKey(KeyCode.KeypadMinus)){
            unsuportedKey = false;
            input = "-";
        }
        if(Input.GetKey(KeyCode.Plus)){
            unsuportedKey = false;
            input = "+";
        }
        if(Input.GetKey(KeyCode.KeypadPlus)){
            unsuportedKey = false;
            input = "+";
        }
        if(field == "x" && input != "")
            inputx.Add(input);
        if(field == "y" && input != "")
            inputy.Add(input);
        if(field == "z" && input != "")
            inputz.Add(input);
        if(field == "airSpeed" && input != "")
            inputAirSpeed.Add(input);
        if(field == "simDuration" && input != "")
            inputSimDuration.Add(input);
        if(field == "targetPreassure" && input != "")
            inputTargetPreassure.Add(input);

        if(inputLengthx < inputx.Count)
            cursorPosx++;
        if(inputLengthy < inputy.Count)
            cursorPosy++;
        if(inputLengthz < inputz.Count)
            cursorPosz++;
        if(inputLengthAirSpeed < inputAirSpeed.Count)
            cursorPosAirSpeed++;
        if(inputLengthSimDuration < inputSimDuration.Count)
            cursorPosSimDuration++;
        if(inputLengthTargetPreassure < inputTargetPreassure.Count)
            cursorPosTargetPreassure++;
        
        unsuportedKeyReing(unsuportedKey);
    }
    public static void unsuportedKeyReing(bool reeStartOrStop){ //klager hvis tasten ikke er suported
        var ree = GameObject.FindGameObjectWithTag("ree");
        if(reeStartOrStop){
            ree.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        }
        else{
            ree.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width,-Screen.height);
        }        
    }
}