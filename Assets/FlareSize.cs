using UnityEngine;
using System.Collections;

public class FlareSize : MonoBehaviour 
{

	public Camera main;
	
	
	int value=300;
	
	void Start()
	{

		GetComponent<Light>().color = GetComponent<MeshRenderer>().material.color;
		GetComponent<LensFlare>().color = GetComponent<Light>().color;
	}

	void Update () 
	{
		
		
		float distance = Vector3.Magnitude(gameObject.transform.position - Camera.main.transform.position);
		gameObject.GetComponent<LensFlare>().brightness = value/distance;
		
		
	} 
}
