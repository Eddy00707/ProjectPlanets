using UnityEngine;
using System.Collections;

public class PlanetDestroyer : MonoBehaviour {

	public GameObject explosionPrefab;

	// Use this for initialization
	void Start () {
	
	}

	void OnDestroy()
	{
		//Instantiate(explosionPrefab,transform.position,transform.rotation);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
