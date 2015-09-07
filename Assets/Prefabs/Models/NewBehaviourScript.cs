using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	public float loopduration, lifetime;
	private float currentClip,currentlifetime;

	void Start()
	{
		currentlifetime=0;
		currentClip=1;
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
		Renderer a= this.GetComponent<Renderer>();
		a.material.SetVector("_ChannelFactor", new Vector4(r,g,b,0));
		currentlifetime+=Time.deltaTime;
		{
			if(currentlifetime>lifetime)
			{
				a.material.SetFloat("_ClipRange",currentClip);
				currentClip-=Time.deltaTime;
				if(currentClip<=0)
				{
					Destroy (this.gameObject);
				}
			}
		}

	}
}
