using UnityEngine;
using System.Collections;

public class PlanetForce : MonoBehaviour {

	public float attractor = 1;

	private GameObject[] planets;

	void Start () 
	{
		planets = GameObject.FindGameObjectsWithTag("Planet");
//		for(int i=0; i < planets.Length; i++ )
//		{
//			Debug.Log(planets[i].name.ToString());
//		}
	
	}
	
	void Update () 
	{
		GetComponent<Rigidbody>().velocity  = new Vector3(0,0,0);
		foreach ( GameObject planet in planets)
		{
		Vector3 planetPosition = planet.transform.position;
		Vector3 playerPosition = transform.position;

		float distance = Mathf.Sqrt(Mathf.Sqrt( Mathf.Pow(planetPosition.x - playerPosition.x, 2)+ Mathf.Pow(planetPosition.y - playerPosition.y, 2)) + Mathf.Pow(planetPosition.z - playerPosition.z, 2));
		//Debug.Log(distance);
		//Debug.Log(rigidbody.sleepVelocity);
		//rigidbody.AddForce(( planetPosition - playerPosition) * attractor * Mathf.Sqrt(planet.rigidbody.mass/10) / Mathf.Pow( distance, 2) );
			GetComponent<Rigidbody>().AddForce((( planetPosition - playerPosition) * attractor * planet.GetComponent<Rigidbody>().mass) / (Mathf.Pow( distance, 3)));
			transform.up = -(planetPosition - playerPosition);
//			float camerasZPosition = GetComponentInChildren<Camera>().transform.position.z;
		}
	}
}
