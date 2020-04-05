using UnityEngine;

public class UIDeleter : MonoBehaviour//fjerner alle ui elementer (og fjerner midlertidigt mesh og grænse), og fjerner derefter sig selv
{
    void Update()
    {
        GameObject[] UIElements = GameObject.FindGameObjectsWithTag("UI");
        foreach(var e in UIElements){
            GameObject.Destroy(e);
        }
        GameObject mesh = GameObject.FindGameObjectWithTag("Mesh");
        mesh.GetComponent<MeshCollider>().enabled = false;
        GameObject particleBorders = GameObject.FindGameObjectWithTag("ParticleBorders");
        particleBorders.GetComponent<BoxCollider>().enabled = false;
        GameObject me = GameObject.FindGameObjectWithTag("UIDeleter");
        GameObject.Destroy(me);
    }
}