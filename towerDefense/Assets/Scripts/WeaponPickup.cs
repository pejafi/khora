using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour 
{
	public	GameObject	m_weaponToPickup	=	null;
	
	public GameObject GetPickup()
	{
		return m_weaponToPickup;
	}
	
	public void Kill()
	{
		Destroy(gameObject);
	}
}
