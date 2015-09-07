using UnityEngine;
using System.Collections;

public class PlayerSpawnOnPlanet : MonoBehaviour 
{
	public GameObject playerPrefab;
	public GameObject [] planetsToSpawn;

	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Y))
		{
			planetsToSpawn = GameObject.FindGameObjectsWithTag ("Planet");
			int a = Random.Range(0,planetsToSpawn.Length-1);
			Instantiate(playerPrefab, planetsToSpawn[a].transform.position+new Vector3(0,planetsToSpawn[a].transform.lossyScale.y,0), planetsToSpawn[a].transform.rotation);
		}
	}
}
