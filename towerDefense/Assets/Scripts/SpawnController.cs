using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour 
{
	public  WaveSequence[] m_waveSequences;
	public  int m_startWave = 0;
	
	public	int m_maxEnemiesInExistance		= 100;
	private int	m_numOfEnemiesInExistance	= 0;
	
	private ArrayList	m_spawners = new ArrayList();
	private ArrayList	m_spawnedBuildings = new ArrayList();
	private ArrayList	m_destinations = new ArrayList();
	
	private bool 		m_spawnActive	=	true;
	private int 		m_waveSequenceIndex = 0;
	
	private bool		m_gameOver = false;
	public  bool IsGameOver() { return m_gameOver; }
	public  void SetGameOver(bool a_value) { m_gameOver = a_value; }
	
	private bool		m_hasWon = false;
	public  bool HasWon() { return m_hasWon; }
	public  void SetWon(bool a_value) { m_hasWon = a_value; }
	
	public  int       m_playerCash			= 100;
		
	void Awake()
	{
		SpawnNewWave();
	}
	
	public void SpawnNewWave()
	{
		if(m_gameOver)
			return;
		
		print ("SpawnNewWave");
		
		if(m_waveSequenceIndex >= m_waveSequences.Length)
		{
			m_hasWon = true;
			m_gameOver = true;
			return;
		}
		
		m_waveSequences[m_waveSequenceIndex].SetSpawnActive(true);
		m_waveSequenceIndex++;
		m_startWave++;
	}
	
	public void RegisterSpawner (ObjSpawner a_spawner) 
	{
		m_spawners.Add(a_spawner);
	}
	
	public void RegisterSpawnedBuilding (Building a_building) 
	{
		m_spawnedBuildings.Add(a_building);
	}
	
	public void RegisterDestination (Destination a_destination) 
	{
		m_destinations.Add(a_destination);
	}
	
	public Destination GetRandomDestination()
	{
		int randNum = Random.Range(0, m_destinations.Count);
		Destination destinations = m_destinations[randNum] as Destination;
		return destinations;
	}
	
	public void RemoveSpawnedBuilding (Building a_building) 
	{
		m_spawnedBuildings.Remove(a_building);
	}
	
	public void InformAllBuildingsEnemyDied(Enemy a_deadEnemy)
	{
		for(int bIndex = 0; bIndex < m_spawnedBuildings.Count; bIndex++)
		{
			Building building = m_spawnedBuildings[bIndex] as Building;
			building.EnemyDied(a_deadEnemy);
		}
		
		EarnCash(a_deadEnemy.m_cashForKill);
	}
	
	public void IncEnemySpawn()
	{
		m_numOfEnemiesInExistance++;
	}
	
	public void DecEnemySpawn()
	{
		m_numOfEnemiesInExistance--;
	}
	
	public bool CanSpawn()
	{
		if(m_numOfEnemiesInExistance < m_maxEnemiesInExistance)
			return true;
		
		return false;
	}
	
	public bool CanPurchase(int a_amount)
	{
		if(m_playerCash >= a_amount)
		{
			return true;
		}
		
		return false;
	}
		
	public void MakePurchase(int a_amount)
	{
		if(m_playerCash >= a_amount)
		{
			m_playerCash -= a_amount;
		}
	}
	
	public void EarnCash(int a_amount)
	{
		m_playerCash += a_amount;
	}
	
}
