using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RotateSun : NetworkBehaviour 
{
	//public float planetSpeed;
	public GameObject [] suns;
	//public float NeededRadius=6f;
	private float G=1f;
	public float RadiusMultiplier=1f;
	public float reverseDirection =1f;
	public float SpeedMultiplier=1f;
	public float direction1,direction2;//safafaf


	void Start()
	{
		suns = GameObject.FindGameObjectsWithTag ("Sun");
		ServerInterpolation.DoRenderWorld += CmdPlanetSpin;
	}

	//[Command(channel=0)]
	void CmdPlanetSpin()
	{
		if (this&&this.isServer) 
		{
			//Debug.Log (Time.time); //DoRenderWorld()
			Vector3 sumOfForces = new Vector3 (0, 0, 0);
			for (int i=0; i<suns.Length; i++) { // do multiple gravity
					if (suns [i]) {
							float m1 = GetComponent<Rigidbody> ().mass;
							float m2 = suns [i].GetComponent<Rigidbody> ().mass;
							//float r=transform.lossyScale.z;
		
		
							Vector3 p1 = suns [i].transform.position - transform.position;
		
							float R = Vector3.Distance (transform.position, suns [i].transform.position);
							float X = (p1.x / R);
							float Y = (p1.y / R);
							float Z = (p1.z / R);
							float GravityForce = SpeedMultiplier * (G * m1 * m2) / (R);
		
							float SpeedForce = (GravityForce * RadiusMultiplier);
		
							//Debug.Log(Force1+" "+Force2+" R"+R+" M"+RadiusMultiplier);
							float dir = Mathf.Sign (reverseDirection);
		
							float direction3 = 1 - direction1 - direction2;
		
							//				direction1 =1;
							//				direction2=0;
							//				direction3=0;
							//это работает
							//b.force=new Vector3(Force1*X+(Force2*X),Force1*Y+(Force2*Z),Force1*Z+Force2*-Y); //направление 1
		
							//b.force=new Vector3(Force1*X+(Force2*Y),Force1*Y+(Force2*-X),Force1*Z+Force2*-Z); //направление 2
							//b.force=new Vector3(Force1*X+(Force2*Z),Force1*Y+(Force2*Y),Force1*Z+Force2*-X); //направление 3
		
		
							sumOfForces += new Vector3 ( GravityForce * X + (direction1 * (SpeedForce * X) + 		direction2 * (SpeedForce * dir * Y) +   direction3 * (SpeedForce * dir * Z)),
								                         GravityForce * Y + (direction1 * (SpeedForce * dir * Z) +  direction2 * (SpeedForce * dir * -X) +  direction3 * (SpeedForce * Y)),
								                         GravityForce * Z + (direction1 * (SpeedForce * dir * -Y) + direction2 * (SpeedForce * Z) + 		direction3 * (SpeedForce * dir * -X))); 
							//So, based on directions 1,2,3, it calculate the trajectory of planet
					}
			}
			ConstantForce b = GetComponent<ConstantForce> ();
			b.force = sumOfForces;
			//RpcSendPlanetForce(sumOfForces);
			RpcClientSendPlanetCoordinates (this.transform.position, this.transform.rotation);
		}
	}
	

	// Update is called once per frame
//	void FixedUpdate () 
//	{
//		//if(this.isLocalPlayer)
//		CmdPlanetSpin();
//	}
	
	[ClientRpc(channel=0)]
	void RpcClientSendPlanetCoordinates(Vector3 serverPosition, Quaternion serverRotation)
	{
		if(this.isClient)
		{
			StartCoroutine(InterpolatePosition (serverPosition));
			//this.transform.position=serverPosition;
			//this.transform.rotation=serverRotation;
		}
	}

	IEnumerator InterpolatePosition(Vector3 destination)
	{
		int stepCount = 5;
		Vector3 movement = (destination-this.transform.position)/stepCount;
		for (int i = 0; i < stepCount; i++) 
		{
			this.transform.position+=movement;
			yield return new WaitForSeconds(0.05f/ServerInterpolation.playerInterpolation);
		}
		yield return null;
	}


	[ClientRpc(channel=0)]
	void RpcSendPlanetForce(Vector3 newForce)
	{
		if(this.isClient)
		{
			ConstantForce a= this.GetComponent<ConstantForce>();
			a.force=newForce;
		}
	}
	
	
}
