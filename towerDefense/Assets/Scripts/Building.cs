using UnityEngine;
using System.Collections;
using Pathfinding;

public class Building : MonoBehaviour 
{	
	private bool		m_isDead						= false;
	public  bool		IsDead()						{ return m_isDead; }
	
	public  Material		m_cantPlace 				= null;
	public  WeaponSystem 	m_weapon	 				= null;
	public  bool			m_disableDetectEnemyTriggerOnSpawn = false;
	
	public 	bool			m_isPlaced					= false;
	
	public  bool			m_autoCheckForNewTarget	= true;
	public  float			m_lookForNewTargetTime	= 1.0f;
	private float			m_lookForNewTargetTimer = 0.0f;
	
	private Material 		m_original  			= null; 
	private	Camera   		m_camera    			= null;
	private bool     		m_canPlace  			= true;
	
	private SpawnController	m_spawnController		= null;
	
	private ArrayList		m_targetsInRange		= new ArrayList();
	
	public  GameObject		m_buildingDetection		= null;
	
	public  int				m_cost					= 25;
	public  int 			m_upgradeCost			= 10;
	private bool			m_hasUpgraded			= false;
	public  bool HasUpgraded() { return m_hasUpgraded; }
	
	public float			m_hitPoints				= 100.0f;
	private float			m_hitPointsStore		= 100.0f;
	
	public	GameObject	m_pfxHit						= null;
	public	GameObject	m_pfxDeath						= null;
	public	GameObject	m_pfxHitGround					= null;
	public  float		m_pfxGroundPos					= 0.5f;
	public  bool        m_pulseOnHit					= true;
	
	public void AddToTargetsInRange(Enemy a_target)
	{
		//TrackTarget(a_target);
		m_targetsInRange.Add(a_target);
	}
	
	public void RemoveFromTargetsInRange(Enemy a_target)
	{
		m_targetsInRange.Remove(a_target);
		LoseTarget(a_target);
	}
	
	void Awake () 
	{		
		m_camera = Camera.main;
		m_spawnController = GameObject.FindWithTag("SpawnController").GetComponent<SpawnController>();
		
		m_hitPointsStore = m_hitPoints;
		SetupProgressBar(m_hitPoints);
			
		if(m_spawnController)
			m_spawnController.RegisterSpawnedBuilding(this);
		
		m_original = GetComponent<Renderer>().material;
		
		if(m_buildingDetection)
			m_buildingDetection.SetActive(false);
	}
	
	void Update()
	{
		if(m_autoCheckForNewTarget)
		{
			m_lookForNewTargetTimer += Time.deltaTime;
			if(m_lookForNewTargetTimer > m_lookForNewTargetTime)
			{
				TrackTarget(GetNextClosetTarget(), true);
				
				m_lookForNewTargetTimer = 0.0f;
			}			
		}
	}	
	
