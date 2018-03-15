
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
    public class OpenMenu : ProfileButtonBase<ButtonSoundProfile>
    {
        private Button.ButtonStateEnum lastState = Button.ButtonStateEnum.Disabled;
        private bool pressed = false;
        private bool goInto = true;

        public GameObject scrollView;

        void Start()
        {
            Button button = GetComponent<Button>();
            button.OnButtonPressed += OnButtonPressed;
            button.StateChange += StateChange;
            button.OnButtonReleased += OnButtonReleased;

            scrollView.SetActive(false);
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
            // takePicCur.testActivation();
            // PlayClip(Profile.ButtonPressed, Profile.ButtonPressedVolume);
            if (goInto)
            {
                if (!pressed)
                {
                    this.GetComponentInChildren<TextMesh>().text = "Close Selection";
                    if(scrollView != null)
                    {
                        scrollView.SetActive(true);
                    }
                    //Debug.Log("openMenu");
                }
                else
                {
                    if (scrollView != null)
                    {
                        scrollView.SetActive(false);
                    }
                    this.GetComponentInChildren<TextMesh>().text = "Show Selection";
                    //Debug.Log("closeMenu");
                }
                goInto = false;
                pressed = !pressed;
            }
        }
        // Debug.Log(Time.realtimeSinceStartup);

        private void Update()
        {
            GameObject cam = GameObject.Find("HoloLens");
            //transform.LookAt(cam.transform);
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        }


        void OnButtonReleased(GameObject go)
        {
            goInto = true;
          // Debug.Log("rel");
        }

    }
}