using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class sendPositionRotationToClient : NetworkBehaviour {

	private float currentResponceTime=0.0f;
	private float ServerResponceTime=0.05f;
	private float interpolationTime=0.1f;
	public float syncSpeed = 5;

	public GameObject upperBody,lowerBody;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(this.isLocalPlayer)
		{
			CmdAskForPosition();
		}
	}

	[Command(channel=0)]
	void CmdAskForPosition()
	{
		if(this.isServer)
		{
		//	if(currentResponceTime>ServerResponceTime)
			{
				currentResponceTime=0;

				
				RpcSendPosition(upperBody.transform.position,lowerBody.transform.position);
				RpcSendRotation(upperBody.transform.rotation,lowerBody.transform.rotation);
				
			}
		//	else currentResponceTime+=Time.deltaTime;
		}
	}
	
	[ClientRpc(channel=0)]
	public void RpcSendPosition(Vector3 positionU, Vector3 positionL)
	{
		if(this.isClient)
		{
			StartCoroutine(SyncPosition(upperBody,positionU));
			StartCoroutine(SyncPosition(lowerBody,positionL));
		}
	}

	[ClientRpc(channel=0)]
	public void RpcSendRotation(Quaternion rotationU, Quaternion rotationL)
	{
		if(this.isClient)
		{
			StartCoroutine(SyncRotation(upperBody,rotationU));
			StartCoroutine(SyncRotation(lowerBody,rotationL));
		}
	}

	IEnumerator SyncPosition(GameObject obj, Vector3 destination)
	{
		float step = 20 / syncSpeed;
		Vector3 oneStep = (destination - obj.transform.position) / step;
		for (int i = 0; i < step; i++) 
		{
			obj.transform.position+=oneStep;
			yield return new WaitForSeconds(ServerResponceTime/step);
		}
	}
	
	IEnumerator SyncRotation(GameObject obj, Quaternion destination)
	{
		float step = 20 / syncSpeed;
		Vector3 oneStep = (destination.eulerAngles - obj.transform.rotation.eulerAngles) / step;
		for (int i = 0; i < step; i++) 
		{
			Quaternion myRotation =obj.transform.rotation; 
			myRotation.eulerAngles+=oneStep;
			yield return new WaitForSeconds(ServerResponceTime/step);
		}
	}
}


