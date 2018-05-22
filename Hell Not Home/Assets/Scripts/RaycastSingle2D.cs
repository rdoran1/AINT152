using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// make sure the gameobject has a line renderer, we will be using this in our code
[RequireComponent(typeof(LineRenderer))]

public class RaycastSingle2D : MonoBehaviour 
{
	// amount of damage the ray will do to a target
	public int damage = 1;

	// max distance the ray will travel
	public float range = 10;

	// layer the targets will be on
	public LayerMask mask;

	// time between firing the ray
	public float fireTime = 0.25f;

	// time the line renderer will be visible
	public float lineVisibleTime = 0.1f;

	// set this to true to debug this component
	public bool debugMode = false;

	// store the transform of this gameobject for more efficient code
	Transform mTransform;

	// the line renderer we will use for our ray
	LineRenderer line;

	// will allow us to fire if not firing already
	bool isFiring = false;

	// we will use this to calculate the end point of our ray
	Ray2D ray;

	// stores the thing we hit with our ray
	RaycastHit2D hit;

	// stores the end point of our ray
	// if it doesnt hit anything, it will be at the max range
	// if it hits a target, it will be at the point it hit the target (hit.point)
	Vector3 endPoint = Vector3.zero;


	void Start () 
	{
		// store the transform for this gameobject, makes code more efficient
		mTransform = transform;		

		// get our line renderer component for use later
		line = GetComponent<LineRenderer>();

		// reset the isfiring to false, ready to fire again
		ResetFire();

		// reset the line renderer
		ResetLine();
	}
	
	
	void Update () 
	{
		// DEBUG CODE
		if(debugMode)
		{
			if (Input.GetMouseButtonDown(0))
			{
				Fire();
			}

			// rotates this gameobject to face the mouse
			Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle + 270, Vector3.forward);

			// draws a debug line from this gameobject to the maximum range
			Ray r = new Ray(transform.position, transform.up);
			Debug.DrawLine(transform.position, r.GetPoint(range), Color.red);
		}
	}

	public void Fire()
	{
		// if we are already firing, exit and do nothing
		if(isFiring) return;

		// set isfiring to true, so no other fire commands can be given
		isFiring = true;
		
		// create a ray for the direction we are currently facing
		// in 2D we use the transform.up to get the players facing direction
		ray = new Ray2D(mTransform.position, mTransform.up);

		// set our end point for the ray, using ray.GetPoint and giving it the range
		// this will fire our ray to the maximum range
		// we will change this later in the code if we hit a target
		endPoint = ray.GetPoint(range);

		// call physics2d.raycast to fire a ray in the facing direction
		// this will store the first thing it hits within the range
		// the mask will filter any hits and only store hits with the matching layer
		hit = Physics2D.Raycast(mTransform.position, mTransform.up, range, mask);

		// if the raycast hit something on a matching layer, we check here
		// if the hit has a transform (the thing it hit) then we actually hit something!
		if(hit.transform != null) // did we hit anything?
		{
			// send the thing we hit some damage
			// note the last parameter "SendMessageOptions.DontRequireReceiver"
			// this will stop any errors if the thing we are messaging doesn't have a "TakeDamage" method with an integer
			hit.transform.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

			// set a new end point, the exact position the raycast hit the transform
			endPoint = hit.point;
		}

		// set our line renderers start position to this gameobject
		// note we set position 0
		line.SetPosition(0, mTransform.position);

		// set the end position of the line renderer
		// note we set position 1
		// the endpoint will either be our maximum range if we didn't hit anything or the exact point we hit something
		line.SetPosition(1, endPoint);

		// switch the line renderer component on, so we can see the line
		line.enabled = true;

		// set a timer using "fireTime" for the isfiring, so we can fire again
		Invoke("ResetFire", fireTime);

		// set a timer using "lineVisibleTime" for the line renderer to switch it off 
		// this timer should be set to a shorter time than the firetime!
		Invoke("ResetLine", lineVisibleTime);
	}

	// method called to reset isfiring
	void ResetFire()
	{
		isFiring = false;
	}

	// method to reset the line renderer
	void ResetLine()
	{
		// switch the line renderer off, making it invisible
		line.enabled = false;

		// set the start and end poisitions of the line renderer to the position of this gameobject
		// this stops any slight flashes of the line renderer when its being switched off or changing position (probably wont happen!)
		line.SetPosition(0, mTransform.position);
		line.SetPosition(1, mTransform.position);
	}
}
