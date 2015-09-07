using UnityEngine;
using System.Collections;

public class RailGunStand : MonoBehaviour 
{

	public const int AMMO = 13, RELOADTIME = 5;
	bool isAvaliable=true;

	void OnTriggerEnter(Collider other)
	{
		if(isAvaliable)
		{
			PlayerShooting PS = other.GetComponentInParent<PlayerShooting>();
			if(PS)
			{
				PS.ammo[PlayerShooting.RAILGUN_PLACE]+=AMMO;
				PS.AddAmmo();
			}
			StartCoroutine(ExpendWeapon());
		}
	}

	IEnumerator ExpendWeapon()
	{
		isAvaliable=false;
		this.gameObject.GetComponent<MeshRenderer>().enabled=false;
		yield return new WaitForSeconds(RELOADTIME);
		isAvaliable=true;
		this.gameObject.GetComponent<MeshRenderer>().enabled=true;
		yield return null;

	}

}
