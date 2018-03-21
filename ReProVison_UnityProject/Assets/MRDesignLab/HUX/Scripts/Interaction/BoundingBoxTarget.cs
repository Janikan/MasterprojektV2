//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEngine;

using UnityEngine.VR.WSA.Input;


namespace HUX.Interaction
{
    /// <summary>
    /// This script assists in using a bounding box to target objects
    /// Bounding boxes and manipulation toolbars can both be used without this script
    /// But this makes it easier to use a single bounding box to target multiple objects
    /// as well as to specify per-target display options and operations
    /// </summary>
    [RequireComponent (typeof (CompoundButton))]
    public class BoundingBoxTarget : MonoBehaviour {

        /// <summary>
        /// Tags to use when selected / deselected
        /// This should be set to something the FocusManager will ignore
        /// Otherwise the colliders from this object may occlude bounding box
        /// </summary>
        public FilterTag TagOnSelected;
        public FilterTag TagOnDeselected;

        /// <summary>
        /// Which operations will be permitted when the bounding box targets this object
        /// </summary>
        [HideInInspector]
        public BoundingBoxManipulate.OperationEnum PermittedOperations = BoundingBoxManipulate.OperationEnum.Drag | BoundingBoxManipulate.OperationEnum.ScaleUniform | BoundingBoxManipulate.OperationEnum.RotateY;

        public BoundingBoxManipulate.FlattenModeEnum FlattenPreference = BoundingBox.FlattenModeEnum.FlattenAuto;

        public BoundingBox.BoundsCalculationMethodEnum BoundsCalculationMethod = BoundingBox.BoundsCalculationMethodEnum.RendererBounds;

        /// <summary>
        /// Whether to show the manipulation display when the bounding box targets this object
        /// </summary>
        [HideInInspector]
        public bool ShowAppBar = true;

        /// <summary>
        /// Bounding box to use. If this is not set, the first bounding box in the scene will be used.
        /// </summary>
        private BoundingBoxManipulate boundingBox;

        /// <summary>
        /// Manipulation toolbar to use. If this is not set, the first toolbar in the scene will be uesd.
        /// </summary>
        private AppBar toolbar;

        //private Button menuButton;

        private GameObject m_ScrollView;

        private InteractionManager interactionManager;

        private void Start()
        {
            Button button = GetComponent<Button>();
            button.FilterTag = TagOnDeselected;

            m_ScrollView = GameObject.Find("ScrollView_Canvas");
            if (m_ScrollView != null)
            {
                m_ScrollView.gameObject.SetActive(false);
            }

           /* menuButton = GameObject.Find("OpenMenuButton").GetComponent<Button>();
            if (menuButton != null)
                menuButton.gameObject.SetActive(false);*/

            //get interactionManager
            interactionManager = GameObject.Find("HoloLens").transform.FindChild("InteractionManager").GetComponent<InteractionManager>();

        }



        public void OnTargetDeselected ()
        {
            Debug.Log("Deselecting target " + name);
            GetComponent<Button>().FilterTag = TagOnDeselected;

           /* if(menuButton != null)
            {
                menuButton.gameObject.SetActive(false);
            }*/

            //Hack for having those two objects nir visible anymore when user taps in free space
            //occure back in when user taps on object (or other object with bodung box) again
           /* GameObject bBoxShell = GameObject.Find("BoundingBoxShell(Clone)");
            if(bBoxShell != null)
            {
                bBoxShell.SetActive(false);
            }
            GameObject appBar = GameObject.Find("AppBar(Clone)");
            if(appBar != null)
            {
                appBar.SetActive(false);
            }*/
        }
        
        public void Tapped()
        {
            Debug.Log("Tap");
            // Return if there isn't a Manipulation Manager

            if(interactionManager != null)
            {
                interactionManager.setExistingBoundingBoxTarget(true);
            }

            if (ManipulationManager.Instance == null)
            {
                Debug.LogError("No manipulation manager for " + name);
                return;
            }

            // Try to find our bounding box
            if (boundingBox == null)
            {
                boundingBox = ManipulationManager.Instance.ActiveBoundingBox;
            }

            // Try to find our toolbar
            if (toolbar == null)
            {
                toolbar = ManipulationManager.Instance.ActiveAppBar;
                if (m_ScrollView != null)
                {
                    toolbar.setScrollView(m_ScrollView);
                }
            }

            // Try to find menuButton
          /*  if (menuButton != null)
            {
                menuButton.gameObject.SetActive(true);
                if (toolbar != null)
                {
                    menuButton.transform.parent = toolbar.transform;
                    menuButton.transform.position = new Vector3(0,0.2f,0);
                }*/

                //for positioning the menu button beside the prodct object and facing the camera; every time it appears again
              /*  GameObject cam = GameObject.Find("HoloLens");
                GameObject product = GameObject.Find("Product");
                Vector3 camRightVector = new Vector3(cam.transform.position.x, 0, 0).normalized;
                menuButton.transform.position = product.transform.position;
                menuButton.gameObject.transform.Translate(camRightVector * 0.2f);*/

           // }


            // If we've already got a bounding box and it's pointing to us, do nothing
            if (boundingBox != null && boundingBox.gameObject.activeSelf && boundingBox.Target == this.gameObject)
                return;

            // Set the bounding box's target and permitted operations
            boundingBox.PermittedOperations = PermittedOperations;
            boundingBox.FlattenPreference = FlattenPreference;
            boundingBox.Target = gameObject;
            boundingBox.BoundsMethodOverride = BoundsCalculationMethod;
            boundingBox.gameObject.SetActive(true);

            if (ShowAppBar)
            {
                // Show it and set its bounding box object
                toolbar.BoundingBox = boundingBox;
                toolbar.Reset();
                toolbar.gameObject.SetActive(true);
                toolbar.HoverOffsetYScale = -0.25f;

            } else if (toolbar != null)
            {
                // Set its bounding box to null to hide it
                toolbar.BoundingBox = null;
                // Set to accept input immediately
                //boundingBox.AcceptInput = true;
            }
            //GetComponent<TakePicture>().startCapturing();
        }

        private void OnDestroy ()
        {
            if (boundingBox != null && boundingBox.Target == this)
                boundingBox.Target = null;
        }
    }
}
