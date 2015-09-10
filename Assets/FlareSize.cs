using UnityEngine;
using System.Collections;

public class FlareSize : MonoBehaviour 
{

	public Camera main;
	float size;
	
	int value=5;
	
	void Start()
	{
		size = this.transform.lossyScale.x;

		GetComponent<Light>().color = GetComponent<MeshRenderer>().material.color;
		GetComponent<LensFlare>().color = GetComponent<Light>().color;
	}

	void Update () 
	{
		
		if(Camera.main)
		{
			float distance = Vector3.Magnitude(gameObject.transform.position - Camera.main.transform.position);
			gameObject.GetComponent<LensFlare>().brightness = size*value/distance;
		}
		
		
	} 
}
