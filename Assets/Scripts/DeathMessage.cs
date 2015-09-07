using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class DeathMessage : NetworkBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		this.gameObject.SetActive(false);
		this.gameObject.GetComponentInParent<PlayerOwner>().owner.GetComponent<PlayerUI>().PlayerDestroyed+=YoureDeadText;
	}
	

	private void YoureDeadText()
	{
		this.gameObject.SetActive(true);
	}

}
