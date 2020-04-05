using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Particle : MonoBehaviour
{
    public int Id = -1;//partikkel id
    public static int staticId;
    public static bool errorInCycle = false; //bool til hvis der sker fejl (på nuværende tidspunkt detekterer den for stor hastighed)
    public string localCycleState; //til komunikation med partikkel controlleren (kunne være int, men koden er nemmmere at forstå, når det er en string)
    public Vector3 posSmall = new Vector3(0,0,0);//"lille" position i hver chunk
    public Vector3Int posLarge = new Vector3Int(0,0,0); //"stor" position, som siger hvilken chunk partiklen er i (bruges ikke)
    public Vector3 velocityReal; //partikkel hastighed i picometer/cycle
    public List<Vector3> velToAdd = new List<Vector3>();//opbevaring af hastighedsændringer fra kolisioner
    public abstract int localRadius{//nedarvet radius egenskab
        get;
    }
    public abstract int localMass{//nedarvet masse egenskab
        get;
    }
    public static Vector3 startSpeedCalc() //udregner hastighed ud fra input SI-enhed hastighed og temperatur
    {
        Vector3 velocitySI = densityCalcParticleController.velocitySI;
        velocitySI += calcTempVel();
        return densityCalcParticleController.velToReal(velocitySI);
    }
    public static Vector3 calcTempVel(){//udregner den tilfædige temperatur hastighedskomposant
        float meanTempSpeed = 0;
        if(densityCalcParticleController.tempVelocity){
            meanTempSpeed = Mathf.Sqrt(
                3*densityCalcParticleController.gasKonstant*densityCalcParticleController.tempKelvin
                /densityCalcParticleController.molarMassFluid);
        }
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f,1f),UnityEngine.Random.Range(-1f,1f),UnityEngine.Random.Range(-1f,1f));
        randomDirection = randomDirection.normalized;
        return randomDirection * meanTempSpeed;
    }
    public static Vector3 updateVel(List<Vector3> addVelList){
        Vector3 velUpdate = new Vector3();
        if(addVelList.Count > 0){
            foreach(var e in addVelList){
                velUpdate += e;
            }
        }
        return velUpdate;
    }
    public static Vector3 meshCollisionSim(Mesh givenMesh, bool border, bool preassureMesh, Vector3 pos, Vector3 vel, int cycleTime, int timeShreshhold, int mass, int radius){//mesh kollision metoden. detekterer kollision og returnerer deltaHastighed
        for(int triangle = 0; triangle < givenMesh.triangles.Length/3; triangle++){
            Vector3[] meshPos = new Vector3[3];
            Vector3 deltaPos = new Vector3(0,0,0);
            
            bool collision = true;
            meshPos[0] = givenMesh.vertices[givenMesh.triangles[triangle*3]];
            meshPos[1] = givenMesh.vertices[givenMesh.triangles[triangle*3+1]];
            meshPos[2] = givenMesh.vertices[givenMesh.triangles[triangle*3+2]];

            Vector3 nummer1 = meshPos[1] - meshPos[0];
            Vector3 nummer2 = meshPos[2] - meshPos[0];

            Vector3 normalVector = Vector3.Cross(nummer1, nummer2);
            if(border)
                normalVector = -normalVector;
            normalVector = normalVector.normalized;
            
            float normalVectorDeltaPos = 0; //udregner afstanden fra trekantens plan (den retvinklede afstand)
            normalVectorDeltaPos = -(normalVector.x*meshPos[0].x-normalVector.x*pos.x
                +normalVector.y*meshPos[0].y-normalVector.y*pos.y
                +normalVector.z*meshPos[0].z-normalVector.z*pos.z)
                /(Mathf.Pow(normalVector.x,2)+Mathf.Pow(normalVector.y,2)+Mathf.Pow(normalVector.z,2));
            if(normalVectorDeltaPos > radius || (normalVectorDeltaPos <= 0 && !border)){
                collision = false; //hvis den ikke er indenfor en korrekt afstand
            }
            if(collision){
                Vector3[] deltaPos2D = new Vector3[3];
                Vector3[,] deltaMeshPos = new Vector3[3,2];
                float[] anglesDynamic = new float[3];
                float[] anglesStatic = new float[3];
                for(int i2 = 0; i2 < 3; i2++){
                    int i3 = i2+1;
                    if(i3 == 3) i3 = 0;
                    int i4 = i3+1;
                    if(i4 == 3) i4 = 0;
                    deltaMeshPos[i2,0] = meshPos[i3]-meshPos[i2];  
                    deltaMeshPos[i2,1] = meshPos[i4]-meshPos[i2];  
                    deltaPos2D[i2] = pos - normalVectorDeltaPos * normalVector - meshPos[i2];
                }
                for(int i2 = 0; i2 < 3; i2++){
                    int i3 = i2+1;
                    if(i3 == 3) i3 = 0;
                    anglesDynamic[i2] = Mathf.Acos(Vector3.Dot(deltaMeshPos[i2,0],deltaPos2D[i2])/
                        (deltaMeshPos[i2,0].magnitude*deltaPos2D[i2].magnitude));
                    anglesStatic[i2] = Mathf.Acos(Vector3.Dot(deltaMeshPos[i2,0],deltaMeshPos[i2,1])/
                        (deltaMeshPos[i2,0].magnitude*deltaMeshPos[i2,1].magnitude)); 
                }
                for(int i2 = 0; i2 < 3; i2++){
                    if(anglesDynamic[i2] > anglesStatic[i2]){
                        collision = false; //hvis punktet er udenfor trekanten
                    }     
                }
            }
            //se om partiklen bevæger sig væk fra eller mod trekanten
            float colAngle = 0;
            if(collision){
                colAngle = Mathf.Acos(Vector3.Dot(normalVector,vel)/
                    (normalVector.magnitude*vel.magnitude));
                if(vel.magnitude == 0){
                    colAngle = 0;
                }
                if(colAngle <= Mathf.PI/2 || colAngle >= Mathf.PI*1.5f){
                    collision = false; //hvis partiklen bevæger sig væk fra trekanten
                }
            }
            if(collision){
                Vector3 mirrorVel;
                float dot = Vector3.Dot(vel, normalVector);
                mirrorVel.x = -2 * dot * normalVector.x + vel.x;
                mirrorVel.y = -2 * dot * normalVector.y + vel.y;
                mirrorVel.z = -2 * dot * normalVector.z + vel.z;
                
                if(preassureMesh){
                    Vector3[] impuls = new Vector3[2];
                    impuls[0] = vel * (((float) mass) / 1000000f);
                    impuls[1] = mirrorVel * (((float) mass) / 1000000f);

                    Vector3 deltaImpuls = impuls[1] - impuls[0];
                    if(cycleTime <= timeShreshhold){
                        if(!border)
                            MainMesh.momentums[triangle] += deltaImpuls.magnitude; 
                        else if(border)
                            densityCalcParticleController.momentum += deltaImpuls.magnitude; 
                    }
                }
                if(border && !preassureMesh && ParticleBordersPositioner.dir == -normalVector){
                    SimParticleController.molecules[staticId].localCycleState = "DeleteMe";
                }
                return (mirrorVel - vel);
            }
        }
        return new Vector3();
    }
    public static void posEstimate(Vector3 posS, Vector3Int posL, Vector3 vel, int chunkSize) //posestimering, som skulle bruges til bevægelse imellem chunks. bliver nu kun brugt til advarsel for for hurtig bevægelse
    {
        Vector3 posEstimateSmall = posS;
        Vector3Int posEstimateLarge = posL;

        posEstimateSmall += vel;

        bool runsOutOfChunck = false; //bliver ikke brugt, fordi det skulle være til chunks mekanikken
        Vector3Int posLargeAdd = new Vector3Int(0,0,0);
        Vector3Int posSmallAdd = new Vector3Int(0,0,0);

        if(posEstimateSmall.x < 0){
            runsOutOfChunck = true;
            posLargeAdd.x--;
            posSmallAdd.x += chunkSize;
        }
        if(posEstimateSmall.x > chunkSize){
            runsOutOfChunck = true;
            posLargeAdd.x++;
            posSmallAdd.x -= chunkSize;
        }
        if(posEstimateSmall.y < 0){
            runsOutOfChunck = true;
            posLargeAdd.y--;
            posSmallAdd.y += chunkSize;
        }
        if(posEstimateSmall.y > chunkSize){
            runsOutOfChunck = true;
            posLargeAdd.y++;
            posSmallAdd.y -= chunkSize;
        }
        if(posEstimateSmall.z < 0){
            runsOutOfChunck = true;
            posLargeAdd.z--;
            posSmallAdd.z += chunkSize;
        }
        if(posEstimateSmall.z > chunkSize){
            runsOutOfChunck = true;
            posLargeAdd.z++;
            posSmallAdd.z -= chunkSize;
        }
        posEstimateLarge += posLargeAdd;
        posEstimateSmall += posSmallAdd;

        Vector3 deltaPosSmall = vel;
        Vector3Int deltaPosLarge = posEstimateLarge - posL;
        

        bool movedTooFast = false;
        if(Mathf.Abs(deltaPosSmall.x) > chunkSize || Mathf.Abs(deltaPosSmall.y) > chunkSize || Mathf.Abs(deltaPosSmall.z) > chunkSize){
            movedTooFast = true;
        }
        if(movedTooFast){
            print("Warning: Particle moved too fast. Try increasing the densityCalcParticleController.invCycleTime variable");
            Vector3 floatDeltaPos = new Vector3(0,0,0);
            floatDeltaPos = deltaPosSmall + deltaPosLarge*densityCalcParticleController.chunckSize;
            Vector3 chuncksMoved = new Vector3(0,0,0);
            chuncksMoved = floatDeltaPos/densityCalcParticleController.chunckSize;
            print("A particle moved " + chuncksMoved + " chuncks");
            errorInCycle = true;
        }
    }
    public static bool calcDeltaVCol(Vector3 deltaPos, Vector3 deltaVelocity, int particleId, int mass, bool actualSim, int localId){//metode til den egentlige udregning af hastighedsændring ved kolision
        float px = deltaPos.x;
        float py = deltaPos.y;
        float pz = deltaPos.z;
        float vx = deltaVelocity.x;
        float vy = deltaVelocity.y;
        float vz = deltaVelocity.z;
        float angleCos =
            Vector3.Dot(deltaPos,deltaVelocity)
            /(deltaPos.magnitude*deltaVelocity.magnitude);
        float angle = Mathf.Acos(angleCos);
        if(!(angle >= Mathf.PI/2 || angle <= -Mathf.PI/2) && (angle <= 0 || angle > 0)){//hvis partiklerne flyver mod hinanden, så er kollisionen valid
            Vector3 momentum = mass * deltaVelocity;
            
            Vector3 projection;
            projection = angleCos*deltaPos.magnitude*deltaVelocity.normalized;
            Vector3 sidewaysDirection;
            sidewaysDirection = -(deltaPos-projection)/Vector3.Magnitude(deltaPos-projection);
            float angleSin = Mathf.Sin(angle);

            Vector3 neighborVelChange = momentum * angleCos / mass;//tilfører hastigheder
            if(neighborVelChange.magnitude > 0 || neighborVelChange.magnitude <= 0){
                if(!actualSim)
                    densityCalcParticleController.molecules[particleId].velToAdd.Add(neighborVelChange);
                else if(actualSim)
                    SimParticleController.molecules[particleId].velToAdd.Add(neighborVelChange);
            }

            Vector3 neighborSideChange = -momentum.magnitude * sidewaysDirection * angleSin / mass / 2;
            if(neighborSideChange.magnitude > 0 || neighborSideChange.magnitude <= 0){
                if(!actualSim)
                    densityCalcParticleController.molecules[particleId].velToAdd.Add(neighborSideChange);
                else if(actualSim)
                    SimParticleController.molecules[particleId].velToAdd.Add(neighborSideChange);
            }

            Vector3 localVelChange = -momentum * angleCos / mass;
            if(localVelChange.magnitude > 0 || localVelChange.magnitude <= 0){
                if(!actualSim)
                    densityCalcParticleController.molecules[localId].velToAdd.Add(localVelChange);
                else if(actualSim)
                    SimParticleController.molecules[localId].velToAdd.Add(localVelChange);
            }

            Vector3 localSideChange = momentum.magnitude * sidewaysDirection * angleSin / mass / 2;
            if(localSideChange.magnitude > 0 || localSideChange.magnitude <= 0){
                if(!actualSim)
                    densityCalcParticleController.molecules[localId].velToAdd.Add(localSideChange);
                else if(actualSim)
                    SimParticleController.molecules[localId].velToAdd.Add(localSideChange);
            }

            return true;
        }
        return false;
    }
    public static void particleCollisionSim(int mass, Vector3 posS, Vector3Int posL, Vector3 vel, int chunkSize, bool actualSim, int moleculeCount, int radius){//over-metode til partikkel kollision
        posEstimate(posS,posL,vel,chunkSize);
        for(int i = 0; i < moleculeCount; i++){
            bool ayyCollision = false;
            if((actualSim && posS != SimParticleController.molecules[i].posSmall) || (!actualSim && posS != densityCalcParticleController.molecules[i].posSmall)){
                Vector3 neighborVelocity = new Vector3();
                Vector3 deltaVelocity = new Vector3();
                Vector3 neighborPos = new Vector3();
                Vector3 deltaPos = new Vector3();
                if(actualSim){
                    neighborVelocity = SimParticleController.molecules[i].velocityReal;
                    deltaVelocity = SimParticleController.molecules[staticId].velocityReal - neighborVelocity;
                    neighborPos = SimParticleController.molecules[i].posSmall;
                    deltaPos = neighborPos - SimParticleController.molecules[staticId].posSmall;
                }
                else if(!actualSim){
                    neighborVelocity = densityCalcParticleController.molecules[i].velocityReal;
                    deltaVelocity = densityCalcParticleController.molecules[staticId].velocityReal - neighborVelocity;
                    neighborPos = densityCalcParticleController.molecules[i].posSmall;
                    deltaPos = neighborPos - densityCalcParticleController.molecules[staticId].posSmall;
                }
                if(deltaPos.magnitude < 2 * radius && deltaPos.magnitude != 0 && deltaVelocity.magnitude != 0){ //MIDLERTIDIGT skal bruge naboens radius
                    ayyCollision = calcDeltaVCol(deltaPos, deltaVelocity, i, mass, actualSim, staticId);
                }
            }
            if(ayyCollision)//hvis den allerede er kolideret, så stop for-lykken (forhindrer flere kollisioner på en gang, hvilket kan være uff)
                break;
        }
        
    }
}
