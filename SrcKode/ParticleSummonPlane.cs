using UnityEngine;

public class ParticleSummonPlane : MonoBehaviour //sætter bare positionen af det røde plan visuelt
{
    void Update()
    {
        transform.rotation = ParticleBordersPositioner.planeRot;
        transform.position = ParticleBordersPositioner.planePos;
        transform.localScale = ParticleBordersPositioner.planeScale;
    }
}