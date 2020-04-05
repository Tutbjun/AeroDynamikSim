using UnityEngine;

public class ParticleForDensityCalc : Helium//partikkeltype, som indeholder overordet kode til trykudregning
{
    void Start()
    {
        Id = densityCalcParticleController.particleStates.Count;
        densityCalcParticleController.particleStates.Add("null");
        staticId = Id;
        velToAdd.Add(startSpeedCalc());
    }
    void Update(){
        velocityReal += updateVel(velToAdd);//opdatering af hastighed
        velToAdd.Clear();
        updateCycleState();

        //simulering på basis af eventhandlerens status:
        if(densityCalcParticleController.cycleStep == "CycleStarting"){
            localCycleState = "AwaitingSimulatingParticleCollision";
        }
        if(densityCalcParticleController.cycleStep == "SimulateParticleCollision" && localCycleState == "AwaitingSimulatingParticleCollision"){
            densityCalcParticleController.molecules[staticId].posSmall += velocityReal;
            if(densityCalcParticleController.dynamicInvCycleTime < Inputs.publicCyclesPerSecond * Inputs.publicDensityCalcTime - densityCalcParticleController.nonSimTime){
                localCycleState = "simulatingParticleCollision";
                particleCollisionSim(localMass, posSmall, posLarge, velocityReal, densityCalcParticleController.chunckSize, false,densityCalcParticleController.molecules.Count, localRadius);
            }
            localCycleState = "AwaitingSimulatingMeshCollision";
        }
        if(densityCalcParticleController.cycleStep == "SimulatingMeshCollision" && localCycleState == "AwaitingSimulatingMeshCollision"){
            densityCalcParticleController.molecules[staticId].velToAdd.Add(meshCollisionSim(BorderMesh.mesh, true, true, posSmall, velocityReal, densityCalcParticleController.dynamicInvCycleTime, Inputs.densityTimeSwitchThreshold, localMass, localRadius));
            localCycleState = "CycleFinished";
        }
        if(errorInCycle){
            print("error in cycle!");
            localCycleState = "Error";
            print(localCycleState);
        }
        showMovement();
    }
    void updateCycleState(){//afraporterer sin cyclestate
        staticId = Id;
        densityCalcParticleController.particleStates.Insert(Id, localCycleState);
        densityCalcParticleController.particleStates.RemoveAt(Id + 1);
    }
    void showMovement(){//hvis en bestem bool er true, så viser den bevægelsen grafisk
        if(densityCalcParticleController.showSphere){
            transform.position = densityCalcParticleController.molecules[staticId].posSmall;
        }
        transform.localScale = new Vector3(localRadius*2, localRadius*2, localRadius*2);    
    }
}