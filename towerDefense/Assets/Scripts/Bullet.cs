using UnityEngine;
using System.Collections;

public enum BulletType
{
	RAY_TRACE = 0,
	PROJECTILE,
}

public class Bullet : MonoBehaviour 
{
	public BulletType	m_type	=	BulletType.RAY_TRACE;
	public float 		m_traverseSpeed;
	public BlastRadius	m_blastRadius	=	null;
	private float		m_hitForce = 1.0f;
	private Vector3		m_endPos;
	public float		m_threshold = 1.5f;
	private bool		m_active = false;
	public bool IsActive()	{ return m_active; }
	public  float		m_rocketHitDamage = 20.0f;
	public  GameObject	m_onHitPFX	= null;
	public  bool 		m_clearOnHit = false;
	
	private Vector3		m_travelDir =  new Vector3(0.0f, 0.0f, 0.0f);
	
	private GameObject  m_objectToHit = null;
	
	private float		m_resetTimer 	= 0.0f;
	private float		m_resetTimerMax = 2.0f;
	
	private  string 		m_owner = "";
	public void SetOwner(string a_owner) { m_owner = a_owner; }	
	public string GetOwner()	{ return m_owner; } 
	
	void Awake()
	{
		GetComponent<Renderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_active)
		{
			if(m_type == BulletType.RAY_TRACE)
			{
				Vector3 direction = m_endPos - transform.position;
				float distance = direction.magnitude;
				
				if(distance < m_threshold)
				{
					/*
					if(m_type == BulletType.RAY_TRACE)
					{
						if(m_blastRadius)
							m_blastRadius.Blast(m_hitForce, m_rocketHitDamage);			
						
						if(m_onHitPFX)
							Instantiate(m_onHitPFX, transform.position, transform.rotation);
					}
					*/
					
					Reset();
				}
				
				transform.position += direction.normalized * ( m_traverseSpeed * Time.deltaTime );
			}
			else if(m_type == BulletType.PROJECTILE)
			{
				m_resetTimer += Time.deltaTime;
				if(m_resetTimer > m_resetTimerMax)
					Reset();
				
				transform.position += m_travelDir.normalized * ( m_traverseSpeed * Time.deltaTime );
			}
		}
	}
	
	public void Activate(Vector3 a_startPos, Vector3 a_endPos, float a_hitForce = 0.0f, GameObject a_objectToHit = null)
	{		
		if(m_active)
			return;
		
		m_active = true;
		transform.position = a_startPos;
		m_endPos = a_endPos;
		GetComponent<Renderer>().enabled = true;
		transform.forward = a_endPos;
		transform.LookAt(a_endPos);
		m_hitForce = a_hitForce;
		m_objectToHit = a_objectToHit;
		
		m_travelDir = m_endPos - transform.position;
	}
	
	void Reset()
	{
		if(m_owner == "")
			Kill();
		
		m_active = false;
		GetComponent<Renderer>().enabled = false;
		m_resetTimer = 0.0f;
		
		if(m_clearOnHit)
			if(m_blastRadius)
					m_blastRadius.Clear ();
	}
	
	void OnTriggerEnter(Collider a_obj)
	{
		if(!m_active)
			return;
		
		if(a_obj.tag == m_owner)
			return;
		
		if(a_obj.tag == "Enemy" || 
		   a_obj.tag == "EnvironmentStatic" ||
		   a_obj.tag == "EnvironmentDynamic" ||
		   a_obj.tag == "Building" ||
		   a_obj.tag == "Ground")
		{
			//print ("here and tag was: " + a_obj.tag);
			if(m_type == BulletType.PROJECTILE)
			{
				if(m_blastRadius)
					m_blastRadius.Blast(m_hitForce, m_rocketHitDamage);
			}
			
			if(m_onHitPFX)
				Instantiate(m_onHitPFX, transform.position, transform.rotation);
			
			Reset();
		}
	}	
	
	public void Kill()
	{
		Destroy(gameObject);
	}
}

