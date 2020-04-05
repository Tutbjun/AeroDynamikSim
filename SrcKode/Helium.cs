using System.Collections.Generic;
using UnityEngine;

public class Helium : Particle //overtype til alle partikler, som på nuværende tidspunkt er i programmet
{
    public override int localMass{
        get{
            return 4002602;//masse af helium i Megaunits
        }
    }
    public override int localRadius{
        get{
            return 140;//radius af objekt
        }
    }
}