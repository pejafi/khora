using UnityEngine;
using System.Collections;

public class ObjSpawner : MonoBehaviour 
{
	public	float		m_timerBetweenSpawns		= 1.0f;
	private	float		m_timerBetweenSpawnsStore	= 1.0f;
	public  int			m_numToSpawnAtOneTime		= 1;
	public	GameObject	m_objectToSpawn				= null;
	public	bool		m_spawnActive				= true;
	public  bool		m_spawnOnStart				= true;
	public  int 	    m_capSpawnAmount			= 0;
	private int			m_spawnCount				= 0;
	private GameObject	m_target					= null;
	
	private SpawnController	m_spawnController		= null;
	
	void Awake()
	{
		m_spawnController = GameObject.FindWithTag("SpawnController").GetComponent<SpawnController>();
		
		if(m_spawnController)
			m_spawnController.RegisterSpawner(this);
	}
	
	void Start () 
	{
		//m_target = m_spawnController.GetRandomDestination().gameObject;
		
		if(m_spawnOnStart)
			m_timerBetweenSpawnsStore = m_timerBetweenSpawns;
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
	
	void Spawn()
	{
		if(m_spawnController)
			if(!m_spawnController.CanSpawn())
				return;
		
		for(int sIndex = 0; sIndex < m_numToSpawnAtOneTime; sIndex++)
		{
			if(m_capSpawnAmount != 0)
				if(m_spawnCount >= m_capSpawnAmount)
					break;
			
			GameObject obj = Instantiate(m_objectToSpawn, transform.position, transform.rotation) as GameObject;
			if(obj.GetComponent<Enemy>())
				obj.GetComponent<Enemy>().SetTarget(m_spawnController.GetRandomDestination().gameObject);
			
			if(m_spawnController)
				m_spawnController.IncEnemySpawn();
			
			m_spawnCount++;
		}	
	}
}
