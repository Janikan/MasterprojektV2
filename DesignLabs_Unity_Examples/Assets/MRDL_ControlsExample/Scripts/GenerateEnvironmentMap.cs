/*
 * Class that generates an environment map out of multiple HololensTexture from the class HoloLensTextures.cs
 * 
 * 
 * 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateEnvironmentMap : MonoBehaviour
{

    private List<HoloLensTexture> textures;

    // Use this for initialization
    void Start()
    {
        textures = new List<HoloLensTexture>();
    }

    public void addTextureToEnvironmentMap(HoloLensTexture holoTex)
    {
        textures.Add(holoTex);
        Debug.Log("Successfully added photo to environment map. Total Size "+textures.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
