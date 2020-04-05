using UnityEngine;

public class Inputs : MonoBehaviour//samlet inputhandler klasse
{
    public static Vector3 corner1;
    public static Vector3 corner2;
    public static Vector3 meshTranslation;
    public static float chunksize;
    public static float publicChunksize;
    public static float publicAirSpeed;
    public float densityCalcTimeSeconds;
    public static float publicDensityCalcTime;
    public static float publicSimCalcTime;
    public long cyclesPerSecond;
    public static long publicCyclesPerSecond;
    public static float momentumToSIFactor;
    public static int densityTimeSwitchThreshold;
    public static int simTimeSwitchThreshold;
    public int fps;
    public Gradient preasureGradient;
    public static Gradient publicPreasureGradient;
    public float standartTargetPreassure;
    public static float publicStandartTargetPreassure;
    public static float publicTargetPreassure;
    public float standartAirSpeed;
    public float standartSimSeconds;
    public static float publicStandartAirSpeed;
    public static float publicStandartSimSeconds;
    void Update() //sætter inputs   
    {
        publicStandartTargetPreassure = standartTargetPreassure;
        publicStandartAirSpeed = standartAirSpeed;
        publicStandartSimSeconds = standartSimSeconds;
        densityCalcParticleController.targetPreassure = publicTargetPreassure;
        publicPreasureGradient = preasureGradient;
        Application.targetFrameRate = fps;
        densityTimeSwitchThreshold = (int) (publicCyclesPerSecond * publicDensityCalcTime * densityCalcParticleController.logForceTime);
        simTimeSwitchThreshold = (int) (publicCyclesPerSecond*publicSimCalcTime*SimParticleController.logForceTime);
        momentumToSIFactor = publicCyclesPerSecond * Mathf.Pow(10,-12) * 1.661f * Mathf.Pow(10,-27);
        publicCyclesPerSecond = cyclesPerSecond;
        publicDensityCalcTime = densityCalcTimeSeconds;
        GameObject mesh = GameObject.FindGameObjectWithTag("Mesh");
        meshTranslation = mesh.transform.position;
        GameObject particleBorders = GameObject.FindGameObjectWithTag("ParticleBorders");
        if(particleBorders.transform.localScale.x == MeshCentralicer.xScale){
            corner1 = particleBorders.transform.position - particleBorders.transform.localScale/2;
            corner2 = particleBorders.transform.position + particleBorders.transform.localScale/2;
            publicChunksize = chunksize;
        }
    }
}