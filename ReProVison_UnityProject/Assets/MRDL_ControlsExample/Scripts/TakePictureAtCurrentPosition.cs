﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.WebCam;
using UnityEngine.VR.WSA.Input;

public class TakePictureAtCurrentPosition : MonoBehaviour
{
    GestureRecognizer m_GestureRecognizer;
    GameObject m_Canvas = null;
    Renderer m_CanvasRenderer = null;
    PhotoCapture m_PhotoCaptureObj;
    CameraParameters m_CameraParameters;
    //bool m_CapturingPhoto = false;
    Texture2D m_Texture = null;
    bool m_updateTexture = false;

    private GameObject mHololens;
    private GameObject mEnvironmentMap;
    private Vector3 mCameraPos;
    private Vector3 mCameraRot;
    private int numOfPictures;

    void Start()
    {
        Initialize();
    }

    void SetupGestureRecognizer()
    {
        m_GestureRecognizer = new GestureRecognizer();
        m_GestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        m_GestureRecognizer.TappedEvent += OnTappedEvent;
        m_GestureRecognizer.StartCapturingGestures();

      //  m_CapturingPhoto = false;
    }

    void Initialize()
    {
        //Debug.Log("Initializing...");
        List<Resolution> resolutions = new List<Resolution>(PhotoCapture.SupportedResolutions);
        Resolution selectedResolution = resolutions[0];

        m_CameraParameters = new CameraParameters(WebCamMode.PhotoMode);
        m_CameraParameters.cameraResolutionWidth = selectedResolution.width;
        m_CameraParameters.cameraResolutionHeight = selectedResolution.height;
        //default mode 1280x720
       // Debug.Log(selectedResolution.width + " x " + selectedResolution.height);
        m_CameraParameters.hologramOpacity = 0.0f;
        m_CameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

        m_Texture = new Texture2D(selectedResolution.width, selectedResolution.height, TextureFormat.BGRA32, false);

        PhotoCapture.CreateAsync(false, OnCreatedPhotoCaptureObject);
    }

    void OnCreatedPhotoCaptureObject(PhotoCapture captureObject)
    {
        m_PhotoCaptureObj = captureObject;
        m_PhotoCaptureObj.StartPhotoModeAsync(m_CameraParameters, OnStartPhotoMode);
    }

    void OnStartPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        SetupGestureRecognizer();

        mHololens = GameObject.Find("HoloLens");
        mEnvironmentMap = GameObject.Find("EnvironmentMap");
        numOfPictures = 0;

        //Debug.Log("Ready!");
        //Debug.Log("Air Tap to take a picture.");
    }

public void OnTappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        Debug.Log("Tap On Fucking Button");
    }

    public void testActivation()
    {
        /* Debug.Log("test2");
         if (m_CapturingPhoto)
         {
             return;
         }

         m_CapturingPhoto = true;*/
        // Debug.Log("Taking picture...");
        if (m_PhotoCaptureObj != null)
        {
            m_PhotoCaptureObj.TakePhotoAsync(OnPhotoCaptured);
        }
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("Shut down photo mode");
        // Shutdown the photo capture resource
        m_PhotoCaptureObj.Dispose();
        m_PhotoCaptureObj = null;
    }


    void OnPhotoCaptured(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (!m_updateTexture)
        {
            if (m_Canvas == null)
            {
                m_Canvas = GameObject.CreatePrimitive(PrimitiveType.Quad);
                m_Canvas.GetComponent<Collider>().enabled = false;
               // float scale = (Screen.height / 2.0f) / Camera.main.orthographicSize;
               // float scaleX = m_Texture.width / scale;
               // float scaleY = m_Texture.height / scale;
               // m_Canvas.transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
                m_Canvas.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                m_Canvas.name = "PhotoCaptureCanvas";
                m_CanvasRenderer = m_Canvas.GetComponent<Renderer>() as Renderer;
                try
                {
                    //in debug mode:System.NullReferenceException
                    m_CanvasRenderer.material = new Material(Shader.Find("AR/HolographicImageBlend"));
                }
                catch(System.NullReferenceException)
                {
                    Debug.Log("Problem with Shader when Debugging");
                }
            }

            Matrix4x4 cameraToWorldMatrix;
            photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
            Matrix4x4 worldToCameraMatrix = cameraToWorldMatrix.inverse;

            Matrix4x4 projectionMatrix;
            photoCaptureFrame.TryGetProjectionMatrix(out projectionMatrix);

            photoCaptureFrame.UploadImageDataToTexture(m_Texture);
            //m_Texture.wrapMode = TextureWrapMode.Clamp;

            m_CanvasRenderer.sharedMaterial.SetTexture("_MainTex", m_Texture);
            m_CanvasRenderer.sharedMaterial.SetMatrix("_WorldToCameraMatrix", worldToCameraMatrix);
            m_CanvasRenderer.sharedMaterial.SetMatrix("_CameraProjectionMatrix", projectionMatrix);
            m_CanvasRenderer.sharedMaterial.SetFloat("_VignetteScale", 0.5f);

            // Position the canvas object slightly in front
            // of the real world web camera.
            Vector3 position = cameraToWorldMatrix.GetColumn(3) - cameraToWorldMatrix.GetColumn(2);

            // Rotate the canvas object so that it faces the user.
            Quaternion rotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));

            m_Canvas.transform.position = position;
            m_Canvas.transform.rotation = rotation;
            m_updateTexture = true;
        }
        else
        {
            photoCaptureFrame.UploadImageDataToTexture(m_Texture);
            m_Texture.wrapMode = TextureWrapMode.Clamp;

            m_CanvasRenderer.sharedMaterial.SetTexture("_MainTex", m_Texture);

            Matrix4x4 cameraToWorldMatrix;
            photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
            Vector3 position = cameraToWorldMatrix.GetColumn(3) - cameraToWorldMatrix.GetColumn(2);
            Quaternion rotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));
            m_Canvas.transform.position = position;
            m_Canvas.transform.rotation = rotation;

            //get orientation and rotatiopn of hololens
            mCameraPos = mHololens.transform.position;
            mCameraRot = mHololens.transform.rotation.eulerAngles;

            //create hololensTexture for further processing e.g. generating environment map
            HoloLensTexture holoTexture = new HoloLensTexture(m_Texture, mCameraPos, mCameraRot);
            Debug.Log(numOfPictures + "th HoloLens Texture created at " + mCameraPos.ToString() + " and " + mCameraRot.ToString());
            //add previously generated texture t environment map
            mEnvironmentMap.GetComponent<GenerateEnvironmentMap>().addTextureToEnvironmentMap(holoTexture);

        }

        numOfPictures++;
       //Debug.Log("Took picture!");
       //m_CapturingPhoto = false;
    }

    //start automated capturing, called in StartCaptureMode.cs
    public void activatePhotoCaptureMode()
    {
        //testActivation();
        if(m_PhotoCaptureObj == null)
        {
            Initialize();
        }
        InvokeRepeating("testActivation", 0.0f, 0.5f);
    }

    //stops automated capturing, called in StartCaptureMode.cs
    public void deactivatePhotoCaptureMode()
    {
        //CancelInvoke();
        if (m_PhotoCaptureObj != null)
        {
            m_PhotoCaptureObj.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
        CancelInvoke();

    }


}