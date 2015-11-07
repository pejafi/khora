using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
	public  bool		m_debug	= false;
	
	private bool		m_isDead						= false;
	public  bool		IsDead()						{ return m_isDead; }
	
	private	Camera   	m_camera    					= null;
	public  float		m_hitPoints						= 1.0f;
	private float		m_hitPointsStore				= 1.0f;
	public	float		m_meleeHitDamage				= 1.0f;
	public  float		m_hitDistance					= 2.0f;
	public  float		m_timeBetweenHits				= 1.0f;
	public  float		m_timeFromInPositionToHitContact = 0.5f;
	private float		m_timeFromInPositionToHitContactStore = 0.0f;
	public	GameObject	m_pfxHit						= null;
	public	GameObject	m_pfxDeath						= null;
	public	GameObject	m_pfxHitGround					= null;
	public  float		m_pfxGroundPos					= 0.5f;
	public  bool        m_pulseOnHit					= true;
	private float		m_hitTime						= 0.0f;
	private bool		m_hit							= false;
	private	float		m_canDestroyRigidbodyTimerMax	= 2.0f;
	private float		m_rigidbodyTimer				= 0.0f;
	private float		m_hitForce						= 0.0f;
	private Vector3		m_hitForceDir					= new Vector3(0.0f, 0.0f, 0.0f);
	private GameObject	m_target						= null;
	
	public  float		m_removeFromSceneTime			= 20.0f;
	
	private SpawnController	m_spawnController			= null;
	private bool		m_haveToldSpawnControllerWeHaveDied = false;

	private WaveSequence	m_waveOwner			= null;
	
	public  int 		m_cashForKill					= 5;
	
	// ranged
	public bool	 m_isRanged 							= false;
	/*
	public float attackRange 							= 30.0f;
	public float shootAngleDistance 					= 10.0f;
	public float m_rotationSpeed 						= 2.0f;
	*/
	public WeaponSystem m_weapon 						= null;
	
	public DetectBuildingTrigger 	m_detectBuildingTrigger			= null;
	public FindBuilding 			m_findBuildingTrigger			= null;
	
	public void SetTarget(GameObject a_target)
	{
		m_target = a_target;
		GetComponent<ObjNavigation>().SetTarget (m_target.transform);
	}
	
	public void SetWaveSequenceOwner(WaveSequence a_waveOwner)
	{
		m_waveOwner = a_waveOwner;
	}
	
	void Awake()
	{
		m_camera = Camera.main;
		m_spawnController = GameObject.FindWithTag("SpawnController").GetComponent<SpawnController>();
		
		m_hitPointsStore = m_hitPoints;
		SetupProgressBar(m_hitPoints);
	}
	
	public	void Hit(float a_force, Vector3 a_forceDir, float a_damage)	
	{ 
		if(m_isDead && !GetComponent<Rigidbody>())
			return;
		
		if(m_pfxHit)
		{
			GameObject hitPFX = Instantiate(m_pfxHit, transform.position, transform.rotation) as GameObject;	
			hitPFX.transform.forward = a_forceDir;
		}
		
		if(m_pfxHitGround)
			Instantiate(m_pfxHitGround, new Vector3(transform.position.x, Random.Range(m_pfxGroundPos, m_pfxGroundPos + 0.3f), transform.position.z), transform.rotation);		
		
		if(m_pulseOnHit)
		{
			iTween.Stop(gameObject, "hit");
			iTween.PunchScale(gameObject, iTween.Hash( "name", "hit", 
														"amount", new Vector3(1.01f, 1.01f, 1.01f), 
														"time", 0.5f, 
														"looptype", "none" ) );		
		}
		
		if(m_isDead)
			return;
		
		m_hitPointsStore -= a_damage;
		if(m_hitPointsStore <= 0.0f)
		{
			m_hitPointsStore = 0.0f;
			m_isDead = true;
			
			if(!m_haveToldSpawnControllerWeHaveDied)
			{
				m_waveOwner.RegisterHasDied();
				m_haveToldSpawnControllerWeHaveDied = true;
			}
			
			if(m_spawnController)
			{
				m_spawnController.InformAllBuildingsEnemyDied(this);
			}
			
			if(m_detectBuildingTrigger)
			{
				m_detectBuildingTrigger.AlertAllBuildingsInRangeWeHaveDied();
				m_detectBuildingTrigger.gameObject.SetActive(false);
			}
			
			if(m_weapon)
				m_weapon.Kill();
				//m_weapon.SetIsDead(true);
		}
		else
		{
			SetProgressBar(m_hitPointsStore);
			return;
		}
		
		m_hitForce = a_force;
		m_hitForceDir = a_forceDir;
		
		if(!GetComponent<Rigidbody>())
		{
			gameObject.AddComponent<Rigidbody>();
			GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f));
		}
		
		if(GetComponent<NavMeshAgent>())
			GetComponent<NavMeshAgent>().enabled = false;
		
		if(GetComponent<ObjNavigation>())
			GetComponent<ObjNavigation>().enabled = false;
		
		if(GetComponent<BoxCollider>())
			GetComponent<BoxCollider>().isTrigger = false;
		
		gameObject.layer = 11; // dead layer
		
		transform.Rotate(new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2)), Random.Range(0, 360)); 
		
		m_hit = true; 
	}
		
	void LateUpdate()
	{
		if(GetComponent<Rigidbody>())
		{
			m_rigidbodyTimer += Time.deltaTime;
			if(m_rigidbodyTimer > m_canDestroyRigidbodyTimerMax)
			{
				if(GetComponent<Rigidbody>().velocity.magnitude <= 0.1f)
					Destroy(GetComponent<Rigidbody>());
				
				m_rigidbodyTimer = 0.0f;
			}
		}
		else if(!m_isDead)
		{
			if(m_isRanged)
			{
				/*
				if (!CanSeeTarget())
				{
					//SendMessage("StopSoundEffect");
					return;
				}
				
				// Rotate towards m_target	
				Vector3 m_targetPoint = m_target.transform.position;
				Quaternion m_targetRotation = Quaternion.LookRotation (m_targetPoint - transform.position, Vector3.up);
				transform.rotation = Quaternion.Slerp(transform.rotation, m_targetRotation, Time.deltaTime * m_rotationSpeed);
			
				// If we are almost rotated towards m_target - fire one clip of ammo
				var forward = transform.TransformDirection(Vector3.forward);
				var targetDir = m_targetPoint - transform.position;
				if (Vector3.Angle(forward, targetDir) < shootAngleDistance)
				{
					if(m_weapon)
						m_weapon.Fire ();
				}
				*/
			}
		}
		
		if(m_isDead && m_hit)
		{
			Vector3 explodePos = transform.position;
			explodePos.y = 0.0f;
			//rigidbody.AddExplosionForce(m_hitForce * 80.0f, explodePos, 0.0f);
			GetComponent<Rigidbody>().AddForce(Random.Range(-50.0f, 50.0f) + m_hitForceDir.x, ((m_hitForce + m_hitForceDir.y) * 80.0f), Random.Range(-50.0f, 50.0f) + m_hitForceDir.z);
			m_hit = false;
		}
		
		if(m_isDead && !GetComponent<Rigidbody>())
		{
			m_removeFromSceneTime -= Time.deltaTime;
			if(m_removeFromSceneTime <= 0.0f)
				Kill();
		}
	}	
	
	public void IncreaseAttributes(float a_speedIncPercentage, float a_damageIncPercentage, float a_hitPointsIncPercentage)
	{
		GetComponent<ObjNavigation>().speed += GetComponent<ObjNavigation>().speed * (a_speedIncPercentage / 100.0f);
		m_hitPointsStore += m_hitPointsStore * (a_hitPointsIncPercentage / 100.0f);		
		
		if(m_weapon)
		{
			m_weapon.SetDamageIncPercentage(a_damageIncPercentage);
			m_meleeHitDamage += m_meleeHitDamage * (a_damageIncPercentage / 100.0f);
		}
	}
	
	public void TargetEnteredTrigger(Building a_target)
	{
		if(!m_weapon)
			return;
		
		// need a target
		if(!m_weapon.m_autoAimTarget)
			m_weapon.m_autoAimTarget = a_target.gameObject;
		else // have a target
		{
			if(m_weapon.m_autoAimTarget.GetComponent<Building>().IsDead()) // make sure it is dead before swapping targets
				m_weapon.m_autoAimTarget = a_target.gameObject;
		}
	}
	
	public bool TargetExitedTrigger(Building a_target)
	{
		if(!m_weapon)
			return false;
		
		// don't track target anymore
		if(m_weapon.m_autoAimTarget == a_target.gameObject)
		{
			m_weapon.m_autoAimTarget = null;
			return true;
		}
		
		return false;
	}	
	
	
	void Kill()
	{		
		if(m_pfxDeath)
			Instantiate(m_pfxDeath, transform.position, transform.rotation);
		
		Destroy(gameObject);
	}
	
	public float DistanceFromDestination()
	{
		return GetComponent<ObjNavigation>().DistanceFromDestination();
	}
	
	// TODO: Remove this GUI and add a 3rd Party GUI system
	
    private Vector2 m_progressBarSize = new Vector2(30, 10);
    public Texture2D emptyTex;
    public Texture2D fullTex;
	
	private  float			m_barFullValue		= 1.0f;
	private float			m_barValue			= 1.0f;
	private	float			m_barFullWidth		= 100.0f;
	private	float			m_barFullHeight		= 1.0f;
	private float			m_ratio				= 1.0f;
	
	public void SetupProgressBar(float a_fullVal)
	{
		m_barFullValue = a_fullVal;
		m_ratio = 1.0f / m_barFullValue;
		
		m_barValue = m_barFullValue * m_ratio;			
		
		SetProgressBar(a_fullVal);
	}
	
	public void SetProgressBar(float val)
	{
		m_barValue = val * m_ratio;	
	}
	
    void OnGUI() 
	{
		if(m_isDead)
			return;
		
	   Vector2 progressBarPos = new Vector2(20, 40);
	   Vector3 screenPos = m_camera.WorldToScreenPoint(transform.position);
	   progressBarPos.x = screenPos.x - (m_progressBarSize.x * 0.5f);
	   progressBarPos.y = Screen.height - (screenPos.y + 35.0f);		
		
       GUI.BeginGroup(new Rect(progressBarPos.x, progressBarPos.y, m_progressBarSize.x, m_progressBarSize.y));
		 GUI.DrawTexture(new Rect(0,0, m_progressBarSize.x, m_progressBarSize.y), emptyTex);
         //GUI.Box(new Rect(0,0, m_progressBarSize.x, m_progressBarSize.y), emptyTex);
 
         GUI.BeginGroup(new Rect(0,0, m_progressBarSize.x * m_barValue, m_progressBarSize.y));
			GUI.DrawTexture(new Rect(0,0, m_progressBarSize.x, m_progressBarSize.y), fullTex);
          //GUI.Box(new Rect(0,0, m_progressBarSize.x, m_progressBarSize.y), fullTex);
         GUI.EndGroup();
       GUI.EndGroup();
    }	
}
