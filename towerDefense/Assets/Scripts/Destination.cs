using UnityEngine;
using System.Collections;

public class Destination : MonoBehaviour 
{
	private SpawnController	m_spawnController			= null;
	
	public 	bool m_pulseEffectEnabled					= true;

	void Awake()
	{
		m_spawnController = GameObject.FindWithTag("SpawnController").GetComponent<SpawnController>();
		
		if(m_spawnController)
			m_spawnController.RegisterDestination(this);
		
		if(m_pulseEffectEnabled)
		{
			iTween.PunchScale(gameObject, iTween.Hash( "name", "destination-pulse", 
														"amount", new Vector3(1.4f, 1.4f, 1.4f), 
														"time", 2.0f, 
														"looptype", "loop" ) );		
		}
	}
}
