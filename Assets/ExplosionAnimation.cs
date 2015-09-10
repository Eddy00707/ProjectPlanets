using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody)) ]
public class ExplosionAnimation : MonoBehaviour {
	public AudioClip explosionSound;
	public float loopduration=3f;
	bool expanding;

	// Use this for initialization
	void Start () 
	{
		expanding=true;
		this.gameObject.AddComponent<AudioSource>();
		if(this.GetComponent<TrailRenderer>())
		this.GetComponent<TrailRenderer>().enabled=false;
		AudioSource aS = GetComponent<AudioSource>();
		aS.clip=explosionSound;
		aS.Play ();
		StartCoroutine(ProcessExplosion());
	}

	IEnumerator ProcessExplosion()
	{
		if(transform.childCount>0)
		{
			for(int i=transform.childCount-1;i>=0;i--)
			{
				Destroy (transform.GetChild(i).gameObject);
			}
		}
		while(true)
		{
			Renderer r = this.GetComponent<Renderer>();
			float displacement =  r.material.GetFloat("_Displacement");
			if(displacement>=0.7f) expanding=false;
			else if(displacement<0) 
			{

				Destroy(this.gameObject);
			}
			r.material.SetFloat("_Displacement",expanding? displacement+0.02f:displacement-0.02f);
			if(!expanding)
			{
				float alpha = r.material.GetFloat("_ClipRange");
				r.material.SetFloat("_ClipRange", alpha-0.04f);
			}
			yield return new WaitForSeconds(.02f);
		}

	}

	// Update is called once per frame
	void Update () 
	{
		float r = Mathf.Sin((Time.time / loopduration) * (2 * Mathf.PI)) * 0.5f + 0.25f;
		float g = Mathf.Sin((Time.time / loopduration + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
		float b = Mathf.Sin((Time.time / loopduration + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
		float correction = 1 / (r + g + b);
		r *= correction;
		g *= correction;
		b *= correction;
		this.GetComponent<Renderer>().material.SetVector("_ChannelFactor", new Vector4(r,g,b,0));
	}
}
