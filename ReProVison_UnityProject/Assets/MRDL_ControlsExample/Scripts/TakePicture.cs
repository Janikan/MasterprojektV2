/*
 * This class activates the camera and saves them as textures
 * 
 * In this case it displays the generated textures on an image collection in the scene
 * 
 * At the end of this class are the methods that invoke the methods and repeats them for generating multiple picures without any user interaction
 * 
*/

using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.VR.WSA.WebCam;

public class TakePicture : MonoBehaviour
{
    PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;
    public GameObject imageCollection;
    int numOfPictures = 0;
    public float takePictureIntervall; 
    private Vector3 cameraPos;
    private Vector3 cameraRot;

    //debug
    private Sprite mySprite;

    //LoggingChannel lc = new LoggingChannel("my provider", null, new Guid("4bd2826e-54a1-4ba9-bf63-92b73ea1ac4a"));
    //lc.LogMessage("I made a message!");

    // Use this for initialization
    void Start()
    {
        //Debug.Log("start");
    }

    public void startCapturing() {
        numOfPictures++;
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Create a PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });

        //get camera parameters: position, orientation
        cameraPos = GameObject.Find("HoloLens").transform.position;
        cameraRot = GameObject.Find("HoloLens").transform.rotation.eulerAngles;
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        SpriteRenderer[] children = imageCollection.GetComponentsInChildren<SpriteRenderer>();
        // children[1].color = Color.red;
        if (numOfPictures >= children.Length)
            numOfPictures = 1;
       
        // Copy the raw image data into the target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        //create hololensTexture for further processing e.g. generating environment map
        HoloLensTexture holoTexture = new HoloLensTexture(targetTexture, cameraPos, cameraRot);
        Debug.Log(numOfPictures + "th HoloLens Texture created at " + cameraPos.ToString() + " and "+cameraRot.ToString());
        //add previously generated texture t environment map
        GameObject.Find("EnvironmentMap").GetComponent<GenerateEnvironmentMap>().addTextureToEnvironmentMap(holoTexture);
        // Create a GameObject to which the texture can be applied
        //GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;

        //MaterialPropertyBlock block = new MaterialPropertyBlock();
        //block.SetTexture("_Texture", targetTexture);
        mySprite = Sprite.Create(targetTexture, new Rect(0.0f, 0.0f, targetTexture.width, targetTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        //children[numOfPictures].SetPropertyBlock(block);
        children[numOfPictures].sprite = mySprite;

        //Debug.Log("Name is 2 " + quad.name);
        //quadRenderer.material = new Material(Shader.Find("Specular"));

        //quad.transform.parent = this.transform;
        //quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);

      
       // quadRenderer.material.SetTexture("_MainTex", targetTexture);


        //Debug.Log("Done");

        // Deactivate the camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        //Debug.Log("Shut down photo mode");
        // Shutdown the photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    public void debug()
    {

        /* SpriteRenderer[] children = imageCollection.GetComponentsInChildren<SpriteRenderer>();
          Debug.Log("num Children " + children.Length);
          Debug.Log(children[1].name);
          Debug.Log("r "+children[1].color.r+" g "+children[1].color.g+" b "+children[1].color.b);

          children[1].color = Color.red;*/
        //photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode); 
        //Debug.Log("foto");
    }

    //start automated capturing, called in StartCaptureMode.cs
    public void activatePhotoCaptureMode()
    {
        InvokeRepeating("startCapturing", 0.0f, takePictureIntervall);
    }

    //stops automated capturing, called in StartCaptureMode.cs
    public void deactivatePhotoCaptureMode()
    {
        CancelInvoke();
    }
}