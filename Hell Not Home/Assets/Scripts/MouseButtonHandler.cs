using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



// this component will send a UnityEvent when a mouse button is pressed, held down or released (or all 3)
// we can assign a mouse button to the "button" property in the inspector
public class MouseButtonHandler : MonoBehaviour 
{
    // we create an enum (enumerable) so we can have a nice dropdown menu in the inspector
    // this will allow us to select which mouse button we want to add events to
    public enum MouseButtonIndex { Left, Right, Middle }

    // this is the selected mouse button we will use from the enum above
    public MouseButtonIndex button;

    // use these events in the inspector to tell other gameobjects to do something when the events are invoked
    // event for when the mouse button is pressed - only activates once each time the the mouse button is pressed
    public UnityEvent onButtonDown;

    // activates constantly when mouse button is held down
    public UnityEvent onButton;

    // activates when the mouse button is released - only activates once each time the mouse button is released
    public UnityEvent onButtonUp;

    void Update ()
    {
        // check if our mouse button has been pressed
        // note we are using casting to turn the button enum value into an integer
        if (Input.GetMouseButtonDown((int)button))
        {
            // send the event if the mouse button was pressed
            onButtonDown.Invoke();
        }

        // check if our mouse button is held down
        if(Input.GetMouseButton((int)button))
        {
            // send the event every frame the mouse button is held down
			// NOTE: this will send A LOT of events! (like, around 60 per second!)
            onButton.Invoke();
        }

        // check if our mouse button has been released
        if (Input.GetMouseButtonUp((int)button))
        {
            // send the event if the key was released
            onButtonUp.Invoke();
        }
	}
}


