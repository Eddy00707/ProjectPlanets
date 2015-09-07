using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float velocity = 10;
	public float range = 100;
	private Vector3 startPoint;


	// Use this for initialization
	void Start () 
	{
		startPoint=transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position+=(velocity*Time.deltaTime*transform.forward);
		if(Vector3.Magnitude(transform.position-startPoint)>=range)
		{
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter(Collider c)
	{
		Destroy(gameObject);
//		Debug.Log("Hit "+c.gameObject.name);
		DestroyableObject hitObject = c.gameObject.GetComponentInParent<DestroyableObject>();
		if(hitObject)hitObject.InflictDamage(PlayerShooting.MINIGUN_DAMAGE);
	}

}
