using UnityEngine;

public class ParticleSummoner : MonoBehaviour//klassen som sørger for partikkelinstantiering til simuleringen
{
    float dynCycleTime = SimParticleController.dynamicInvCycleTime;
    bool starting = true;
    void Update()
    {
        if(((SimParticleController.debugMode && Input.GetKey(KeyCode.Space)) || (!SimParticleController.debugMode && (dynCycleTime - SimParticleController.cyclesToParticleSummon) > SimParticleController.dynamicInvCycleTime) || starting) && dynCycleTime > 0){
            starting = false;
            dynCycleTime -= SimParticleController.cyclesToParticleSummon;//udregner partikkelantal, hvor meget afstand der skal være mellem dem osv.
            float volume = ParticleBordersPositioner.myScale.x * ParticleBordersPositioner.myScale.y * ParticleBordersPositioner.myScale.z;
            float qubeVolume = Mathf.Pow(volume,0.333f);
            float qubeParticleAmount = Mathf.Pow(SimParticleController.particlesPrVol * volume,0.333f);
            
            float xScaler = ParticleBordersPositioner.myScale.x / qubeVolume;
            float yScaler = ParticleBordersPositioner.myScale.y / qubeVolume;
        
            float particlesX = xScaler * qubeParticleAmount;
            float particlesY = yScaler * qubeParticleAmount;
            int particlesXint = (int) (particlesX + 0.5f);
            int particlesYint = (int) (particlesY + 0.5f);
            float particleDistanceX = ParticleBordersPositioner.myScale.x/particlesX;
            float particleDistanceY = ParticleBordersPositioner.myScale.y/particlesY;
            
            SimParticleController.shouldRun = false;
            for(int ix = 0; ix < particlesXint; ix++){//skaber et grid af partikler (man kan måske kalde det en bølge)
                for(int iy = 0; iy < particlesYint; iy++){
                    GameObject particle = GameObject.Instantiate(SimParticleController.publicHelium);
                    particle.transform.position = new Vector3((ix+0.5f)*particleDistanceX - ParticleBordersPositioner.myScale.x/2,
                        (iy+0.5f)*particleDistanceY - ParticleBordersPositioner.myScale.y/2,200);
                    particle.transform.position = ParticleBordersPositioner.planeRot * particle.transform.position;
                    particle.transform.position += ParticleBordersPositioner.planePos;
                    SimParticleController.molecules.Add(particle.GetComponent<ParticleForSim>());
                }
            }
        }
    }
}