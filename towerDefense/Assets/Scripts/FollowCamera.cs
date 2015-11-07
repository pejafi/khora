using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour 
{
	//public GameObject 	m_followObj 	= null;
	public Vector3 	m_offsetPos		= new Vector3(0.0f, 0.0f, 0.0f);
	
	public void Enable (GameObject a_followObj) 
	{
		GetComponent<Camera>().enabled = true;
		//m_followObj = a_followObj;
		transform.position = m_target.transform.position + m_offsetPos;
		
		m_target = a_followObj;
	}
	
	public void Disable () 
	{
		GetComponent<Camera>().enabled = false;	
		//m_followObj = null;
		
		m_target = null;
	}
	
	/*
	void Update()
	{
		if(m_followObj)
			transform.position = m_followObj.transform.position + m_offsetPos;
		else if(GetComponent<Camera>().enabled)
			Disable();
	}
	*/
	
    public  GameObject  m_target          = null;
    //public  float       m_height          = 9.0f;
    //public  float       m_zDistance       = 0.0f;	
    public  float       m_zoomInHeight    = 6.0f;
    public  float       m_positionDamping = 2.0f;
    private Transform   m_targetTransform = null;
    //private Camera      m_camera          = null;

    public  float       m_zoomedInFov = 45f;
    public  float       m_normalFov   = 60f;
    public  float       m_zoomInSpeed =  5f;

    private CharacterController m_characterController;
    private Transform  m_transform;
    private Quaternion m_initialOrientation;
	
	private bool		m_isTargetDead	= false;
	public void TargetIsDead()	{ m_isTargetDead = true; }

    void Awake()
    {
        //m_camera = camera;
        m_characterController = GetComponent<CharacterController>();
        m_transform = transform;
        m_initialOrientation = m_transform.rotation;
//        m_colorCorrection = GetComponent<ColorCorrectionCurves>();
//        m_redKeyframe = m_redFlashCurve[1];
//        m_redKeyframe.value = m_redKeyframe.time;
//
//        if(m_colorCorrection)
//            m_colorCorrection.enabled = false;
    }

    void Update()
    {
		if(m_isTargetDead)
			return;
		
        if (!m_target)
            return;
		
        if(m_targetTransform==null)
            m_targetTransform = m_target.transform;

        Vector3 targetPosition = m_targetTransform.position;
        targetPosition.y += m_offsetPos.y;//m_height;

        targetPosition.x = m_targetTransform.position.x + m_offsetPos.x;
        targetPosition.z = m_targetTransform.position.z + m_offsetPos.z;//m_zDistance;

        m_characterController.Move(Vector3.Lerp(transform.position, targetPosition, m_positionDamping * Time.deltaTime) - transform.position);

       // float dist = Vector3.Distance(targetPosition, transform.position);
		
		/*
        if(dist >= 6f)
        {
            m_transform.position = targetPosition;
            //m_target.ForceUnlockCamera();
        }
        */

		/*
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.BackQuote))
            m_camera.fov = Mathf.Lerp(m_camera.fov, m_zoomedInFov, Time.deltaTime * m_zoomInSpeed);
        else
            m_camera.fov = Mathf.Lerp(m_camera.fov, m_normalFov, Time.deltaTime * m_zoomInSpeed);
#else
        m_camera.fov = Mathf.Lerp(m_normalFov, m_zoomedInFov, Input.mousePosition.normalized.magnitude);
#endif
		*/

	}

    void LateUpdate()
    {
		if(m_isTargetDead)
			return;
		
        m_transform.rotation = Quaternion.Slerp(m_transform.rotation, m_initialOrientation, Time.deltaTime * 2f);
	}
}
