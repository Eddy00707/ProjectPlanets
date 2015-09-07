using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ServerInterpolation : NetworkBehaviour {
	
	//private float currentTickTime=0.0f;
	private float tickTime=0.05f;

	public const float playerInterpolation = 10;
	private float interpolationTime=0.1f;

	public delegate void TickAction();
	public static event TickAction DoRenderWorld;


	//public float syncSpeed=5;
	
	// Use this for initialization
	void Start () 
	{
		//if(this.isServer)
		//{
			StartCoroutine(Tick());
		//}
		
	}

	IEnumerator Tick()
	{
		for(;;)
		{
			if(DoRenderWorld!=null)
			{
				DoRenderWorld();

			}
			yield return new WaitForSeconds (tickTime);
		}
	}

}