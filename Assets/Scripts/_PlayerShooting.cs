
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public class _PlayerShooting : NetworkBehaviour 
{
	bool globalShooting=false;
	
	//public ParticleSystem muzzleFlash;
	//Animator anim;
	public GameObject rayCaster;
	public int selectedWeapon;
	AudioSource weaponAudio;
	//public GameObject lowerBody;
	public GameObject [] gunzModels;


	//Minigun
	public GameObject [] minigunBarrels;
	float minigunHeat ; 
	bool Mheat, Mshoot, Mcool;
	public Animator minigunAnimations;
	private const int MINIGUN_PLACE=2;
	public const float MINIGUN_DAMAGE = 20;
	public GameObject bulletPrefab;
	const float MINIGUN_RELOAD_TIME=0.1f, MINIGUN_ALT_RELOAD_TIME = 0.3f; 
	public AudioClip minigunShootingSound;
	public AudioSource minigunAudio;
	//public GameObject destroyTEMP;

	//Railgun
	public const int RAILGUN_PLACE=1;
	public const float RAILGUN_DAMAGE = 35;

	public AudioClip railgunShootingSound;
	public GameObject railgunTrail;
	public float railgunShootingTime;
	public float railgunReloadTime;

	//GraviGun
	private const int GRAVIGUN_PLACE = 3;
	private const int GRAVITRAIL_VERTEX_COUNT=50;
	public LineRenderer gravigunTrail;
	private float graviForce;
	public GameObject gravigunParticles;
	private bool gravityCoroutineStarted=false, gravigunAtFullPower=false;
	public AudioClip gravigunShootingSound, gravigunMaxEnergySound;
	public AudioSource gravigunAudio;
	//global
	 bool [] shooting;
	 bool [] reloading;
	public int [] ammo;

	//events
	public delegate void ChangingAmmo();
	public  event ChangingAmmo AmmoIsChanged ;


	public const int defaultWeapon=3;
	
	// Use this for initialization
	void Start () { //FIXME when other player connects, first player always with minigun (maybe ask from server, which weapon is on)
		minigunHeat=0;
		graviForce=0;
		Mheat=false;
		Mcool=false;
		Mshoot=false;
//		minigunIsCooling=false;

		shooting = new bool[10];
		reloading =new bool[10];
		ammo =new int[10];
		for(int i=0;i<shooting.Length;i++)
		{
			shooting[i]=false;
			reloading[i]=false;
			ammo[i]=-1;

		}
		ammo[RAILGUN_PLACE]=13;
		ammo[GRAVIGUN_PLACE]=1;
		ammo[MINIGUN_PLACE]=500;
		//ammo[RAILGUN_PLACE]=5;
		if(AmmoIsChanged!=null) AmmoIsChanged();
		selectedWeapon=defaultWeapon;
		gunzModels[selectedWeapon].SetActive (true);

		weaponAudio = this.GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(this.isLocalPlayer)
		{

			if(Input.GetKeyDown(KeyCode.Alpha1)&&ammo[RAILGUN_PLACE]>=0&&selectedWeapon!=RAILGUN_PLACE)
			{
				StopAllGunz ();
				ChangeWeapon (RAILGUN_PLACE);
				CmdChangeWeapon(RAILGUN_PLACE);
			}
			if(Input.GetKeyDown(KeyCode.Alpha2)&&ammo[MINIGUN_PLACE]>=0&&selectedWeapon!=MINIGUN_PLACE)
			{
				StopAllGunz ();
				ChangeWeapon(MINIGUN_PLACE);
				CmdChangeWeapon(MINIGUN_PLACE);
			}
			if(Input.GetKeyDown(KeyCode.Alpha3)&&ammo[GRAVIGUN_PLACE]>=0&&selectedWeapon!=GRAVIGUN_PLACE)
			{
				StopAllGunz ();
				ChangeWeapon (GRAVIGUN_PLACE);
				CmdChangeWeapon(GRAVIGUN_PLACE);
			}

			if(Input.GetButton ("Fire1"))  //pressed left mouse button
			{

				if(selectedWeapon==RAILGUN_PLACE&&!shooting[RAILGUN_PLACE]&&!reloading[RAILGUN_PLACE]&&ammo[RAILGUN_PLACE]>0)
				{
					ammo[RAILGUN_PLACE]--;
					if(AmmoIsChanged!=null) AmmoIsChanged();
					StartCoroutine(RailGunShoot());
					CmdRailgunShoot();
				}
				else if(selectedWeapon == MINIGUN_PLACE&&!shooting[MINIGUN_PLACE]&&!reloading[MINIGUN_PLACE]&&ammo[MINIGUN_PLACE]>0)
				{
					if(!Mheat) //Starting minigun spin 
					{
						CmdMinigunStart();
						StartMinigun ();
					}
					if(minigunHeat>=1&&!Mshoot) //shooting minigun
					{

						CmdMinigunShoot();
						ShootMinigun (false);
					}

				}

				else if(selectedWeapon == GRAVIGUN_PLACE&&!shooting[GRAVIGUN_PLACE]&&!reloading[GRAVIGUN_PLACE]) //FIXME do quick gravigun trail interpolation
				{
					if(AmmoIsChanged!=null) AmmoIsChanged();
					PrepareGravigun();
					CmdGravigunShoot();
				}
				//else globalShooting=false;
			}

			else if(Input.GetButton ("Fire2"))  //pressed left mouse button
			{
				
				if(selectedWeapon==RAILGUN_PLACE&&!shooting[RAILGUN_PLACE]&&ammo[RAILGUN_PLACE]>0)
				{
					GameObject a = this.GetComponent<PlayerController>().camera;
					a.GetComponent<Camera>().fieldOfView = 15;
				}
				else if(selectedWeapon == MINIGUN_PLACE&&!shooting[MINIGUN_PLACE]&&!reloading[MINIGUN_PLACE]&&ammo[MINIGUN_PLACE]>=6)
				{
					StartCoroutine (MinigunAltShoot());
				}
			}

			else //when i release the shootingbutton
			{

				StopAllGunz();
			}
			//Debug.Log(minigunHeat);
		}
	}


	/// <summary>
	/// Put into this function all that should be done when we change the weapon or release the shootingbutton
	/// </summary>
	void StopAllGunz()
	{
		GameObject a = this.GetComponent<PlayerController>().camera;
		a.GetComponent<Camera>().fieldOfView = 70;
		StopGravigun();
		CmdGravigunStop();
		CmdMinigunStop();
		StopMinigun ();
	}


	//ALL weapons section
	public void AddAmmo()
	{
		if(AmmoIsChanged!=null) AmmoIsChanged();
	}

	/// <summary>
	/// Shoots a stright ray from camera forward	
	/// </summary>
	/// <returns>Point in Global coordinates, that ought to be hit by weapon.</returns>
	Vector3 GetShootingPoint()
	{
		RaycastHit hit;
		if(Physics.Raycast(rayCaster.transform.position,rayCaster.transform.forward, out hit))
		{
			return hit.point;
		}
		else return rayCaster.transform.forward*100000;
	}

	void ChangeWeapon(int newWeaponPlace)
	{
		StopMinigun();
		if(AmmoIsChanged!=null) AmmoIsChanged();
		gunzModels[selectedWeapon].SetActive (false);
		selectedWeapon=newWeaponPlace;
		gunzModels[selectedWeapon].SetActive (true);
		Debug.Log("Selected weapon "+newWeaponPlace);
	}

	[Command(channel=0)]
	void CmdChangeWeapon(int newWeaponPlace)
	{
		if(this.isServer)
		{
			ChangeWeapon(newWeaponPlace);
			RpcChangeWeapon(newWeaponPlace);
		}
	}

	[ClientRpc(channel=0)]
	void RpcChangeWeapon(int newWeaponPlace)
	{
		if(!this.isLocalPlayer&&this.isClient)
		{
			ChangeWeapon(newWeaponPlace);
		}
	}


	//RAILGUN section

	[Command(channel=0)]
	void CmdRailgunShoot()
	{
		if(this.isServer)
		{
			RaycastHit hit;
			if(Physics.Raycast(railgunTrail.transform.position, GetShootingPoint ()-railgunTrail.transform.position, out hit))
			{
				
				//Debug.Log(hit.transform.gameObject.name);
				DestroyableObject hitObject = hit.transform.gameObject.GetComponentInParent<DestroyableObject>();
				if(hitObject&&hitObject!=this.gameObject)
				{
					hitObject.InflictDamage(RAILGUN_DAMAGE);
				}
			}
			StartCoroutine(RailGunShoot());
			RpcRailgunShoot();
		}
	}
	
	[ClientRpc(channel=0)]
	void RpcRailgunShoot()
	{
		if(!this.isLocalPlayer&&this.isClient)
		{
			StartCoroutine(RailGunShoot());
		}
	}


	IEnumerator RailGunShoot()
	{
		shooting[RAILGUN_PLACE] = true;
		//railgunTrail.GetComponent<LineRenderer>().SetPosition(0,railgunTrail.transform.position*railgunTrail.transform.localToWorldMatrix);
		railgunTrail.GetComponent<LineRenderer>().SetPosition(1,railgunTrail.transform.InverseTransformPoint(GetShootingPoint()));
		Debug.Log(GetShootingPoint()+" "+(railgunTrail.transform.InverseTransformPoint(GetShootingPoint())));
		railgunTrail.SetActive (true);
		weaponAudio.clip=railgunShootingSound;
		weaponAudio.Play();
		yield return new WaitForSeconds(railgunShootingTime);
		shooting[RAILGUN_PLACE]=false;
		reloading[RAILGUN_PLACE]=true;
		yield return new WaitForSeconds(railgunShootingTime);
		//RailGunTrail.enabled = false;
		railgunTrail.SetActive (false);
		yield return new WaitForSeconds(railgunReloadTime-(railgunShootingTime)); //why?
		reloading[RAILGUN_PLACE]=false;
		
	}



	//MINIGUN section
	/// <summary>
	/// Starts the minigun
	/// </summary>
	private void StartMinigun()
	{
		Mheat=true;
		Mcool=false;
		Mshoot=false;
		minigunAnimations.SetBool("Heat",true);
		Debug.Log("should start");
		minigunAnimations.SetBool("Cool",false);
		minigunAnimations.SetBool("Shoot",false);
		StartCoroutine(HeatMinigun());
	}

	/// <summary>
	/// Shoots the minigun.
	/// </summary>
	private void ShootMinigun(bool serverIsCalling)
	{
		Mshoot=true;
		Mheat=false;
		Mcool=false;
		minigunAnimations.SetBool("Shoot",true);
		minigunAnimations.SetBool("Cool",false);
		minigunAnimations.SetBool("Heat",false);
		StartCoroutine(MinigunShoot(serverIsCalling));
	}
	/// <summary>
	/// Stops the minigun.
	/// </summary>
	private void StopMinigun()
	{
		Mheat=false;
		if(!Mcool&&minigunHeat>0)
		{
			Mcool=true;
			minigunAnimations.SetBool("Heat",false);
			minigunAnimations.SetBool("Shoot",false);	
			minigunAnimations.SetBool("Cool",true);
			StartCoroutine(CoolMinigun());
		}
		if(minigunHeat<=0) 
		{
			minigunHeat=0;
			Mcool=false;
			minigunAnimations.SetBool("Cool",false);
			StopCoroutine (CoolMinigun());
			StopCoroutine (HeatMinigun());
			StopCoroutine ("MinigunShoot");
		}
	}



	IEnumerator HeatMinigun()
	{
		for(;;)
		{
			if(minigunHeat<1&&Mheat)
			{
				minigunHeat+=0.1f;
				yield return new WaitForSeconds (0.15f);
				
			}
			else break;
		}
	}

	IEnumerator MinigunShoot(bool serverCallsYou)
	{
		for(;;)
		{
			if(!(minigunHeat>=1&&Mshoot)||ammo[MINIGUN_PLACE]<=0)
			{
				break;
			}
			shooting[MINIGUN_PLACE] = true;
			weaponAudio.clip=minigunShootingSound;
			weaponAudio.Play();
			Instantiate (bulletPrefab, minigunBarrels[0].transform.position,Quaternion.LookRotation((GetShootingPoint())- minigunBarrels[0].transform.position ));
			if(serverCallsYou)
			{
				ammo[MINIGUN_PLACE]--;
				if(AmmoIsChanged!=null) AmmoIsChanged();
			}
			shooting[MINIGUN_PLACE]=false;
			reloading[MINIGUN_PLACE]=true;
			yield return new WaitForSeconds(MINIGUN_RELOAD_TIME);
			reloading[MINIGUN_PLACE]=false;
			
		}
		
	}

	IEnumerator CoolMinigun()
	{
		for(;;)
		{
			if(minigunHeat>0&&Mcool)	
			{
				if(minigunHeat<=0.1f)
				{
					minigunHeat=0;
					break;
				}
				minigunHeat-=0.1f;
				yield return new WaitForSeconds (0.2f);

			}
			else break;

		}
	}

	//Alt shoot

	IEnumerator MinigunAltShoot()
	{
		shooting[MINIGUN_PLACE] = true;
		weaponAudio.clip=minigunShootingSound;
		weaponAudio.Play();
		for(int i=0;i<minigunBarrels.Length-1;i++)
		{
			Instantiate (bulletPrefab,minigunBarrels[i].transform.position,Quaternion.LookRotation(GetShootingPoint()-minigunBarrels[i].transform.position));
		}
		ammo[MINIGUN_PLACE]-=6;
		if(AmmoIsChanged!=null) AmmoIsChanged();
		shooting[MINIGUN_PLACE]=false;
		reloading[MINIGUN_PLACE]=true;
		yield return new WaitForSeconds(MINIGUN_ALT_RELOAD_TIME);
		reloading[MINIGUN_PLACE]=false;
	}


	//Server MINIGUN section

	[Command(channel=0)]
	void CmdMinigunStart()
	{
		if(this.isServer)
		{
			StartMinigun ();
			RpcMinigunStart();
		}
	}


	[Command(channel=0)]
	void CmdMinigunShoot()
	{
		if(this.isServer)
		{
			ShootMinigun(true);
			RpcMinigunShoot();
		}
	}


	[Command(channel=0)]
	void CmdMinigunStop()
	{
		if(this.isServer)
		{
			StopMinigun();
			RpcMinigunStop();
		}
	}


	[ClientRpc(channel=0)]
	void RpcMinigunStart()
	{
		if(!this.isLocalPlayer&&this.isClient)
		{
			StartMinigun ();
		}
	}

	
	[ClientRpc(channel=0)]
	void RpcMinigunShoot()
	{
		if(!this.isLocalPlayer&&this.isClient)
		{
			ShootMinigun (false);
		}
	}
	



	[ClientRpc(channel=0)]
	void RpcMinigunStop()
	{
		if(!this.isLocalPlayer&&this.isClient)
		{
			StopMinigun ();
		}
	}



	//GRAVIGUN section


	[Command(channel=0)]
	void CmdGravigunShoot()
	{
		if(this.isServer)
		{
			PrepareGravigun ();
			RpcGravigunShoot();
		}
	}

	[Command(channel=0)]
	void CmdGravigunStop()
	{
		if(this.isServer)
		{
			StopGravigun ();
			RpcGravigunStop();
		}
	}
	
	
	[ClientRpc(channel=0)]
	void RpcGravigunShoot()
	{
		if(!this.isLocalPlayer&&this.isClient)
		{
			PrepareGravigun();
		}
	}


	[ClientRpc(channel=0)]
	void RpcGravigunStop()
	{
		if(!this.isLocalPlayer&&this.isClient)
		{
			StopGravigun ();
		}
	}


	void StopGravigun()
	{
		StopCoroutine ("GravirayCoroutine");
		gravigunAudio.Stop ();
		gravigunAudio.loop=false;
		gravigunAtFullPower=false;

		gravityCoroutineStarted=false;
		gravigunParticles.SetActive (false);
		graviForce=0;
		gravityCoroutineStarted=false;
		gravigunTrail.enabled=false;
	}

	void PrepareGravigun()
	{
		gravigunTrail.SetWidth(graviForce*2,graviForce*2);
		gravigunTrail.enabled=true;
		gravigunTrail.SetPosition(0,gravigunTrail.gameObject.transform.position);
		RaycastHit hit;
		if(Physics.Raycast(gravigunTrail.transform.position, GetShootingPoint()-gravigunTrail.transform.position, out hit))
		{
			GameObject hitObject = hit.transform.gameObject;
			
			if(hitObject)
			{
				
				//gravigunTrail.SetPosition(1,hit.point);

				gravigunParticles.SetActive (true);
				if(!gravityCoroutineStarted)
				{
					gravityCoroutineStarted=true;
					StartCoroutine (GravirayCoroutine(hitObject));
					if(this.isServer)StartCoroutine(ShootGravigun(hitObject));
					StartCoroutine(IncreaseGravigunStrength());
				}
				//hitObject.InflictDamage(RAILGUN_DAMAGE);
			}
			
		}
		
		else
		{
			//gravigunAudio.Stop ();
			//graviForce=0;
			//gravigunTrail.SetWidth(graviForce*2,graviForce*2);
			//gravityCoroutineStarted=false;
			//gravigunTrail.SetPosition(1,gravigunTrail.gameObject.transform.position + gravigunTrail.gameObject.transform.forward*1000);
		}
	}

	IEnumerator IncreaseGravigunStrength()
	{
		while(true)
		{
			if(!gravityCoroutineStarted) break;
			if(!gravigunAtFullPower&&graviForce>=0.25)
			{
				gravigunAtFullPower=true;
				gravigunAudio.clip=gravigunMaxEnergySound;
				gravigunAudio.loop=true;
				gravigunAudio.Play ();
			}
			if(graviForce<.5)graviForce+=0.05f;
			
			yield return new WaitForSeconds(1);
		}
	}

	IEnumerator GravirayCoroutine(GameObject destinationObj)
	{
		while(true)
		{
			if(!gravityCoroutineStarted) break;
//			Debug.Log (destinationObj.name);
			SetGravityRay(destinationObj.transform.position);
			yield return new WaitForSeconds(0.01f);
		}
	}

	private void SetGravityRay(Vector3 destination)
	{
		gravigunTrail.SetVertexCount(GRAVITRAIL_VERTEX_COUNT);
		for(int i=0;i<GRAVITRAIL_VERTEX_COUNT;i++)
		{
			float j=(float)i/GRAVITRAIL_VERTEX_COUNT;
			gravigunTrail.SetPosition((int)(i),Vector3.Lerp(Vector3.Lerp(gravigunTrail.gameObject.transform.position,
			                                                             gravigunTrail.gameObject.transform.position + gravigunTrail.gameObject.transform.forward*4f,j),
			                                                Vector3.Lerp(gravigunTrail.gameObject.transform.position + gravigunTrail.gameObject.transform.forward*4f,
			             									destination,j),j));
		}


	}
	
	private Vector3 AddGravityObject(GameObject obj)
	{
		
		Vector3 gravityVector=(obj.transform.position-this.transform.position);
		Vector3 thisForce = Vector3.Normalize(gravityVector)*graviForce*gravityVector.magnitude*3;
		return thisForce;
	}


	IEnumerator ShootGravigun(GameObject obj)
	{
		//Vector3 gravityForce=Vector3.zero;
		graviForce=0;
		gravigunAudio.clip=gravigunShootingSound;
		gravigunAudio.Play();
		while(true)
		{
			if(!gravityCoroutineStarted) break;
			Rigidbody rb = this.GetComponent<Rigidbody>();
			rb.AddForce(AddGravityObject(obj));
//			Debug.Log(graviForce);
			yield return new WaitForSeconds(0.05f);
			
		}
	}


}