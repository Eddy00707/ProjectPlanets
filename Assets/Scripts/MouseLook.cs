using UnityEngine; 
using UnityEngine.Networking;
using System.Collections;

public class MouseLook : NetworkBehaviour
{

	/// MouseLook rotates the transform based on the mouse delta. /// Minimum and Maximum values can be used to constrain the possible rotation

	/// To make an FPS style character: /// - Create a capsule. /// - Add a rigid body to the capsule /// - Add the MouseLook script to the capsule. /// -> Set the mouse look to use LookX. (You want to only turn character but not tilt it) /// - Add FPSWalker script to the capsule

	/// - Create a camera. Make the camera a child of the capsule. Reset it's transform. /// - Add a MouseLook script to the camera. /// -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.) [AddComponentMenu("Camera-Control/Mouse Look")] public class MouseLook : MonoBehaviour {


	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;
	//public GameObject upperBody;
	float rotationX = 0F;
	float rotationY = 0F;



	void Start ()
	{
		ServerInterpolation.DoRenderWorld+=SendPlayerPosition;
		// Make the rigid body not change rotation
		if (this.GetComponent<Rigidbody>())
			this.GetComponent<Rigidbody>().freezeRotation = true;

	}


	void Update ()
	{
		if(this)
		{
			if(this.isLocalPlayer) 
			{
				Quaternion originalRotation = this.transform.localRotation;
				// Read the mouse input axis
				rotationX = Input.GetAxis("Mouse X") * sensitivityX;
				rotationY = Input.GetAxis("Mouse Y") * sensitivityY;

				Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
				Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);




				this.transform.localRotation = (originalRotation * xQuaternion * yQuaternion);

			}
		}


	}

	void SendPlayerPosition()
	{
		if(this)
			CmdLocalRotation (this.transform.localRotation);
	}

	[Command(channel=0)]
	void CmdLocalRotation(Quaternion newLocalRotation)
	{
		if(this.isServer)
		{
			this.transform.localRotation=newLocalRotation;
			RpcLocalRotation(this.transform.localRotation);
		}
	}
//
//
	[ClientRpc(channel=0)]
	void RpcLocalRotation(Quaternion newLocalRotation)
	{
		if(this.isClient&&!this.isLocalPlayer)
		{
			this.transform.localRotation=newLocalRotation;
			
		}
	}




}