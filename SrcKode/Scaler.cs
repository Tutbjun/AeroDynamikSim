using UnityEngine;

public class Scaler : MonoBehaviour //skalerer mesh og grænsen til brugbare størrelser til simulering
{
    public float scaler;
    public static float publicScaler = 1;
    public static bool scaled = false;
    void Start()
    {
        publicScaler = scaler;
        var e = GameObject.FindGameObjectWithTag("Mesh");
        e.transform.position *= scaler;
        e.transform.localScale *= scaler;
        e = GameObject.FindGameObjectWithTag("ParticleBorders");
        e.transform.localScale *= scaler;
        e.transform.position *= scaler;
        scaled = true;
    }
}