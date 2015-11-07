using UnityEngine;
using System.Collections;

public enum WeaponType
{
	PISTOL = 0,
	MACHINE_GUN,
	ROCKET,
	FLAME,
	LASER,
	GRENADE,
	MELEE,
	PASSIVE,
}

public class WeaponSystem : MonoBehaviour 
{
	public WeaponType m_weaponType = WeaponType.PISTOL;
	public LayerMask  m_fireAtLayerMask;
	public float m_fireRange = 100.0f;
	public float fireRate = 0.05f;
	public int bulletsPerClip = 40;
	public int clips = 20;
	public bool	m_unlimitedAmmo					= false;
	public float reloadTime = 0.5f;
	public Renderer[] muzzleFlash;
	public float		m_hitForce						= 10.0f;
	public float		m_hitDamage						= 1.0f;

	private int bulletsLeft = 0;
	private float nextFireTime = 0.0f;
	private int m_LastFrameShot = -1;
	
	public GameObject[] m_pointFire;
	private int			m_pointFireIndex = 0;
	public int m_numOfBulletsToInstantiate = 0;
	public GameObject m_bullet = null;
	private ArrayList m_bullets;
	private int m_bulletUsedIndex = 0;
	
	private bool m_isDead = false;
	public bool GetIsDead()	{ return m_isDead; }
	//public bool SetIsDead(bool a_isDead) { m_isDead = a_isDead; }
	
	int GetBulletsLeft () { return bulletsLeft; }
	public WeaponType GetWeaponType()	{	return m_weaponType; }
	
	public 	float m_warmUpTimeTillFire		=	0.0f;
	private float m_warmUpTimeTillFireStore	=	0.0f;
	
	public  bool  			m_autoAim 						= false;
	public  GameObject 		m_autoAimTarget					= null;
	public 	float 			m_autoAimAttackRange 			= 30.0f;
	public 	float 			m_autoAimShootAngleDistance 	= 10.0f;
	public 	float 			m_autoAimRotationSpeed 			= 2.0f;
	public  LayerMask		m_enemyLayer;
	public  bool			m_autoAimMustSeeTargetToFire	= false;
	
	private float 			m_damageIncPercentage			= 0.0f;
	public void SetDamageIncPercentage(float a_percentage) 
	{ 
		m_damageIncPercentage = a_percentage;
		m_hitDamage += m_hitDamage * (m_damageIncPercentage / 100.0f);
	}
	
	public void StopSoundEffect()
	{
		if(GetComponent<AudioSource>())
			if(GetComponent<AudioSource>().isPlaying)
				GetComponent<AudioSource>().Stop();
	}
	
	void Start () 
	{
		m_warmUpTimeTillFireStore = m_warmUpTimeTillFire;
		
		bulletsLeft = bulletsPerClip;
		
		if(m_pointFire.Length > 0)
		{
			// put all the bullet trails at the end of the firing point
			m_bullets = new ArrayList(m_numOfBulletsToInstantiate);
			for(int bTIndex = 0; bTIndex < m_numOfBulletsToInstantiate; bTIndex++)
			{
				GameObject projectile = Instantiate(m_bullet) as GameObject;
				projectile.transform.position = m_pointFire[0].transform.position;
				//m_bullets[bTIndex].transform.parent = m_pointFire.transform;
				
				// hack
				string ownerTag = transform.parent.tag;
				if(ownerTag == "BuildingDetection")
					ownerTag = "Building";
				
				projectile.GetComponent<Bullet>().SetOwner(ownerTag);
				
				m_bullets.Add(projectile);
			}
		}
		
		// hide muzzles
		for(int bTIndex = 0; bTIndex < muzzleFlash.Length; bTIndex++)
		{
			muzzleFlash[bTIndex].GetComponent<Renderer>().enabled = false;
		}
		
	}
	
