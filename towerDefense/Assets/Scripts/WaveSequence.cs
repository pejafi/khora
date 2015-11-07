using UnityEngine;
using System.Collections;

public class WaveSequence : MonoBehaviour
{
	public Enemy[] m_enemiesToSpawn;
	public float   m_speedIncPercentage = 0.0f;
	public float   m_damageIncPercentage = 0.0f;
	public float   m_hitPointsIncPercentage = 0.0f;
	
	public	float		m_timerBetweenSpawns		= 1.0f;
	private	float		m_timerBetweenSpawnsStore	= 1.0f;
	
	private int     m_spawnCount = 0;
	private int		m_deadCount = 0;
	
	private bool	m_spawnActive = false;
	public  bool	IsSpawnActive()	{ return m_spawnActive; }
	
	private SpawnController	m_spawnController			= null;
	
	void Awake()
	{
		m_spawnController = GameObject.FindWithTag("SpawnController").GetComponent<SpawnController>();
	}
	
	void Update () 
	{
		if(m_spawnActive)
		{
			m_timerBetweenSpawnsStore += Time.deltaTime;
			if(m_timerBetweenSpawnsStore > m_timerBetweenSpawns)
			{
				Spawn();				
				m_timerBetweenSpawnsStore = 0.0f;
			}
		}
	}
	
	public void SetSpawnActive(bool a_value) 
	{ 
		m_spawnActive = a_value; 
	}
	
	public void RegisterHasDied()
	{
		if(!m_spawnActive)
			return;
		
		m_deadCount++;
			
		if(m_deadCount >= m_enemiesToSpawn.Length)
		{
			m_spawnActive = false;
			
			if(m_spawnController)
				m_spawnController.SpawnNewWave();
		}
	}
	
	void Spawn()
	{
		if(m_spawnCount >= m_enemiesToSpawn.Length)
		{
			//m_spawnActive = false;
			return;
		}
		
		Enemy obj = Instantiate(m_enemiesToSpawn[m_spawnCount], transform.position, transform.rotation) as Enemy;
	
		if(obj)
		{
			obj.IncreaseAttributes(m_speedIncPercentage, m_damageIncPercentage, m_hitPointsIncPercentage);
			
			obj.SetTarget(m_spawnController.GetRandomDestination().gameObject);
			obj.SetWaveSequenceOwner(this);
		}
				
		m_spawnCount++;
	}
}
