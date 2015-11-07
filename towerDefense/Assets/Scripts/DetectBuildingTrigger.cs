using UnityEngine;
using System.Collections;

public class DetectBuildingTrigger : MonoBehaviour 
{
	public  bool		m_debug	= false;

	private Enemy  		m_parentComponent = null;
	
	private ArrayList	m_buildingsInRange = new ArrayList();
	
	private SpawnController	m_spawnController			= null;
	
	void Awake()
	{
		m_spawnController = GameObject.FindWithTag("SpawnController").GetComponent<SpawnController>();
		m_parentComponent = gameObject.transform.parent.gameObject.GetComponent<Enemy>();
	}
	
	void OnTriggerEnter (Collider a_obj) 
	{
		if(a_obj.tag == "BuildingDetection")
		{
			a_obj.gameObject.transform.parent.gameObject.GetComponent<Building>().AddToTargetsInRange(m_parentComponent);
			m_buildingsInRange.Add(a_obj.gameObject);
						
						if(m_debug) { print("BuildingDetection - add to building: " + m_parentComponent); }
		}
		
		if(a_obj.tag == "Destination" && !m_parentComponent.IsDead())
		{
			if(m_spawnController)
				m_spawnController.SetGameOver(true);
		}
	}
	
	void OnTriggerExit (Collider a_obj)
	{		
		if(a_obj.tag == "BuildingDetection")
		{
			a_obj.gameObject.transform.parent.gameObject.GetComponent<Building>().RemoveFromTargetsInRange(m_parentComponent);
			m_buildingsInRange.Remove(a_obj.gameObject);
						
						if(m_debug) { print("BuildingDetection - remove from building: " + m_parentComponent); }
		}
	}
	
	public void AlertAllBuildingsInRangeWeHaveDied()
	{
		for(int bIndex = 0; bIndex < m_buildingsInRange.Count; bIndex++)
		{
			GameObject building = m_buildingsInRange[bIndex] as GameObject;
			
			if(building)
				building.transform.parent.GetComponent<Building>().RemoveFromTargetsInRange(m_parentComponent);
			
						if(m_debug) { print("BuildingDetection - remove from building: " + m_parentComponent); }
		}
	}
}
