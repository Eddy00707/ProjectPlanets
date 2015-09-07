using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ArmorDrive : NetworkBehaviour 
{
	float veloMyMax =10, veloMyCurr =0;

	float veloSrvMax = 10, veloSrvCurr = 0;

	float periodSrvRpc = 0.02f;
	float timeSrvRpcLast = 0;

	void Awake()
	{
		this.transform.position = new Vector3(0,1.1f,0);
	}

	// Update is called once per frame
	void Update () 
	{
		if(this.isLocalPlayer)
		{
			float veloMyNew = 0;
			veloMyNew += Input.GetKey (KeyCode.W) ? veloMyMax : 0;
			veloMyNew += Input.GetKey (KeyCode.S) ? -veloMyMax : 0;
			if(veloMyCurr!=veloMyNew)
			{
				CmdDrive(veloMyCurr=veloMyNew);
			}
		}
	}

	[Command(channel=0)]
	void CmdDrive(float veloSvrNew)
	{
		if(this.isServer)
		{
			veloSvrNew = Mathf.Clamp (veloSvrNew,-veloSrvMax, veloSrvMax);
			veloSrvCurr = veloSvrNew;
		}
	}

	void FixedUpdate()
	{
		if(this.isServer)
		{
			this.transform.Translate(0,0,veloSrvCurr*Time.deltaTime, Space.Self);
			if(timeSrvRpcLast+periodSrvRpc<Time.time)
			{
				RpcUpdateUnitPosition(this.transform.position);
				RpcUpdateUnitOrientation(this.transform.rotation);
				timeSrvRpcLast = Time.time;
			}
		}
	}

	[ClientRpc(channel=0)]
	void RpcUpdateUnitPosition(Vector3 position)
	{
		this.transform.position = position;
	}

	[ClientRpc(channel=0)]
	void RpcUpdateUnitOrientation(Quaternion rotation)
	{
		this.transform.rotation = rotation;
	}


}