	void Update() 
	{
		if(m_isDead)
		{
			Destroy(gameObject);
			return;
		}

		if(!m_autoAim)
			return;
		
		if(!m_autoAimTarget)
			return;
		
		if (!CanSeeTarget())
		{
			//SendMessage("StopSoundEffect");
			return;
		}
		
		// Rotate towards m_target	
		Vector3 m_targetPoint = m_autoAimTarget.transform.position;
		Quaternion m_targetRotation = Quaternion.LookRotation (m_targetPoint - transform.position, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, m_targetRotation, Time.deltaTime * m_autoAimRotationSpeed);
	
		// If we are almost rotated towards m_target - fire one clip of ammo
		var forward = transform.TransformDirection(Vector3.forward);
		var targetDir = m_targetPoint - transform.position;
		if (Vector3.Angle(forward, targetDir) < m_autoAimShootAngleDistance)
		{
			Fire ();
		}
	}
	
	bool CanSeeTarget()
	{
		if(!m_autoAimTarget)
			return false;
		
		if (Vector3.Distance(transform.position, m_autoAimTarget.transform.position) > m_autoAimAttackRange)
			return false;
		
		if(m_autoAimMustSeeTargetToFire)
		{
			RaycastHit hit;
			if (Physics.Linecast (m_pointFire[m_pointFireIndex].transform.position, m_autoAimTarget.transform.position, out hit, m_enemyLayer))
			{
				return hit.transform == m_autoAimTarget.transform;
			}
		}
		else
			return true;
			
		return false;
	}
	
	void LateUpdate() 
	{
		if (muzzleFlash.Length > 0) 
		{
			int mFIndex = m_pointFireIndex;
			if(mFIndex < 0) { mFIndex = muzzleFlash.Length - 1; }
			
			// We shot this frame, enable the muzzle flash
			if (m_LastFrameShot == Time.frameCount) 
			{
				muzzleFlash[m_pointFireIndex].transform.localRotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
				muzzleFlash[m_pointFireIndex].enabled = true;
	
				if (GetComponent<AudioSource>()) 
				{
					if (!GetComponent<AudioSource>().isPlaying)
						GetComponent<AudioSource>().Play();
					
					GetComponent<AudioSource>().loop = true;
				}
			} 
			else 
			{
				// We didn't shoot so disable the muzzle flash
				muzzleFlash[m_pointFireIndex].enabled = false;
				//enabled = false;
				
				// Stop playing sound
				if (GetComponent<AudioSource>())
					GetComponent<AudioSource>().loop = false;
			}
		}
	}
	
	public void ResetFire()
	{
		m_warmUpTimeTillFireStore = m_warmUpTimeTillFire;
	}
	
	public void Fire () 
	{
		m_warmUpTimeTillFireStore -= Time.deltaTime;
		if(m_warmUpTimeTillFireStore > 0.0f)
			return;
		
		m_warmUpTimeTillFireStore = m_warmUpTimeTillFire;
		
		if (bulletsLeft == 0 && !m_unlimitedAmmo)
		{
			StopSoundEffect();
			return;
		}
		
		// If there is more than one bullet between the last and this frame
		// Reset the nextFireTime
		if (Time.time - fireRate > nextFireTime)
			nextFireTime = Time.time - Time.deltaTime;
		
		// Keep firing until we used up the fire time
		while( nextFireTime < Time.time && bulletsLeft != 0) 
		{
			FireOneShot();
			nextFireTime += fireRate;
		}
	}
	
