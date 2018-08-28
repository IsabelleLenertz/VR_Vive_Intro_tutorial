using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {
    // Store references to the controller
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private GameObject collidingObject;
    private GameObject objectInHand;

    void Awake()
    {
        // Get a handle on the controllers
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void setCollidingObject(Collider col)
    {
        // If the player is already holding smt or the colliding object does not have a rigid body
        if(collidingObject || !col.GetComponent<Rigidbody>()){
            return;
        }
        collidingObject = col.gameObject;
    }

	// Update is called once per frame
	void Update () {
		if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }
        else if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
	}

    public void OnTriggerEnter(Collider other)
    {
        setCollidingObject(other);
    }

    // Ensure that the target is set when the player holds a controller over an object for a while.
    public void onTriggerStay(Collider other)
    {
        setCollidingObject(other);
    }

    public void onTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }
        collidingObject = null;
    }

    private void GrabObject()
    {
        objectInHand = collidingObject;
        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        objectInHand = null;
    }
}
