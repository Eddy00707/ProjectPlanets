using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CreatePlanet : NetworkBehaviour {

	public GameObject planetPrefab;
	public GameObject sunPrefab;
	private List<Material> materialsPlanet, materialsSun;

	void Start () 
	{
		materialsPlanet=new List<Material>();
		Object [] a=(Resources.LoadAll("Planets"));
		foreach (Object o in a)
		{
			materialsPlanet.Add(o as Material);
		}

		materialsSun=new List<Material>();
		Object [] b=(Resources.LoadAll("Stars"));
		foreach (Object o in b)
		{
			materialsSun.Add(o as Material);
		}


	}
	
	// Update is called once per frame
	void Update () 
	{
		if(this.isLocalPlayer)
		{
			if(Input.GetKeyDown(KeyCode.R))
			{
				float rndRngP = 50;
				Vector3 planetPosition = new Vector3(Random.Range(-rndRngP,rndRngP),Random.Range(-rndRngP,rndRngP),Random.Range(-rndRngP,rndRngP));
				float direction = Random.value-0.5f;
				//if(direction==0f)direction++;
				float a1=Random.value;
				float a2 =Random.Range(0,a1);
				GameObject[] suns = GameObject.FindGameObjectsWithTag("Sun");
				//GameObject sunToSpinRound = suns[Random.Range(0, suns.Length)]; Deprecated
				Material myMaterial = materialsPlanet[(int)Random.Range(0,materialsPlanet.Count)];
				float planetSize = Random.Range(5,15);

				CmdCreatePlanet(planetPosition,direction,a1,a2,suns,myMaterial,planetSize);
			}
			if(Input.GetKeyDown(KeyCode.T))
			{
				createSun();
			}
		}
	}

	[Command(channel = 0)]
	void CmdCreatePlanet(Vector3 planetPosition, float direction, float a1, float a2, GameObject[] suns, Material myMaterial, float planetSize)
	{
		if(this.isServer)
		{
		
			GameObject newPlanet = (GameObject)Instantiate (planetPrefab, planetPosition, new Quaternion(0,0,0,1)); //change before release
			//SetPlanetParameters(newPlanet,  suns,planetSize*20f, new Vector3(planetSize,planetSize,planetSize),direction,a1,a2,Random.Range(3,7),Random.Range(.2f,.5f),myMaterial);

			RpcSendPlanetToPlayer  (newPlanet.GetComponent<NetworkView>().viewID);
		
		}
	}

	//[ClientRpc(channel=0)]
	void RpcSendPlanetToPlayer(NetworkViewID id)
	{
		if(this.isClient)
		{
			NetworkView a= NetworkView.Find(id);
			GameObject newPlanet = a.gameObject;
			Instantiate (newPlanet, newPlanet.transform.position,newPlanet.transform.rotation);
//			GameObject player = GameObject.FindGameObjectWithTag("Player");
//			if(player)player.GetComponent<PlayerGravity>().RecheckGravityObjects();
		}
	}

	void createSun()
	{
		float rndRngS = 50;
		float sunSize = Random.Range(25, 70);
		Material mySunMaterial = materialsSun[(int)Random.Range(0,materialsSun.Count)];
		SetSunParameters(sunPrefab, new Vector3(sunSize,sunSize,sunSize), mySunMaterial);
		//Component halo = sunPrefab.GetComponent("Halo"); halo.GetType().GetProperty("Size").SetValue(halo, 100, null);
		Instantiate(sunPrefab,new Vector3 (Random.Range(-rndRngS,rndRngS),Random.Range(-rndRngS,rndRngS),Random.Range(-rndRngS,rndRngS)), new Quaternion(0,0,0,1));
		GameObject [] allPlanets = GameObject.FindGameObjectsWithTag("Planet");
		foreach(GameObject a in allPlanets)
		{
			a.GetComponent<RotateSun>().suns = GameObject.FindGameObjectsWithTag("Sun");
		}
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if(player)player.GetComponent<PlayerGravity>().RecheckGravityObjects();
	}

	void SetSunParameters(GameObject sun,Vector3 size, Material mat)
	{
		sun.transform.localScale = size;
		sun.GetComponent<Rigidbody>().mass = size.x*30f;
		Renderer r = sun.GetComponent<Renderer>();
		r.material= mat;
		sun.GetComponent<Light>().color = mat.color;
		sun.name = mat.name;


	}

	void SetPlanetParameters(GameObject planet, GameObject[] suns,float mass, Vector3 scale,float direction, float dir1, float dir2, float radMult, float spMult, Material mat)
	{
		planet.GetComponent<RotateSun>().suns = suns;
		planet.tag = "Planet";
		planet.GetComponent<Rigidbody>().mass = mass;
		planet.transform.localScale = scale;
		planet.GetComponent<RotateSun>().reverseDirection = direction;
		Renderer r = planet.GetComponent<Renderer>();
		r.material= mat;
		RotateSun RS = planet.GetComponent<RotateSun>();
		RS.direction1 = dir1;
		RS.direction2 = dir2;
		RS.RadiusMultiplier=radMult;
		RS.SpeedMultiplier = spMult;
		planet.name = mat.name;
	}

}
