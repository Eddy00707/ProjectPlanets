using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DestroyableObject : NetworkBehaviour 
{

	public const float PLAYER_HEALTH=100;
	public Material explosionMaterial;
	public float health;
	//public GameObject explosion1;
	public delegate void UIEvents();
	public static event UIEvents HPIsChanged;

	void Start()
	{
		if(this.gameObject.tag=="Planet")
		{
			health=this.GetComponent<Rigidbody>().mass;
		}
		else
		{
			health=PLAYER_HEALTH; 
		}
		//if(HPIsChanged!=null)HPIsChanged();
	}

	public void InflictDamage(float damage)
	{
		if(this.isServer)
		{
			health-=damage;
			//this.GetComponent<PlayerUI>().SetUIHp(health);
			//Debug.Log (health);
			if(health<=0) 
			{
				this.gameObject.GetComponent<MeshRenderer>().material = explosionMaterial;
				this.gameObject.GetComponent<ExplosionAnimation>().enabled=true;
				this.gameObject.GetComponent<SphereCollider>().enabled=false;
				//this.gameObject.AddComponent<AudioSource>().
			}
			RpcSendHPToClient (health);
			if(HPIsChanged!=null)	HPIsChanged();
		}
	}

	[ClientRpc(channel=0)]
	void RpcSendHPToClient(float hp)
	{
		if(this.isClient)
		{
			health=hp;
			if(HPIsChanged!=null)	HPIsChanged();
		}
	}



}
