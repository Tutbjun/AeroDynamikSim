using UnityEngine;

public class EventHandler : MonoBehaviour //styreklasse, som kontrolerer et par ting i starten, men holder mest af alt styr på simuleringen
{
    public GameObject UIDeleter;
    public GameObject MeshDivider;
    public GameObject densityCalculator;
    public GameObject simController;
    public GameObject Scaler;
    int step = -1;
    public static string dataPath;
    public static bool evaluateSim;
    public static bool simRunning;
    public GameObject placeYourThingsText;
    public GameObject findingPreassureText;
    public GameObject simulatingText;
    public GameObject evaluatingText;    
    public void startSim(){ //trigger til startknappen
        simRunning = true;
    }
    void Update(){
        if(step == -1){//placerer skærmtekst
            placeTextOnScreen(placeYourThingsText);
            removeTextfromScreen(findingPreassureText);
            removeTextfromScreen(simulatingText);
            removeTextfromScreen(evaluatingText);
            UIHandler.unsuportedKeyReing(false);      
            MeshCentralicer.Centralise();      
            step++;
        }
        if(simRunning){//når startknappen er trykket på
            if(step == 0){//ordner visuelle ting
                GameObject.Instantiate(UIDeleter);
                GameObject.FindGameObjectWithTag("Mesh").GetComponent<MeshRenderer>().enabled = false;
                GameObject.FindGameObjectWithTag("ParticleBorders").GetComponent<MeshRenderer>().enabled = false;
                GameObject.FindGameObjectWithTag("ParticleSummonPlane").GetComponent<MeshRenderer>().enabled = false;
                removeTextfromScreen(placeYourThingsText);
                step++;
            }
            GameObject UIDeleterCreation;
            UIDeleterCreation = GameObject.FindGameObjectWithTag("UIDeleter");
            if(UIDeleterCreation == null && step == 1){//vælger datamappe
                //dataPath = EditorUtility.OpenFolderPanel("Choose data output folder", "", "JegErEnMappe");
                dataPath = "FeatureIkkeImplementeret";
                step++;
            }
            if(dataPath != null && step == 2){//starter trykudregning
                GameObject.Instantiate(densityCalculator);
                placeTextOnScreen(findingPreassureText);
                step++;
            }
            if(densityCalcParticleController.preassureFound && step == 3){//ordner visuelle ting efter trykudregning
                removeTextfromScreen(findingPreassureText);
                GameObject.FindGameObjectWithTag("Mesh").GetComponent<MeshRenderer>().enabled = true;
                GameObject.FindGameObjectWithTag("ParticleBorders").GetComponent<MeshRenderer>().enabled = true;
                GameObject.FindGameObjectWithTag("ParticleSummonPlane").GetComponent<MeshRenderer>().enabled = true;
                //GameObject.Instantiate(MeshDivider);
                step++;
            }
            if(step == 4){//skalerer meshen op
                GameObject.Instantiate(Scaler);
                step++;
            }
            if(step == 5){//starter simulering
                GameObject.Instantiate(simController);
                step++;
                placeTextOnScreen(simulatingText);
            }
            if(step == 6 && SimParticleController.simFinished){//evaluerer simulering
                removeTextfromScreen(simulatingText);
                placeTextOnScreen(evaluatingText);
                evaluateSim = true;
                step++;
            }
            if(step == 7 && MainMesh.meshColored){//viser farver frem
                removeTextfromScreen(evaluatingText);
                step++;
            }
        }
    }
    void placeTextOnScreen(GameObject text){ //metoder til placering af tekst
        text.transform.position = new Vector2(Screen.width/2,text.GetComponent<RectTransform>().sizeDelta.y/2);
    }
    void removeTextfromScreen(GameObject text){
        text.transform.position = new Vector2(Screen.width/2,-text.GetComponent<RectTransform>().sizeDelta.y/2);
    }
}