using System.Collections.Generic;
using UnityEngine;

public class SimParticleController : MonoBehaviour //eventhandler til simuleringen. sørger for kontrol af partikler
{
    public static bool simFinished;//siger sig selv
    bool startup = true;
    public GameObject particleSummoner;//partikkel instantiatoren
    public static List<string> particleStates = new List<string>();//liste med alle partikkelstatuser
    public GameObject helium; //helium prefab
    public static GameObject publicHelium; //hvis andre objekter har brug for acces til det
    public static int count; //antal partikler i en bestemt kasse med bestemt volume
    public static List<ParticleForSim> molecules = new List<ParticleForSim>();//her bliver alle instantierede partikler "opbevaret"
    public static string cycleStep = "BeforeCycle";//overordnet sim status. partiklerne følger denne
    public static bool debugMode = false;
    public static bool shouldRun = false;
    public static int compensateVelTime = 10;//hvor ofte der skal kompenseres for samlet hastighedsændring
    public static float startVelSumPrMol; //variabel til startværdi for kinetisk energi i systemet
    public static Vector3 velocitySI;//input hastighed
    public static int dynamicInvCycleTime;//ur for hvor langt simuleringen er nået
    public static float logForceTime = 0.5f;//hvor meget skal der gå af simuleringen ud af 1 før kræfter bliver logget
    public static float cyclesToParticleSummon;//hvor ofte der skal kompenseres for fejlfart i partiklerne
    public static float particlesPrVol;//siger sig selv
    void Start(){
        dynamicInvCycleTime = (int) (Inputs.publicCyclesPerSecond * Inputs.publicSimCalcTime);
    }
    void Update()
    {
        if(dynamicInvCycleTime < 0){//sletter alle partikkelinstanser når simulationen er færdig
            simFinished = true;
            for(int i = 0; i < particleStates.Count; i++){
                particleStates[i] = "DeleteMe";
            }
        }
        if(molecules.Count == particleStates.Count){//sørger for at alle partikler er ordentligt instantieret før den kører
            shouldRun = true;
        }

        if(startup && Inputs.publicAirSpeed != 0){//nogle startprocedurer før simulationen kører
            count = densityCalcParticleController.count;//overfører det fundne resultat fra tryk simulering
            startup = false;
            particlesPrVol = count / Mathf.Pow(densityCalcParticleController.chunckSize,3);//omregner det til pr. volume
            velocitySI = ParticleBordersPositioner.dir * Inputs.publicAirSpeed;//laver far til hastighed
            cyclesToParticleSummon = Mathf.Pow(particlesPrVol,-0.333f) / densityCalcParticleController.velToReal(velocitySI).magnitude;//udregner hvor ofte der skal komme en ny bølge af partikler
            particleSummoner = GameObject.Instantiate(particleSummoner);//instantierer partikkelinstantiatoren
        }
        else if (!startup){
            publicHelium = helium;
            if(cycleStep == "SimFinished"){//fjerner kassen
                if(GameObject.FindGameObjectWithTag("ParticleBorderBox") != null){
                    var e = GameObject.FindGameObjectWithTag("ParticleBorderBox");
                    GameObject.Destroy(e);
                }
            }
            else{
                string particleState = "nada";//finder samlet partikkelstatus
                if(molecules.Count >= 1){
                    for(int i = 0; i < particleStates.Count; i++){
                        if(particleStates[i] == "DeleteMe"){//fjerner partikler hvis de er markeret til at blive fjernet
                            particleStates.RemoveAt(i);
                            foreach(var e in GameObject.FindGameObjectsWithTag("Particle")){
                                if(e.transform.position == molecules[i].transform.position){
                                    GameObject.Destroy(e);
                                }
                            }
                            molecules.RemoveAt(i);
                            for(int i2 = i; i2 < molecules.Count; i2++){
                                molecules[i2].Id--;
                            }
                        }
                    }
                    foreach(var e0 in particleStates){ //finder bare første værdi
                        particleState = e0;
                        if(e0 != null){
                            break;
                        }
                    }
                    foreach(var e in particleStates){
                        if(e != particleState){
                            particleState = "uenig"; //hvis alle ikke er lig den første, så er de uenige
                            break;
                        }
                    }
                }
                else{//hvis der ikke er partikler endnu
                    cycleStep = "BeforeCycle";
                }

                if(particleState == "CycleFinished"){
                    float ja = ((float)dynamicInvCycleTime)/((float) compensateVelTime);//kompenserer for energitab, da partiklerne kan miste eller få for meget hastighed
                    if((int) ja == ja){
                        float velSumPrMol = 0;
                        foreach(var e in molecules){
                            velSumPrMol += e.velocityReal.magnitude;
                        }
                        velSumPrMol /= molecules.Count;
                        float multiplier = startVelSumPrMol/velSumPrMol;
                        foreach(var e in molecules){
                            e.velocityReal = e.velocityReal * multiplier;
                        }
                    }
                    if(startVelSumPrMol == 0){
                        foreach(var e in molecules){
                            startVelSumPrMol += e.velocityReal.magnitude;
                        }   
                        startVelSumPrMol /= molecules.Count;
                    }
                    if((Input.GetKeyDown("space") && debugMode) || !debugMode){
                        cycleStep = "BeforeCycle";
                    }
                    if(dynamicInvCycleTime < 0){
                        cycleStep = "BeforeCycle";
                    }
                }
                //for at holde partiklerne i kort snor så de ikke kører foran hinanden (mest til implementering med GPU):
                if(cycleStep == "BeforeCycle"){
                    cycleStep = "CycleStarting";
                    dynamicInvCycleTime--;
                }
                if(particleState == "AwaitingSimulatingParticleCollision" && dynamicInvCycleTime > 0){
                    cycleStep = "SimulateParticleCollision";
                }
                else if(dynamicInvCycleTime <= 0){
                    cycleStep = "BeforeCycle";
                }
                if(particleState == "AwaitingSimulatingMeshCollision" && dynamicInvCycleTime > 0){
                    cycleStep = "SimulatingMeshCollision";
                }
                
            }
        }
    }
}