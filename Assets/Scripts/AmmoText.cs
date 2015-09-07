using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmmoText : MonoBehaviour {
	
	Text ammoText;
	
	
	void Start () 
	{
		ammoText= this.gameObject.GetComponent<Text>();

		this.gameObject.GetComponentInParent<PlayerOwner>().owner.GetComponent<PlayerShooting>().AmmoIsChanged+=SetUIAmmo;
		//this.gameObject.GetComponentInParent<PlayerOwner>().owner.GetComponent<PlayerUI>().PlayerDestroyed+=HideHealth;
	}
	
	void Update()
	{
		//SetUIHp();
	}

	
	
	void SetUIAmmo()
	{
		if(this) 
		{
			if(ammoText==null) Debug.Log ("no text in UI");
			PlayerShooting PS=GetComponentInParent<PlayerOwner>().owner.GetComponent<PlayerShooting>();
			ammoText.text=PS.ammo[PS.selectedWeapon].ToString();
		}
	}
	
}
