/*
 * Class that generates an environment map out of multiple HololensTexture from the class HoloLensTextures.cs
 * 
 * 
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Unity;

public class GenerateEnvironmentMap : MonoBehaviour
{

    private List<HoloLensTexture> textures;
    private SpatialMappingManager spatialMapping;

    // Use this for initialization
    void Start()
    {
        textures = new List<HoloLensTexture>();
        spatialMapping = GameObject.Find("HoloLens").GetComponentInChildren<SpatialMappingManager>();

    }

    public void addTextureToEnvironmentMap(HoloLensTexture holoTex)
    {
        textures.Add(holoTex);
        Debug.Log("Successfully added photo to environment map. Total Size " + textures.Count);
    }

    // Update is called once per frame
    void Update()
    {
        //Get Spatil information of the currently visible real content
        //currently every scan creates a new list
        List<MeshFilter> meshFilters = SpatialMappingManager.Instance.GetMeshFilters();
       // Debug.Log("Mesh Filters: " + meshFilters.Count);
    }
}

