using UnityEngine;
using System.Collections;

public class BlastRadius : MonoBehaviour 
{
	private ArrayList	m_objectsToKill = new ArrayList();
	
	public void Blast(float a_hitForce, float m_hitDamage)
	{
		for(int kIndex = 0; kIndex < m_objectsToKill.Count; kIndex++)
		{
			GameObject kObject = m_objectsToKill[kIndex] as GameObject;
			if(kObject)
			{
				if(kObject.GetComponent<Enemy>())
					kObject.GetComponent<Enemy>().Hit(a_hitForce, (kObject.transform.position - transform.position), m_hitDamage);
				if(kObject.GetComponent<Building>())
					kObject.GetComponent<Building>().Hit((kObject.transform.position - transform.position), m_hitDamage);
			}
		}
	}
	
	void OnTriggerEnter(Collider a_obj)
	{
		if(a_obj.tag == transform.parent.GetComponent<Bullet>().GetOwner())
			return;	
				
		if(a_obj.tag == "Enemy")
		{
			m_objectsToKill.Add(a_obj.gameObject);
		}		
		else if(a_obj.tag == "Building")
		{
			m_objectsToKill.Add(a_obj.gameObject);
		}		
	}
	
	void OnTriggerExit(Collider a_obj)
	{
		if(a_obj.tag == "Enemy")
		{
			m_objectsToKill.Remove(a_obj.gameObject);
		}
		else if(a_obj.tag == "Building")
		{
			m_objectsToKill.Remove(a_obj.gameObject);
		}		
	}
	
	public void Clear()
	{
		m_objectsToKill.Clear();
	}
}
