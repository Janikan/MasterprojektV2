/*
 * Class that defines a Hololens Texture
 * 
 * Basically the photo that was captured by the HoloLens RGB camera as texture together with the information
   about position and orientation of the camera when it was captured
 * 
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloLensTexture : MonoBehaviour {

    private Texture2D holoTex;
    private Vector3 holoPos;
    private Vector3 holoRot;


    //constructor
    public HoloLensTexture(Texture2D holoTex, Vector3 holoPos, Vector3 holoRot)
    {
        this.holoTex = holoTex;
        this.holoPos = holoPos;
        this.holoRot = holoRot;
    }

    public Vector3 getCapturePosition()
    {
        return holoPos;
    }

    public Vector3 getCaptureOrientation()
    {
        return holoRot;
    }

    public Texture2D getTexture()
    {
        return holoTex;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
