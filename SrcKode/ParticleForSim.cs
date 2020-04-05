using UnityEngine;

public class ParticleForSim : Helium//partikkeltype, som indeholder overordet kode til simuleringen
{

    void Start()
    {
        posSmall = transform.position;
        Id = SimParticleController.particleStates.Count;
        SimParticleController.particleStates.Add("null");
        staticId = Id;
        velToAdd.Add(startSpeedCalc());
    }
    
    void Update(){
        if(SimParticleController.shouldRun){ 
            velocityReal += updateVel(velToAdd);//opdatering af hastighed
            velToAdd.Clear();
            updateCycleState();

            //simulering på basis af eventhandlerens status:
            if(SimParticleController.cycleStep == "CycleStarting"){
                localCycleState = "AwaitingSimulatingParticleCollision";
            }
            if(SimParticleController.cycleStep == "SimulateParticleCollision" && localCycleState == "AwaitingSimulatingParticleCollision"){
                SimParticleController.molecules[staticId].posSmall += velocityReal;
                localCycleState = "simulatingParticleCollision";
                particleCollisionSim(localMass, posSmall, posLarge, velocityReal, densityCalcParticleController.chunckSize, true, SimParticleController.molecules.Count, localRadius);
                localCycleState = "AwaitingSimulatingMeshCollision";
            }
            if(SimParticleController.cycleStep == "SimulatingMeshCollision" && localCycleState == "AwaitingSimulatingMeshCollision"){
                velToAdd.Add(meshCollisionSim(BorderMeshSim.mesh, true, false, posSmall, velocityReal, SimParticleController.dynamicInvCycleTime, Inputs.simTimeSwitchThreshold, localMass, localRadius));
                velToAdd.Add(meshCollisionSim(GameObject.FindGameObjectWithTag("Mesh").GetComponent<MeshFilter>().mesh, false, true, posSmall, velocityReal, SimParticleController.dynamicInvCycleTime,Inputs.simTimeSwitchThreshold, localMass, localRadius));
                if(localCycleState != "DeleteMe")
                    localCycleState = "CycleFinished";
            }
            if(errorInCycle){
                print("error in cycle!");
                localCycleState = "Error";
                print(localCycleState);
            }
            showMovement();
        }
    }
    void updateCycleState(){//afraporterer sin cyclestate
        staticId = Id;
        SimParticleController.particleStates.Insert(Id, localCycleState);
        if(SimParticleController.particleStates.Count > SimParticleController.molecules.Count){
            SimParticleController.particleStates.RemoveAt(Id+1);
        }
    }

    void showMovement(){//ja
        if(densityCalcParticleController.showSphere)
            transform.position = SimParticleController.molecules[staticId].posSmall;
        transform.localScale = new Vector3(localRadius*2, localRadius*2, localRadius*2);
    }
}