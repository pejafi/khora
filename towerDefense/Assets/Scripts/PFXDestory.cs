using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class PFXDestory : MonoBehaviour
{
	public bool DontDestory; // don't destory on PFX finish 
	
	void OnEnable()
	{
		StartCoroutine("CheckAlive");
	}
	
	IEnumerator CheckAlive ()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			if(!this.GetComponent<ParticleSystem>().IsAlive(true))
			{
				if(DontDestory)
				{
					#if UNITY_3_5
						this.gameObject.SetActiveRecursively(false);
					#else
						this.gameObject.SetActive(false);
					#endif
				}
				else
					GameObject.Destroy(this.gameObject);
				break;
			}
		}
	}
}
