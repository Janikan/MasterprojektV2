
/*
 * Class for Button, that start a method in another script on click and stops on another click
 * 
 * In this case especially, it starts/stops the capture mode of the camera that takes pictures every x-seconds, implemented in TakePicture.cs
 * 
 */
using UnityEngine;

namespace HUX.Buttons
{
    [RequireComponent(typeof(CompoundButton))]
    public class StartCaptureMode : ProfileButtonBase<ButtonSoundProfile>
    {
        private Button.ButtonStateEnum lastState = Button.ButtonStateEnum.Disabled;
        private bool pressed = false;
        private bool goInto = true;
        private TakePicture takePic;

        void Start()
        {
            Button button = GetComponent<Button>();
            button.OnButtonPressed += OnButtonPressed;
            button.StateChange += StateChange;
            button.OnButtonReleased += OnButtonReleased;

            takePic = GameObject.Find("HoloLens").transform.FindChild("RGBCamera").GetComponent<TakePicture>();
        }

        void StateChange(Button.ButtonStateEnum newState)
        {
            // Don't play the same state multiple times
            if (lastState == newState)
                return;

            lastState = newState;

            // Don't play sounds for inactive buttons
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
                return;

        }

        void OnButtonPressed(GameObject go)
        {
            // PlayClip(Profile.ButtonPressed, Profile.ButtonPressedVolume);
            if (goInto)
            {
                if (!pressed)
                {
                    Debug.Log("start");
                    takePic.activatePhotoCaptureMode();
                }
                else
                {
                    Debug.Log("stop");
                    takePic.deactivatePhotoCaptureMode();
                }
                goInto = false;
                pressed = !pressed;
            }
           // Debug.Log(goInto);
        }
           // Debug.Log(Time.realtimeSinceStartup);
           

        void OnButtonReleased(GameObject go)
        {
            goInto = true;
           // Debug.Log("rel");
        }

    }
}