using UnityEngine;
using System.Collections;

public class iTweenPathOptions : MonoBehaviour 
{
	public 	bool 		m_traversePath 	= true;
	public	float		m_traverseSpeed = 10.0f;
	public  bool		m_orientToPath	= false;
	public  float		m_lookTime		= 0.0f;
	public  string		m_loopOption	= "loop";
	
	// random timing for pathing to start
	private bool		m_pathingStarted 			= false;
	public 	float		m_possiblePathWaitTimeMin 	= 0.0f; 
	public 	float		m_possiblePathWaitTimeMax 	= 0.0f; 
	private float		m_waitTimer 				= 0.0f;
	private float		m_randomGenWaitTime 		= 0.0f;
	
	public  bool		m_destroyOnPathEnd			= false;
	public  bool		m_stopOnPathEnd				= false;
	
	public bool 		m_positionAtBeginningOnPathEnd	 = false;
	
	private Vector3 m_originalPos					= new Vector3(0.0f, 0.0f, 0.0f);
	private Quaternion m_originalRotation;
	
	void Awake()
	{		
		m_originalPos = gameObject.transform.position;
		m_originalRotation = gameObject.transform.rotation;
	}
	
	// Use this for initialization
	void Start () 
	{
		GenerateRandomWaitTime();
	}
	
	void Update()
	{
		if(!m_pathingStarted)
		{
			m_waitTimer += Time.deltaTime;
			if(m_waitTimer > m_randomGenWaitTime)
			{
				if(m_traversePath)
				{
					iTween.MoveTo( gameObject, iTween.Hash( "path", GetComponent<iTweenPath>().GetPath(), 
														   "time", m_traverseSpeed, 
						 								   "orienttopath", m_orientToPath, 
						 								   "looktime", m_lookTime, 
														   "easetype", "easeInOutSine", 
														   "looptype", m_loopOption,
														   "onstart", "PathingStarted",
														   "oncomplete", "PathingEnded" ) );
				}
			}
		}
	}
	
	void PathingStarted()
	{
		m_pathingStarted = true;
	}
	
	void PathingEnded()
	{				
		if(m_positionAtBeginningOnPathEnd)
			PositionAtStart();
		
		if(m_stopOnPathEnd)
		{
			m_traversePath = false;
			return;
		}
		
		if(m_destroyOnPathEnd)
			Kill();
		
		m_pathingStarted = false;
		GenerateRandomWaitTime();
		iTween.Stop(gameObject);
	}
	
	public void PositionAtStart()
	{
		iTween.Stop(gameObject);
		GenerateRandomWaitTime();
		gameObject.transform.position = m_originalPos;
		gameObject.transform.rotation = m_originalRotation;
		m_waitTimer = 0.0f;
		m_traversePath = false;
		m_pathingStarted = false;
	}
	
	void GenerateRandomWaitTime()
	{
		m_randomGenWaitTime = Random.Range(m_possiblePathWaitTimeMin, m_possiblePathWaitTimeMax);
	}
	
	void Kill()
	{
		Destroy(gameObject);
	}
	
	public void RepathWithSpeed(float a_traverseSpeed, bool a_stopAllTweensFirst = false)
	{
		if(a_stopAllTweensFirst)		
			iTween.Stop (gameObject);
	
		iTween.MoveTo( gameObject, iTween.Hash( "path", GetComponent<iTweenPath>().GetPath(), 
																   "time", a_traverseSpeed, 
								 								   "orienttopath", m_orientToPath, 
								 								   "looktime", m_lookTime, 
																   "easetype", "easeInOutSine", 
																   "looptype", m_loopOption,
																   "onstart", "PathingStarted",
																   "oncomplete", "PathingEnded" ) );		
	}
	
	public void SetTraversePath(bool a_traversePath)	{ m_traversePath = a_traversePath; }
}
