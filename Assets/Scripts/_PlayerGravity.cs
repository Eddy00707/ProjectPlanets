using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerGravity : NetworkBehaviour {

	public GameObject upperBody,lowerBody;
	public float upperBodyDistance;
//	public LineRenderer graviPath;
	public GameObject[] gravityPlanets, gravitySuns;
	public float GravityMultiplier=1f;
	Vector3 graviVector;

	const float HOVER_DISTANCE = 5;

	void Start () 
	{
		graviVector=Vector3.zero;
		ServerInterpolation.DoRenderWorld += CmdSetPlayerPosition;
		//upperBody = GetComponent<MouseLook> ().gameObject; //Instantiate (upperBody, this.transform.position+new Vector3(0,-upperBodyDistance,0),this.transform.rotation);
		gravityPlanets = GameObject.FindGameObjectsWithTag("Planet");
		gravitySuns = GameObject.FindGameObjectsWithTag("Sun");
		if((this.isLocalPlayer||this.isServer))
		{
			//Camera.SetupCurrent (upperBody.GetComponent<Camera>());


		}
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
				Vector3 gravityVector=(array[i].transform.position-lowerBody.transform.position);

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
				ConstantForce b = lowerBody.GetComponent<ConstantForce>();
				Vector3 sumOfGravityForce = Vector3.zero;
				CalculateGravity (gravityPlanets,ref sumOfGravityForce);
				CalculateGravity(gravitySuns,ref sumOfGravityForce);
				Vector3 mainVector = sumOfGravityForce;//new Vector3(Force1*X, Force1*Y, Force1*Z); 

				//Debug.Log(b.force);
				//b.force = new Vector3(0,1,0);
				Vector3 upDirection = -Vector3.Normalize(mainVector); //Vector3.ClampMagnitude(-mainVector,.01f);//-new Vector3(Force1*X, Force1*Y, Force1*Z);

				if(upDirection==Vector3.zero)
				{
					upDirection=Vector3.up;
				}
				Transform transformUpper = upperBody.GetComponent<Transform>();
				//Vector3 lookDirection=Vector3.Cross(upperBody.transform.right, upDirection);
				lowerBody.transform.up = upDirection;

				Vector3 hoverForce=Vector3.zero;
				RaycastHit hit;
				if(Physics.Raycast (lowerBody.transform.position, mainVector, out hit))
				{
					if(hit.collider.gameObject)
					{
						float distance =Vector3.Magnitude(hit.point-lowerBody.transform.position);
						//if(distance<HOVER_DISTANCE)
						{
							hoverForce=-mainVector*Mathf.Pow (HOVER_DISTANCE/distance,.1f);
						}
					}
				}
				//Debug.Log("Forces:"+hoverForce+","+mainVector);
				mainVector=hoverForce+mainVector;
				b.force=mainVector;



				//settting upper body position
				Vector3 destination = lowerBody.transform.position+upDirection*upperBodyDistance;
				Vector3 movement =  destination-upperBody.transform.position;
				upperBody.GetComponent<Rigidbody>().AddForce(movement);

				//graviPath.SetPosition(0,graviPath.gameObject.transform.position);
				//graviPath.SetPosition(1,upperBody.transform.position);

				RpcPlayerPosition(lowerBody.transform.rotation, upperBody.transform.position, lowerBody.transform.position);


				
			}
		}
	}

	[ClientRpc(channel=0)] // figure out, why after disabling this string it won`t work on client
	void RpcPlayerPosition(Quaternion rotationL, Vector3 positionU, Vector3 positionL)
	{
		if(!this.isServer)
		{
			StartCoroutine (InterpolateRotationL(rotationL)) ;
			StartCoroutine (InterpolatePositionU (positionU));
			StartCoroutine (InterpolatePositionL (positionL));
		}	
	}

	IEnumerator InterpolateRotationL( Quaternion destination)
	{
		int stepCount = 5;
		Vector3 movement = (destination.eulerAngles-lowerBody.transform.rotation.eulerAngles)/stepCount;
		for (int i = 0; i < stepCount; i++) 
		{
			Quaternion tmp = upperBody.transform.rotation;
			tmp.eulerAngles+=movement;
			yield return new WaitForSeconds(0.05f/ServerInterpolation.playerInterpolation);
		}
		yield return null;
	}


	IEnumerator InterpolatePositionU( Vector3 destination)
	{
		int stepCount = 5;
		Vector3 movement = (destination-upperBody.transform.position)/stepCount;
		for (int i = 0; i < stepCount; i++) 
		{
			upperBody.transform.position+=movement;
			yield return new WaitForSeconds(0.05f/ServerInterpolation.playerInterpolation);
		}
		yield return null;
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
	


}
