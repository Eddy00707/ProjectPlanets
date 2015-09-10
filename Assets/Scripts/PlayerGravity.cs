using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerGravity : NetworkBehaviour {
	
	//public GameObject upperBody,lowerBody;
	//public float upperBodyDistance;
	//	public LineRenderer graviPath;
	public GameObject[] gravityPlanets, gravitySuns;
	public float GravityMultiplier;
	public Vector3 graviForce;
	const float HOVER_DISTANCE = 7;
	
	void Start () 
	{
		ServerInterpolation.DoRenderWorld += CmdSetPlayerPosition;
		//upperBody = GetComponent<MouseLook> ().gameObject; //Instantiate (upperBody, this.transform.position+new Vector3(0,-upperBodyDistance,0),this.transform.rotation);
		gravityPlanets = GameObject.FindGameObjectsWithTag("Planet");
		gravitySuns = GameObject.FindGameObjectsWithTag("Sun");
	}
	
	public void RecheckGravityObjects()
	{
		gravityPlanets = GameObject.FindGameObjectsWithTag("Planet");
		gravitySuns = GameObject.FindGameObjectsWithTag("Sun");
	}
	
	public void CalculateGravity(GameObject[] array,ref Vector3 sumOfGravityForce)
	{
		for(int i=0;i<array.Length;i++)
		{
			if(array[i]!=null)
			{
				float m2=array[i].GetComponent<Rigidbody>().mass;
				Vector3 gravityVector=(array[i].transform.position-this.transform.position);
				
				float Force1 =(GravityMultiplier*m2)/(Mathf.Pow(Vector3.Magnitude (gravityVector),2));
				Vector3 thisForce = Vector3.Normalize(gravityVector)*Force1;
				sumOfGravityForce+=thisForce;
			}
		}
	}
	
	
	[Command(channel=0)]
	void CmdSetPlayerPosition()
	{
		if(this)
		{
			if(this.isServer)
			{
				//graviPath.SetPosition(1, upperBody.transform.position);
				ConstantForce b = this.GetComponent<ConstantForce>();
				Vector3 sumOfGravityForce = Vector3.zero;
				CalculateGravity (gravityPlanets,ref sumOfGravityForce);
				CalculateGravity(gravitySuns,ref sumOfGravityForce);
				Vector3 mainVector = sumOfGravityForce;//new Vector3(Force1*X, Force1*Y, Force1*Z); 
				
				
				Vector3 hoverForce=Vector3.zero;
				RaycastHit hit;
				if(Physics.Raycast (this.transform.position, mainVector, out hit))
				{
					if(hit.collider.gameObject)
					{
						float distance =Vector3.Magnitude(hit.point-this.transform.position);
						if(distance<HOVER_DISTANCE*5)
						{
							if(distance<=HOVER_DISTANCE)
							{
								hoverForce=-mainVector*distance/(Mathf.Pow (distance,.7f));
							}
							else
							{
								hoverForce=-mainVector*HOVER_DISTANCE/(Mathf.Pow (distance,.7f));
							}
						}
					}
				}
				//Debug.Log("Force:"+mainVector);
				graviForce = mainVector;
				mainVector+=hoverForce;
				b.force=mainVector;
				RpcPlayerPosition( this.transform.position, this.transform.rotation);
				
				
				
			}
		}
	}
	
	[ClientRpc(channel=0)] // figure out, why after disabling this string it won`t work on client
	void RpcPlayerPosition(Vector3 position,Quaternion rotation)
	{
		if(!this.isServer)
		{
			StartCoroutine (InterpolatePosition(position));
			StartCoroutine (InterpolateRotation(rotation));
			
		}	
	}
	
	IEnumerator InterpolateRotation( Quaternion destination)
	{
		int stepCount = 5;
		Vector3 movement = (destination.eulerAngles-this.transform.rotation.eulerAngles)/stepCount;
		for (int i = 0; i < stepCount; i++) 
		{
			Quaternion tmp = this.transform.rotation;
			tmp.eulerAngles+=movement;
			yield return new WaitForSeconds(0.05f/ServerInterpolation.playerInterpolation);
		}
		yield return null;
	}
	
	
	IEnumerator InterpolatePosition( Vector3 destination)
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
	
	
	
	
	
}

