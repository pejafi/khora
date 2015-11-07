using UnityEngine;
using System.Collections;

public class ObjNavigation : AIPath 
{
	public Transform m_target = null;
	
	void Start () 
	{
		if(m_target)
			SetTarget(m_target);
		
		base.Start();
	}
	
	public void SetTarget(Transform a_target, bool a_canMove = true, bool a_canSearch = true)
	{
		m_target = a_target;
		target = a_target;
		canMove = a_canMove;
		canSearch = a_canSearch;
	}	
	
	public float DistanceFromDestination()
	{
		if(m_target)
		{
			Vector3 distance = m_target.transform.position - transform.position;
			return distance.magnitude;
		}	
		
		return 0.0f;
	}
}