	void FireOneShot () 
	{
		if(m_isDead)
			return;

		
		Vector3 direction = m_pointFire[m_pointFireIndex].transform.TransformDirection(Vector3.forward);
		RaycastHit hit;
		
		// Did we hit anything?
		if (Physics.Raycast (m_pointFire[m_pointFireIndex].transform.position, direction, out hit, m_fireRange, m_fireAtLayerMask)) 
		{
			//print ("hit: " + hit.collider.gameObject.name);
			if(hit.collider.tag == "Building")
			{
				Building building = hit.collider.gameObject.GetComponent<Building>();
				if(!building.m_isPlaced || building.IsDead())
					return;
			}
			
			// Apply a force to the rigidbody we hit
			/*
			if (hit.rigidbody)
				hit.rigidbody.AddForceAtPosition(force * direction, hit.point);
			*/
			if(m_weaponType == WeaponType.ROCKET)
			{
				// bullet class applies damange
			}
			else
			{
				if(hit.collider.tag == "Enemy")
				{
					// Send a damage message to the hit object
					hit.transform.gameObject.GetComponent<Enemy>().Hit(m_hitForce, transform.TransformDirection(Vector3.forward), m_hitDamage);
				}
				else if(hit.collider.tag == "Building")
				{
					// Send a damage message to the hit object
					hit.transform.gameObject.GetComponent<Building>().Hit(direction, m_hitDamage);
				}
				// TODO: remove this "&& hit.collider.gameObject.name == "Player"" if you want enemies to shoot enemies
				else if(hit.collider.gameObject != gameObject )//&& hit.collider.gameObject.name == "Player") // don't kill yourself
				{
					// Send a damage message to the hit object			
				}
			}
			
			// effects
			GameObject projectile = m_bullets[m_bulletUsedIndex] as GameObject;
			
			if(projectile.GetComponent<Bullet>())
			{
				if(m_weaponType == WeaponType.ROCKET)
					projectile.GetComponent<Bullet>().m_rocketHitDamage = projectile.GetComponent<Bullet>().m_rocketHitDamage + (projectile.GetComponent<Bullet>().m_rocketHitDamage * (m_damageIncPercentage / 100.0f));
				
				projectile.GetComponent<Bullet>().Activate(m_pointFire[m_pointFireIndex].transform.position, hit.point, m_hitForce, hit.collider.gameObject);
			}
		}
		
		bulletsLeft--;
		
		m_bulletUsedIndex++;
		if(m_bulletUsedIndex >= m_bullets.Count)
		{
			m_bulletUsedIndex = 0;
		}
		
		m_pointFireIndex++;
		if(m_pointFireIndex >= m_pointFire.Length)
		{
			m_pointFireIndex = 0;
		}
				
		// Register that we shot this frame,
		// so that the LateUpdate function enabled the muzzleflash renderer for one frame
		m_LastFrameShot = Time.frameCount;
		//enabled = true;
		
		// Reload gun in reload Time		
		if (bulletsLeft == 0)
			StartCoroutine(Reload());			
	}
	
	IEnumerator Reload () 
	{
		StopSoundEffect();
		
		// Wait for reload time first - then add more bullets!
		yield return new WaitForSeconds(reloadTime);
	
		// We have a clip left reload
		if (clips > 0) 
		{
			clips--;
			bulletsLeft = bulletsPerClip;
		}
		else if(m_unlimitedAmmo) // unlimited
		{
			bulletsLeft = bulletsPerClip;
		}
	}
	
	public void Kill()
	{
		m_isDead = true;
		
		// put all the bullet trails at the end of the firing point
		for(int bTIndex = 0; bTIndex < m_bullets.Count; bTIndex++)
		{
			GameObject projectile = m_bullets[bTIndex] as GameObject;
			
			if(!projectile.GetComponent<Bullet>().IsActive())
				projectile.GetComponent<Bullet>().Kill();
			else
				projectile.GetComponent<Bullet>().SetOwner("");
		}	
		
		m_bullets.Clear();
	}
	
	public void PurchaseUpgrade()
	{
		Vector3 scale = transform.localScale;
		scale = scale * 1.25f;
		transform.localScale = scale;
		
		float m_upgradePercentage = 0.4f;
		fireRate -= (fireRate * m_upgradePercentage);
		SetDamageIncPercentage(m_upgradePercentage * 100.0f);
		m_autoAimAttackRange += m_autoAimAttackRange * m_upgradePercentage;
		m_fireRange += m_fireRange * m_upgradePercentage;
	}
}