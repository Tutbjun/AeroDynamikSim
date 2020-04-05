using System.Collections.Generic;
using UnityEngine;

public class densityCalcParticleController : MonoBehaviour //eventhandler til tryksimuleringen. sørger for kontrol af partikler
{
    public static List<string> particleStates = new List<string>();
    public static float momentum = 0;
    static public List<GameObject> particles = new List<GameObject>();
    public GameObject meshBorder;
    public GameObject helium;
    static public bool preassureFound = false;
    public static float targetPreassure;
    public static int count = 0;
    public static int countChange = 1;
    public static List<ParticleForDensityCalc> molecules = new List<ParticleForDensityCalc>();
    public static string cycleStep = "CyclePrepPhase";
    public static bool debugMode = false;
    public static bool showSphere = true;
    public static float molarMassFluid = 4.002602f; //er sat til helium nu
    public static float tempKelvin = 273.15f; 
    public static bool tempVelocity = true;
    public const float gasKonstant = 8.314472f;
    public static int compensateVelTime = 10;
    public static float preassureScaler = 50;//plejer være 50
    public static int nonSimTime = 50;
    public static float startVelSum;
    public static Vector3 velocitySI = new Vector3(0f,0f,0f);
    public static int chunckSize = 5000;
    public static int dynamicInvCycleTime =(int) (Inputs.publicCyclesPerSecond * Inputs.publicDensityCalcTime); 
    public static float logForceTime = 0.5f;

    public static Vector3 velToReal (Vector3 velSI){//metode som omdanner SI enheder til picometer/cycle
        long hastighedMellemX = (long) (velSI.x * 1000000000000L + 0.5f);
        int vx = (int) (hastighedMellemX/Inputs.publicCyclesPerSecond);

        long hastighedMellemY = (long) (velSI.y * 1000000000000L + 0.5f);
        int vy = (int) (hastighedMellemY/Inputs.publicCyclesPerSecond);

        long hastighedMellemZ = (long) (velSI.z * 1000000000000L + 0.5f);
        int vz = (int) (hastighedMellemZ/Inputs.publicCyclesPerSecond);
        Vector3 velReal = new Vector3(vx, vy, vz);
        return velReal;
    }
    void Update()
    {
        if(cycleStep != "SimFinsihed"){
            if(cycleStep == "CyclePrepPhase"){
                var e = GameObject.FindGameObjectWithTag("ParticleBorderBox");
                dynamicInvCycleTime = (int) (Inputs.publicCyclesPerSecond * Inputs.publicDensityCalcTime);
                if(e != null){//udregning af trykændring
                    float force = momentum * Inputs.momentumToSIFactor / Inputs.publicDensityCalcTime * 2;
                    momentum = 0;
                    float picometer = Mathf.Pow(10,-12);
                    float area = Mathf.Pow(e.transform.localScale.x*picometer,2)*2 + Mathf.Pow(e.transform.localScale.y*picometer,2)*2 + Mathf.Pow(e.transform.localScale.z*picometer,2)*2;
                    float preassure = force / area;
                    float deltaPreassure = targetPreassure - preassure;
                    float lastCountChange = countChange;
                    countChange = (int) (deltaPreassure / preassureScaler + 1f);
                    if(lastCountChange / countChange < 0)//hvis der er skudt forbi, så bliver multiplieren formindsket eller noget i den stil
                        preassureScaler *= 2;
                    if(lastCountChange == countChange)
                        preassureFound = true;
                }
                momentum = 0;
                GameObject.Destroy(e);
                GameObject.Instantiate(meshBorder);
                cycleStep = "BeforeCycle";
                count += countChange;
                molecules.Clear();//sletter partikler
                GameObject[] particleObjects = GameObject.FindGameObjectsWithTag("Particle");
                foreach( var yote in particleObjects){
                    GameObject.Destroy(yote);
                }
                particles.Clear();
                particleStates.Clear();
                if(preassureFound){
                    cycleStep = "SimFinished";
                }
                else{
                    for(int i = 0; i < count; i++){//danner nye
                        GameObject particle = GameObject.Instantiate(helium);
                        molecules.Add(particle.GetComponent<ParticleForDensityCalc>());
                    }
                }
                startVelSum = 0;
            }
            string particleState = "nada";//finder samlet partikkel simstatus
            if(molecules.Count >= 1){
                foreach(var e0 in particleStates){ //finder bare første værdi
                    particleState = e0;
                    if(e0 != null){
                        break;
                    }
                }
                foreach(var e in particleStates){
                    if(e != particleState){
                        particleState = "uenig"; //hvis alle ikke er lig den første, så bliver de dømt uenig
                    }
                }
            }
            if(particleState == "CycleFinished"){
                float ja = ((float)dynamicInvCycleTime)/((float) compensateVelTime);
                if((int) ja == ja){//kompenserer for kinetisk energiændring (fordi der bør være energibevarelse), fordi der kan ske fejl, hvor hastigheden ikke bliver præcist simuleret
                    float velSum = 0;
                    foreach(var e in molecules){
                        velSum += e.velocityReal.magnitude;
                    }
                    float multiplier = startVelSum/velSum;
                    foreach(var e in molecules){
                        e.velocityReal = e.velocityReal * multiplier;
                    }
                }
                if(startVelSum == 0){
                    foreach(var e in molecules){
                        startVelSum += e.velocityReal.magnitude;
                    }   
                }
                cycleStep = "CycleFinished";
                if((Input.GetKeyDown("space") && debugMode) || !debugMode){
                    cycleStep = "BeforeCycle";
                }
                if(dynamicInvCycleTime < 0){
                    cycleStep = "CyclePrepPhase";
                }
            }
            //kontrol af partikler, så de ikke kommer foran hinanden (mest til implementering med GPU):
            if(cycleStep == "BeforeCycle"){
                cycleStep = "CycleStarting";
                dynamicInvCycleTime--;
            }
            if(particleState == "AwaitingSimulatingParticleCollision" && dynamicInvCycleTime > 0){
                cycleStep = "SimulateParticleCollision";
            }
            else if(dynamicInvCycleTime <= 0){
                cycleStep = "CyclePrepPhase";
            }
            if(particleState == "AwaitingSimulatingMeshCollision" && dynamicInvCycleTime > 0){
                cycleStep = "SimulatingMeshCollision";
            }
        }
    }
}