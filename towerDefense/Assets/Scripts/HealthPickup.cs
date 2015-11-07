using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour 
{
	public  float		m_hitPointInc	=	25.0f;

	public float GetPickup()
	{
		return m_hitPointInc;
	}
	
	public void Kill()
	{
		Destroy(gameObject);
	}
}
