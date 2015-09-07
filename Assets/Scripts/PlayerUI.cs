using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerUI : NetworkBehaviour {

	public GameObject canvas;

	public delegate void PlayerDestroyDelegate();
	public event PlayerDestroyDelegate PlayerDestroyed;

	private GameObject myUI;

	// Use this for initialization
	void Start () 
	{
		if(this.isLocalPlayer)
		{
			myUI=(GameObject) Instantiate (canvas);
			myUI.GetComponent<PlayerOwner>().owner=this.gameObject;
		}
	}

	void OnDestroy()
	{
		//Destroy(myUI);
		if(PlayerDestroyed!=null) PlayerDestroyed();
	}


	// Update is called once per frame
	void Update () 
	{
	
	}
	

}
