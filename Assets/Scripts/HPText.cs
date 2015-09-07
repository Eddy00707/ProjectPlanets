using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPText : MonoBehaviour {

	// Use this for initialization


	void Start () 
	{
		Text mytext= this.gameObject.GetComponent<Text>();
		float defaultHealth=DestroyableObject.PLAYER_HEALTH;
		mytext.text=defaultHealth.ToString();
		DestroyableObject.HPIsChanged+=SetUIHp;
		this.gameObject.GetComponentInParent<PlayerOwner>().owner.GetComponent<PlayerUI>().PlayerDestroyed+=HideHealth;
	}

	void Update()
	{
		//SetUIHp();
	}


	void HideHealth()
	{
		this.gameObject.GetComponent<Text>().text="";
	}


	void SetUIHp()
	{
		if(this) 
		{
			Text mytext= this.gameObject.GetComponent<Text>();
			if(mytext==null) Debug.Log ("no text in UI");
			float health=GetComponentInParent<PlayerOwner>().owner.GetComponent<DestroyableObject>().health;
			mytext.text=health.ToString();
		}
	}

}
