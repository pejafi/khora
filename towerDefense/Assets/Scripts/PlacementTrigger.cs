using UnityEngine;
using System.Collections;

public class PlacementTrigger : MonoBehaviour 
{
	public  bool		m_debug	= false;
	public  string[]	m_tags;

	private ArrayList m_objectInTrigger = new ArrayList();
	private Building  m_parentComponent = null;
	
	void Awake()
	{
		m_parentComponent = gameObject.transform.parent.gameObject.GetComponent<Building>();
	}
	
	void OnTriggerEnter (Collider a_obj) 
	{
		// same object
		if(gameObject.transform.parent.gameObject == a_obj.gameObject)
			return;
		
		for(int tIndex = 0; tIndex < m_tags.Length; tIndex++)
		{
			if(a_obj.tag == m_tags[tIndex])
			{
				if(m_debug) { print("ADD a_obj: " + a_obj); }
				m_objectInTrigger.Add(a_obj);
				if(m_debug) { print("ENTER count: " + m_objectInTrigger.Count); }
			}
		}
		
		if(m_objectInTrigger.Count > 0)
			m_parentComponent.SetCanPlace(false);
		else
			m_parentComponent.SetCanPlace(true);			
	}
	
	void OnTriggerExit (Collider a_obj)
	{
		// same object
		if(gameObject.transform.parent.gameObject == a_obj.gameObject)
			return;
		
		for(int tIndex = 0; tIndex < m_tags.Length; tIndex++)
		{
			if(a_obj.tag == m_tags[tIndex])
			{
				if(m_debug) { print("REMOVE a_obj: " + a_obj); }
				m_objectInTrigger.Remove(a_obj);
				if(m_debug) { print("EXIT count: " + m_objectInTrigger.Count); }
			}
		}
		
		if(m_objectInTrigger.Count > 0)
			m_parentComponent.SetCanPlace(false);
		else
			m_parentComponent.SetCanPlace(true);			
	}
}
