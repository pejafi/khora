using UnityEngine;
using System.Collections;

public class FindBuilding : MonoBehaviour 
{
	public  bool		m_debug	= false;

	private Enemy  		m_parentComponent = null;
	
	void Awake()
	{
		m_parentComponent = gameObject.transform.parent.gameObject.GetComponent<Enemy>();
	}
	
	void OnTriggerEnter (Collider a_obj) 
	{
		if(a_obj.tag == "Building")
		{
						if(m_debug) { print("Set Target to kill as: " + a_obj); }
			
			m_parentComponent.TargetEnteredTrigger(a_obj.gameObject.GetComponent<Building>());
		}
	}
	
	void OnTriggerExit (Collider a_obj)
	{		
		if(a_obj.tag == "Building")
		{
						if(m_debug) { print("Set Target to leave alone as: " + a_obj); }
			
			m_parentComponent.TargetExitedTrigger(a_obj.gameObject.GetComponent<Building>());
		}
	}
}