	public	void Hit(Vector3 a_forceDir, float a_damage)	
	{ 
		if(!m_isPlaced)
			return;
		
		if(m_isDead)
			return;
		
		if(m_pfxHit)
		{
			GameObject hitPFX = Instantiate(m_pfxHit, transform.position, transform.rotation) as GameObject;		
			hitPFX.transform.forward = a_forceDir;
			//hitPFX.transform.LookAt(-a_forceDir);
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
		
		m_hitPointsStore -= a_damage;
		if(m_hitPointsStore <= 0.0f)
		{
			m_hitPointsStore = 0.0f;
			m_isDead = true;
			Kill();
		}
		SetProgressBar(m_hitPointsStore);
	}	
		
	public bool HasTarget()
	{
		if(!m_weapon)
			return true; // we have no weapon no point in trying to find a target
		
		if(!m_weapon.m_autoAimTarget)
			return false;
		
		if(m_weapon.m_autoAimTarget)
			if(m_weapon.m_autoAimTarget.GetComponent<Enemy>().IsDead())
				return false;
		
		return true;
	}
		
	public void TrackTarget(Enemy a_target, bool a_forceTargetChange = false)
	{
		if(!m_weapon)
			return;
		
		if(!a_target)
			return;
		
		//print ("a_target: " + a_target);
		//print ("m_weapon.m_autoAimTarget: " + m_weapon.m_autoAimTarget);
		// need a target
		if(!m_weapon.m_autoAimTarget)
			m_weapon.m_autoAimTarget = a_target.gameObject;
		else // have a target
		{
			if(a_forceTargetChange)
				m_weapon.m_autoAimTarget = a_target.gameObject;
			else if(m_weapon.m_autoAimTarget.GetComponent<Enemy>().IsDead()) // make sure it is dead before swapping targets
				m_weapon.m_autoAimTarget = a_target.gameObject;
		}
	}
	
	public bool LoseTarget(Enemy a_target)
	{
		if(!m_weapon)
			return false;
		
		if(!a_target)
			return false;
		
		// don't track target anymore
		//print ("m_weapon.m_autoAimTarget = " + m_weapon.m_autoAimTarget + "     ==     a_target.gameObject = " + a_target.gameObject);
		if(m_weapon.m_autoAimTarget == a_target.gameObject)
		{
			/*Enemy enemy = GetNextClosetTarget();
			
			if(enemy)
				m_weapon.m_autoAimTarget = enemy.gameObject;
			else*/
			m_weapon.m_autoAimTarget = null;
			
			return true;
		}
		
		return false;
	}
	
	public void EnemyDied(Enemy a_deadEnemy)
	{
		if(!m_weapon)
			return;
		
		if(!m_weapon.m_autoAimTarget)
			return;
		
		if(!a_deadEnemy)
			return;
		
		if(m_weapon.m_autoAimTarget == a_deadEnemy.gameObject)
		{
			m_weapon.m_autoAimTarget = null;
		}
	}
	
	public void SetCanPlace(bool a_placeEnabled)	
	{ 
		if(m_isPlaced)
			return;
		
		m_canPlace = a_placeEnabled; 
		
		if(m_cantPlace && !m_canPlace)
		{
			ChangeMaterial(gameObject, m_cantPlace);
		}
		else
		{
			ChangeMaterial(gameObject, m_original);
		}
	}
			
	public void Reposition (Vector3 a_pos) 
	{
		transform.position = a_pos;
	}
	
	public bool PlaceBuilding()
	{
		if(!m_canPlace)
			return false;
		
		RemoveCollisionFromAStar(gameObject);
		
		if(m_buildingDetection)
			m_buildingDetection.SetActive(true);
		
		m_isPlaced = true;
				
		return true;
	}
	
	void RemoveCollisionFromAStar(GameObject a_obj, bool a_direct = false)
	{
		if(!a_obj)
			return;
		
		if(a_obj.GetComponent<Collider>())
		{
			Bounds b = a_obj.GetComponent<Collider>().bounds;
		
			GraphUpdateObject guo = new GraphUpdateObject(b);
			AstarPath.active.UpdateGraphs (guo);

			if (a_direct)
				AstarPath.active.FlushGraphUpdates();
		}
			
		bool childExists = true;
		int cIndex = 0;
		while(childExists)
		{
			if(cIndex >= a_obj.transform.childCount)
			{
				break;
			}
			
			GameObject child = a_obj.transform.GetChild(cIndex).gameObject;
			
			if(!child)
			{
				childExists = false;
			}
			else
			{
				if(child.GetComponent<Collider>())
				{
					Bounds b = child.GetComponent<Collider>().bounds;
				
					GraphUpdateObject guo = new GraphUpdateObject(b);
					AstarPath.active.UpdateGraphs (guo);
	
					if (a_direct)
						AstarPath.active.FlushGraphUpdates();
				}
				
				RemoveCollisionFromAStar(child, a_direct);
			}
			
			cIndex++;
		}
	}
	
	void ChangeMaterial(GameObject a_obj, Material a_material)
	{
		if(!a_obj)
			return;
		
		if(a_obj.GetComponent<Renderer>())
			a_obj.GetComponent<Renderer>().material = a_material;
			
		bool childExists = true;
		int cIndex = 0;
		while(childExists)
		{
			if(cIndex >= a_obj.transform.childCount)
			{
				break;
			}
			
			GameObject child = a_obj.transform.GetChild(cIndex).gameObject;
			
			if(!child)
			{
				childExists = false;
			}
			else
			{			
				ChangeMaterial(child, a_material);
			}
			
			cIndex++;
		}
	}	
	
	void Dead()
	{
		if(m_spawnController)
			m_spawnController.RemoveSpawnedBuilding(this);
	}
	
	
	public Enemy GetNextClosetTarget()
	{
		if(m_targetsInRange.Count > 0)
		{
			Enemy enemyToReturn = m_targetsInRange[0] as Enemy;
			float distance = enemyToReturn.DistanceFromDestination();
			
			for(int tIndex = 0; tIndex < m_targetsInRange.Count; tIndex++)
			{
				Enemy objCompare = m_targetsInRange[tIndex] as Enemy;
				
				float distanceCompare = objCompare.DistanceFromDestination();
				
				if(distanceCompare < distance)
				{
					enemyToReturn = objCompare;
					distance = distanceCompare;
				}
			}
			
			return enemyToReturn;
		}		
		
		return null;
	}
	
	void Kill()
	{
		if(m_pfxDeath)
			Instantiate(m_pfxDeath, transform.position, transform.rotation);
		
		if(m_weapon)
			m_weapon.Kill();

		AstarPath.active.Scan();
		
		Destroy(gameObject);
	}
	
	public void PurchaseUpgrade()
	{
		if(m_weapon)
		{
			// TODO: add your custom upgrade code in here
			m_weapon.PurchaseUpgrade(); 
			
			float m_upgradePercentage = 0.4f;
			m_hitPoints += m_hitPointsStore * m_upgradePercentage; 		
			m_hitPointsStore += m_hitPointsStore * m_upgradePercentage; 		
			m_progressBarSize.x += m_progressBarSize.x * m_upgradePercentage;
			SetupProgressBar(m_hitPoints);
		}
		
		m_hasUpgraded = true;
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
