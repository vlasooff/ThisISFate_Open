using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseTool : MonoBehaviour
{
    public Material[] materials;
    public Material windows;
    public MeshRenderer house;

    [EasyButtons.Button]
    public void RandomHouse()
    {
        house.sharedMaterials[0] = materials[Random.Range(0, materials.Length)];
        house.sharedMaterials[1] = materials[Random.Range(0, materials.Length)];
    }
    [EasyButtons.Button]
    public void SetWindows()
    {
        
        house = new MeshRenderer();
        house.material = windows;
        house.UpdateGIMaterials();
    }
}
