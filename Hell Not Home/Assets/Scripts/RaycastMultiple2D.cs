using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// make sure the gameobject has a line renderer, we will be using this in our code
[RequireComponent(typeof(LineRenderer))]
public class RaycastMultiple2D : MonoBehaviour 
{
	// amount of damage the ray will do to a target
	public int damage = 1;

	// number of allowed hits for the raycast
	// if you have a hitcount of 2, the raycast will pass through the first one and stop at the next
	public int hitCount = 2;

	// max distance the ray will travel
	public float range = 10;

	// layer the targets will be on
	public LayerMask mask;

	// if the raycast hits something with this tag, the ray will not pass through it
	// this allows the ray to pass through damageable things and stop at walls
	public string wallTag;

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

	// stores an array of all the things our ray passed through
	RaycastHit2D[] hits;

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
		ray = new Ray2D(transform.position, transform.up);

		// set our end point for the ray, using ray.GetPoint and giving it the range
		// this will fire our ray to the maximum range
		// we will change this later in the code if we hit a target
		endPoint = ray.GetPoint(range);

		// create an array to store our hits
		// the hitcount is the total number of hits we can store
		hits = new RaycastHit2D[hitCount];

		// call physics2d.raycastnonalloc to fire a ray in the facing direction
		// this will store all of the things it hits within the range in the "hits" array we just created
		// note this method returns the total number of hits, so we check that number is bigger than zero
		if(Physics2D.RaycastNonAlloc(transform.position, transform.up, hits, range, mask) > 0) // are number of hits larger than zero?
		{
			// we loop through the hits array, using hitcount
			for(int i = 0; i < hitCount; i++)
			{
				// some of the hits may not have a transform, as the ray may not have hit enough targets
				if(hits[i].transform != null) // make sure we actually hit something!
				{
					// check the hit transform's tag, to see if we hit a wall
					if(hits[i].transform.CompareTag(wallTag)) // check for wal tag
					{
						// if we hit a wall, set the end point here, so the line renderer will stop here
						endPoint = hits[i].point;

						// exit from the for loop, we hit a wall, nothing more to see here
						break;
					}
					else // if we didn't hit a wall, we hit something we can damage!
					{
						// send the thing we hit some damage
						// note the last parameter "SendMessageOptions.DontRequireReceiver"
						// this will stop any errors if the thing we are messaging doesn't have a "TakeDamage" method with an integer
						hits[i].transform.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
					}

					// if this is the last thing in the array, set the endpoint for the line renderer here
					if(i == hitCount-1)
					{
						// set our endpoint fo rthe line renderer
						endPoint = hits[i].point;
					}
				}
			}
		}

		// set our line renderers start position to this gameobject
		// note we set position 0
		line.SetPosition(0, transform.position);

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
