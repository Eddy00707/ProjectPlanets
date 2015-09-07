using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour 
{
	public float playerSpeed,crouchSpeed, jumpSpeed;
	public bool LockCursor;
	public GameObject lowerBody, upperBody, camera;
	//public float jumpDuration;
	bool jumping = false;//, crouching=false;
	public Texture2D cursorTexture;
	CursorMode cursorMode = CursorMode.ForceSoftware;
	//Vector2 hotSpot = new Vector2(30, 20);
	void Start()
	{
		ServerInterpolation.DoRenderWorld+=LocalPlayerMovement;
		if(this.isLocalPlayer)
		{
			this.GetComponentInChildren<AudioListener>().enabled=true;
			Camera myCamera= camera.GetComponent<Camera>();
			myCamera.enabled=true;
			Camera.SetupCurrent (myCamera);
			camera.tag="MainCamera";
		}
		//Cursor.SetCursor(cursorTexture,new Vector2(-140,10), cursorMode);
		if(LockCursor)Cursor.lockState=CursorLockMode.Locked;
		Cursor.visible = true;
	}

	void LocalPlayerMovement() 
	{
		if(this)
		{
			if(this.isLocalPlayer)
			{
				if(Input.GetKey(KeyCode.Escape))
				{
					Application.Quit();
					//UnityEditor.EditorApplication.isPlaying = false;
				}

				float fvBv=Input.GetAxis("Vertical");
				float LfRt=Input.GetAxis("Horizontal");
				bool jump = (Input.GetAxis("Jump")>0)?true:false;
				bool crouch = (Input.GetAxis ("Crouch")>0)?true:false;
				CmdServerMovePlayer(fvBv,LfRt,jump, crouch);
			}
		}
	}


	[Command(channel=0)]
	private void CmdServerMovePlayer(float FwBk, float LtRt, bool jump, bool crouch)
	{
		if(this.isServer)
		{
			Vector3 newForce = Vector3.zero;
			float angle = Vector3.Angle(upperBody.transform.up,lowerBody.transform.up);
			//Debug.Log(angle);
			ConstantForce cf = lowerBody.GetComponent<ConstantForce>();
			Vector3 lookDirection=Vector3.Normalize(Vector3.ProjectOnPlane(upperBody.transform.forward,lowerBody.transform.up));
			if(FwBk!=0)
			{
				
				newForce+= ((angle>90)?-1:1)	*((FwBk>0)?1:-1)*lookDirection*playerSpeed; //instead of forward
			}
			if(LtRt!=0)
			{
				newForce+= ((LtRt>0)?1:-1) * Vector3.Normalize(Vector3.ProjectOnPlane (upperBody.transform.right,lowerBody.transform.up))*playerSpeed;
			}
			cf.force+=Vector3.ClampMagnitude(newForce,playerSpeed);
			if(jump)
			{
				if(!jumping)
				{
					jumping=true;
					cf.force+=(lowerBody.transform.up*jumpSpeed);

					StartCoroutine(MakeJump()); 
				}
			}
			if(crouch)
			{
				cf.force+=(-lowerBody.transform.up*crouchSpeed);
			}
			RpcPlayerPosition(lowerBody.transform.rotation, lowerBody.transform.position);
			
		}                                             
	}        




	[ClientRpc(channel=0)] // figure out, why after disabling this string it won`t work on client
	void RpcPlayerPosition(Quaternion rotationL, Vector3 positionL)
	{
		if(!this.isServer)
		{
			lowerBody.transform.rotation=rotationL;
			StartCoroutine (InterpolatePositionL (positionL));
		}	
	}



	IEnumerator InterpolatePositionL(Vector3 destination)
	{
		int stepCount = 5;
		Vector3 movement = (destination-lowerBody.transform.position)/stepCount;
		for (int i = 0; i < stepCount; i++) 
		{
			lowerBody.transform.position+=movement;
			yield return new WaitForSeconds(0.05f/ServerInterpolation.playerInterpolation);
		}
		yield return null;
	}

	private IEnumerator MakeJump()
	{
	yield return new WaitForSeconds(2);
		jumping=false;
	}





}
