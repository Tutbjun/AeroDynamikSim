using System.Collections.Generic;
using UnityEngine;

public class MeshDivider : MonoBehaviour //dividerer mesh i chunks (bliver ikke brugt)
{
    public GameObject meshPartPrefab;
    int meshPartsCreated = 0;

    void Update()
    {
        int invix = 0;
        if(Inputs.corner1 != null){
            for(int ix = (1 + (int) ((Inputs.corner2.x - Inputs.corner1.x)/ ((float) Inputs.publicChunksize))); ix > 0; ix--){ //x
                invix++;    
                int inviy = 0;
                for(int iy = (1 + (int) ((Inputs.corner2.y - Inputs.corner1.y)/ ((float) Inputs.publicChunksize))); iy > 0; iy--){ //y
                    inviy++;
                    int inviz = 0;
                    for(int iz = (1 + (int) ((Inputs.corner2.z - Inputs.corner1.z)/ ((float) Inputs.publicChunksize))); iz > 0; iz--){ //z
                        inviz++;

                        List<int> chunkVerticiesIndex = new List<int>();//alle vertices i denne chunk

                        for(int i = 0; i < BareMesh.mesh.vertices.Length; i++){
                            if(BareMesh.mesh.vertices[i].x > Inputs.corner1.x + (invix - 1) * Inputs.publicChunksize && BareMesh.mesh.vertices[i].x < Inputs.corner1.x + invix * Inputs.publicChunksize){//hvis den er indenfor chunken
                                if(BareMesh.mesh.vertices[i].y > Inputs.corner1.y + (inviy - 1) * Inputs.publicChunksize && BareMesh.mesh.vertices[i].y < Inputs.corner1.y + inviy * Inputs.publicChunksize){//hvis den er indenfor chunken
                                    if(BareMesh.mesh.vertices[i].z > Inputs.corner1.z + (inviz - 1) * Inputs.publicChunksize && BareMesh.mesh.vertices[i].z < Inputs.corner1.z + inviz * Inputs.publicChunksize){//hvis den er indenfor chunken
                                        chunkVerticiesIndex.Add(i);
                                    }
                                }
                            }
                        }
                        List<int> verticesToAdd = new List<int>();
                        for(int i = 0; i < BareMesh.mesh.triangles.Length/3; i++){
                            bool[] iChunck = new bool[3];
                            for(int i2 = 0; i2 < 3; i2++){
                                foreach(var e in chunkVerticiesIndex){
                                    if(BareMesh.mesh.triangles[i*3 + i2] == e){//hvis en af trekanterneindexværdierne macher med hvad der er i chunken
                                        iChunck[i2] = true;
                                    }
                                }
                            }
                            if(iChunck[0] != iChunck[1] || iChunck[1] != iChunck[2] || iChunck[0] != iChunck[2]){//så overlapper en trekant med en chunkgrænse
                                //og så skal resten af trekanten med
                                for(int i2 = 0; i2 < 3; i2++){
                                    if(!iChunck[i2]){
                                        verticesToAdd.Add(BareMesh.mesh.triangles[i*3 + i2]);
                                    }
                                }
                            }
                        }
                        foreach(var e in verticesToAdd){
                            bool duplicate = false;
                            foreach(var e2 in chunkVerticiesIndex){
                                if(e == e2){
                                    duplicate = true;
                                }
                            }
                            if(!duplicate){
                                chunkVerticiesIndex.Add(e);
                            }
                        }
                        //Nu burde der være en komplet indexlist med alle punkter i chunken
                        List<int> chunckTrianglesList = new List<int>();

                        for(int i = 0; i < BareMesh.mesh.triangles.Length/3; i++){
                            for(int i2 = 0; i2 < chunkVerticiesIndex.Count; i2++){
                                if(BareMesh.mesh.triangles[i*3] == chunkVerticiesIndex[i2]){
                                    bool overlappingTriangle = false;
                                    foreach(var e in verticesToAdd){
                                        if(e == chunkVerticiesIndex[i2]){
                                            overlappingTriangle = true;
                                        }
                                    }
                                    if(!overlappingTriangle){
                                        for(int i3 = 0; i3 < 3; i3++){
                                            chunckTrianglesList.Add(BareMesh.mesh.triangles[i*3 + i3]);
                                        }
                                    }
                                }
                            }
                        }
                        
                        Vector3[] voxelPoints = new Vector3[6*4];
                        voxelPoints[0] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);
                        voxelPoints[1] = new Vector3((invix) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);
                        voxelPoints[2] = new Vector3((invix) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);
                        voxelPoints[3] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);

                        voxelPoints[4] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);
                        voxelPoints[5] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);
                        voxelPoints[6] = new Vector3((invix) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);
                        voxelPoints[7] = new Vector3((invix) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);

                        voxelPoints[8] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);
                        voxelPoints[9] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);
                        voxelPoints[10] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);
                        voxelPoints[11] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);

                        voxelPoints[12] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);
                        voxelPoints[13] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);
                        voxelPoints[14] = new Vector3((invix) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);
                        voxelPoints[15] = new Vector3((invix) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);

                        voxelPoints[16] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);
                        voxelPoints[17] = new Vector3((invix) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);
                        voxelPoints[18] = new Vector3((invix) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);
                        voxelPoints[19] = new Vector3((invix - 1) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);

                        voxelPoints[20] = new Vector3((invix) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);
                        voxelPoints[21] = new Vector3((invix) * Inputs.publicChunksize, (inviy - 1) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);
                        voxelPoints[22] = new Vector3((invix) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz) * Inputs.publicChunksize);
                        voxelPoints[23] = new Vector3((invix) * Inputs.publicChunksize, (inviy) * Inputs.publicChunksize, (inviz - 1) * Inputs.publicChunksize);

                        for(int i = 0; i < BareMesh.mesh.triangles.Length / 3; i++){

                            int[] triangle = new int[3];
                            for(int i2 = 0; i2 < 3; i2++){
                                triangle[i2] = BareMesh.mesh.triangles[i*3 + i2];
                            }

                            bool[] trekantIPlan = new bool[6];
                            for(int i2 = 0; i2 < 6; i2++){

                                Vector3 normalVector1 = new Vector3();
                                Vector3 crossVector11 = voxelPoints[i2*4 + 1] - voxelPoints[i2*4];
                                Vector3 crossVector21 = voxelPoints[i2*4 + 3] - voxelPoints[i2*4];
                                normalVector1 = Vector3.Cross(crossVector11,crossVector21);
                                normalVector1 = normalVector1.normalized;

                                bool[] positiv = new bool[3];
                                for(int i3 = 0; i3 < 3; i3++){//se maple fil
                                    float x;
                                    Vector3 p = voxelPoints[i2*4];
                                    Vector3 v = BareMesh.mesh.vertices[triangle[i3]];
                                    Vector3 n = normalVector1;
                                    x = -(n.x*p.x - n.x*v.x + n.y*p.y - n.y*v.y + n.z*p.z - n.z*v.z) / (Mathf.Pow(n.x,2) + Mathf.Pow(n.y,2) + Mathf.Pow(n.z,2));
                                    if(x > 0){
                                        positiv[i3] = true;
                                    }
                                    else if(x <= 0){
                                        positiv[i3] = false;
                                    }
                                }
                                if(positiv[0] != positiv[1] || positiv[0] != positiv[2] || positiv[2] != positiv[1]){
                                    Vector3 normalVector2 = new Vector3();
                                    Vector3 crossVector12 = BareMesh.mesh.vertices[triangle[1]] - BareMesh.mesh.vertices[triangle[0]];
                                    Vector3 crossVector22 = BareMesh.mesh.vertices[triangle[2]] - BareMesh.mesh.vertices[triangle[0]];
                                    normalVector2 = Vector3.Cross(crossVector12,crossVector22);
                                    normalVector2 = normalVector2.normalized;

                                    for(int i3 = 0; i3 < 3; i3++){//se maple fil
                                        float x;
                                        Vector3 v = voxelPoints[i2*4];
                                        Vector3 p = BareMesh.mesh.vertices[triangle[i3]];
                                        Vector3 n = normalVector2;
                                        x = -(n.x*p.x - n.x*v.x + n.y*p.y - n.y*v.y + n.z*p.z - n.z*v.z) / (Mathf.Pow(n.x,2) + Mathf.Pow(n.y,2) + Mathf.Pow(n.z,2));
                                        if(x > 0){
                                            positiv[i3] = true;
                                        }
                                        else if(x <= 0){
                                            positiv[i3] = false;
                                        }
                                    }
                                    if(positiv[0] != positiv[1] || positiv[0] != positiv[2] || positiv[2] != positiv[1]){
                                        Vector3[] firkantHjørneVector = new Vector3[4];
                                        for(int i3 = 0; i3 < 4; i3++){
                                            int i4 = i3 + 1;
                                            if(i4 == 4) i4 = 0;
                                            firkantHjørneVector[i3] = voxelPoints[i2*4+i4] - voxelPoints[i2*4+i3];
                                        }
                                        
                                        float[,] vinkler = new float[3,4];
                                        for(int i4 = 0; i4 < 3; i4++){
                                            for(int i3 = 0; i3 < 4; i3++){
                                                Vector3 DotVector = BareMesh.mesh.vertices[triangle[i4]] - voxelPoints[i2*4+i3];
                                                vinkler[i4,i3] = Mathf.Acos( Vector3.Dot( firkantHjørneVector[i3], DotVector ) / ( DotVector.magnitude * firkantHjørneVector[i3].magnitude ) );
                                            }
                                        }
                                        bool[] indenforFirkant = new bool[3];
                                        for(int i3 = 0; i3 < 3; i3++){
                                            indenforFirkant[i3] = true;
                                            for(int i4 = 0; i4 < 4; i4++){
                                                if(vinkler[i3,i4] > Mathf.PI/2){
                                                    indenforFirkant[i3] = false;
                                                }
                                            }
                                        }
                                        //hvis en af indeforFirkant er true, så skal trekanten tilføjes
                                        bool indenforFirkantDefinate = false;
                                        for(int i3 = 0; i3 < 3; i3++){
                                            if(indenforFirkant[i3] == true){
                                                indenforFirkantDefinate = true;
                                            }
                                        }
                                        if(indenforFirkantDefinate){
                                            bool duplicate = false;
                                            for(int i3 = 0; i3 < chunckTrianglesList.Count/3; i3++){
                                                if(triangle[i3] == chunckTrianglesList[i3] && triangle[i3+1] == chunckTrianglesList[i3+1] && triangle[i3+2] == chunckTrianglesList[i3+2]){
                                                    duplicate = true;
                                                }
                                            }
                                            if(!duplicate){
                                                foreach(var e in triangle){
                                                    chunckTrianglesList.Add(e);
                                                    
                                                    bool verticieDuplicate = false;
                                                    for(int i3 = 0; i3 < chunkVerticiesIndex.Count; i3++){
                                                        if(chunkVerticiesIndex[i3] == e){
                                                            verticieDuplicate = true;
                                                        }
                                                    }
                                                    if(!verticieDuplicate){
                                                        chunkVerticiesIndex.Add(e);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //konvertering til vector3 array
                        Vector3[] chunkVerticies = new Vector3[chunkVerticiesIndex.Count];
                        for(int i = 0; i < chunkVerticies.Length; i++){
                            chunkVerticies[i] = BareMesh.mesh.vertices[chunkVerticiesIndex[i]];
                        }
                        
                        //konvertering til int array
                        int[] chunkTriangles = new int[chunckTrianglesList.Count];
                        for(int i = 0; i < chunkTriangles.Length; i++){
                            for(int i2 = 0; i2 < chunkVerticies.Length; i2++){
                                if(chunkVerticiesIndex[i2] == chunckTrianglesList[i]){
                                    chunkTriangles[i] = i2;
                                }
                            }
                        }

                        //objekt med meshdelen dannes
                        if(chunkVerticies.Length > 0){
                            GameObject aMeshPart = Instantiate(meshPartPrefab) as GameObject;
                            aMeshPart.GetComponent<MeshFilter>().mesh.vertices = chunkVerticies;
                            aMeshPart.GetComponent<MeshFilter>().mesh.triangles = chunkTriangles;
                            aMeshPart.transform.position += Inputs.meshTranslation;
                            meshPartsCreated++;
                        }

                    }            
                }
            }
        }
        if(GameObject.FindGameObjectsWithTag("meshPart").Length > 0)
        GameObject.Destroy(this);
    }
}