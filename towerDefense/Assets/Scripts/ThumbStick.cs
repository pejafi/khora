using UnityEngine;
using System.Collections;

public class ThumbStick 
	: MonoBehaviour 
{	
	#region Enums
	
		// Determines the location of the thumbstick on the mobile display
		public enum enThumbStickLocation
		{
			LowerLeft,
			LowerRight,
		};
	
		// Event types we can notify subscribers about
		public enum enThumbStickEvent
		{
			MoveStart,
			MoveHold,
			MoveChange,
			MoveEnd,
			Click,
		};

	#endregion

	#region Delegates
	
		// Delegate (method signature) for events
		public delegate void deThumbStickEvent (enThumbStickEvent tsE, Vector2 pos);

	#endregion

	#region Events
	
		// Event to notify subscribers
		public event deThumbStickEvent evThumbStickEvent;

	#endregion


	#region Properties
	
		// Returns the angle of the thumbstick expressed degrees.
		// 000 = X+
		// 090 = Z-
		// 180 = X-
		// 270 = Z+
		public float StickAngle
		{
			get { return WrapAngle( AngleFromVector(m_offSet) + 90.0f); }
		}
	
		// Returns the horizontal velocity of the thumbstick where fully left = -1 and fully right = +1
		public float StickVelocityX
		{
			get 
			{ 
				return ( (m_lastTouchPos.x - m_zone.xMin) / (m_zone.xMax - m_zone.xMin) - 0.5f ) * 2.0f;								
			}
		}
	
		// Returns the vertical velocity of the thumbstick where fully up = -1 and fully down = +1
		public float StickVelocityY
		{
			get 
			{ 
				return ( (m_lastTouchPos.y - m_zone.yMin) / (m_zone.yMax - m_zone.yMin) - 0.5f ) * 2.0f;								
			}
		}
	
		// Determines if the thumbstick is currently being touched / moved
		public bool Active
		{
			get { return m_active; }
		}

	#endregion

	#region Members
		
		// The texture to use for the thumbstick
		public Texture m_StickTexture;
	
		public float   m_maxRadius = 100.0f;

		// Determines how much of the screen (0.0 - 1.0f horizontally and vertically) to use for the thumbstick zone
		public Vector2 m_ZoneSize
			= new Vector2(0.2f, 0.2f);

		// Determines the location of the thumbstick on the mobile display
		public enThumbStickLocation
			m_StickLocation = enThumbStickLocation.LowerLeft;
	
		// Determines how large to draw the thumbstick.  This does not affect the active zone.
		public float		
			m_StickScale = 0.5f;		

		private Rect m_zone;
		private bool m_active;

		private Vector2 m_origin;
		private Vector2 m_offSet;		
		private Vector2 m_lastTouchPos;
		private int     m_fingerID;

	#endregion
	
	void Start () 
	{	
		SetupThumbStick();
	}	
	
	void Update () 
	{	
		UpdateThumbStick();
	}
	
	// Renders the thumbstick and optionally additional debug info
	void OnGUI()
	{
		GUI.DrawTexture(GetStickRect(m_zone, m_offSet), m_StickTexture);		
	}
	
	// Initialize the thumbstick
	private void SetupThumbStick()
	{		
		float xS = m_ZoneSize.x * (float)Screen.width;
		float yS = m_ZoneSize.y * (float)Screen.height;

		switch (m_StickLocation)
		{
			case enThumbStickLocation.LowerLeft:
				
				m_zone = new Rect
					(0, Screen.height - yS, xS, yS);
				
				break;

			case enThumbStickLocation.LowerRight:

				m_zone = new Rect
					(Screen.width - xS, Screen.height - yS, xS, yS);

				break;
		}

		m_origin   = GetZoneCentre(m_zone);
		m_offSet   = Vector2.zero;
		m_fingerID = -1;
		m_active   = false;	

		m_lastTouchPos = m_origin;	
	}
	
	// Update the state of the thumbstick
	private void UpdateThumbStick()
	{
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE
		if((Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)) && m_StickLocation == enThumbStickLocation.LowerLeft)
		{
			// Get the touch position
			Vector2 m_touchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			// Invert the Y axis 
			m_touchPos.y 
				= Screen.height - m_touchPos.y;				

			// Is this a new touch?
			if (Input.GetMouseButtonDown(0))
			{
				// Is it inside the active zone?
				if (m_zone.Contains(m_touchPos))
				{
					m_origin   = GetZoneCentre(m_zone);
					m_offSet   = Vector2.zero;
					m_active   = true;

					m_lastTouchPos = m_touchPos;

					RaiseEvent(enThumbStickEvent.MoveStart, m_touchPos);
				}
			}	
			else
			{
				/* old box way
				m_touchPos.x = Mathf.Clamp(m_touchPos.x, m_zone.xMin, m_zone.xMax);
				m_touchPos.y = Mathf.Clamp(m_touchPos.y, m_zone.yMin, m_zone.yMax);						
				*/
				
				m_lastTouchPos = m_touchPos;
				
				// Use a circle instead of a box.
				Vector3 pivot = m_origin;
				Vector3 point; 
				Vector3 pointRelatedToPivot;
				point = m_touchPos;
				pointRelatedToPivot = point - pivot;
				if( pointRelatedToPivot.sqrMagnitude>Mathf.Pow(m_maxRadius,2))
				{  
				    point = pivot + pointRelatedToPivot.normalized * m_maxRadius;
				}
				m_touchPos = point;				
				
				m_offSet = m_touchPos - m_origin;	
				
				//RaiseEvent(enThumbStickEvent.MoveHold,   m_touchPos);					
				RaiseEvent(enThumbStickEvent.MoveChange, m_touchPos);					
			}	
			
			// Is the touch finished?
			if (Input.GetMouseButtonUp(0))
			{
				m_origin   = GetZoneCentre(m_zone);
				m_offSet   = Vector2.zero;
				m_fingerID = -1;
				m_active   = false;	

				m_lastTouchPos = m_origin;		
				
				RaiseEvent(enThumbStickEvent.MoveEnd, m_touchPos);							
			}						
		}
		
		if((Input.GetMouseButton(1) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1)) && m_StickLocation == enThumbStickLocation.LowerRight)
		{
			// Get the touch position
			Vector2 m_touchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			// Invert the Y axis 
			m_touchPos.y 
				= Screen.height - m_touchPos.y;				

			// Is this a new touch?
			if (Input.GetMouseButtonDown(1))
			{
				// Is it inside the active zone?
				if (m_zone.Contains(m_touchPos))
				{
					m_origin   = GetZoneCentre(m_zone);
					m_offSet   = Vector2.zero;
					m_active   = true;

					m_lastTouchPos = m_touchPos;

					RaiseEvent(enThumbStickEvent.MoveStart, m_touchPos);
				}
			}	
			else
			{
				m_lastTouchPos = m_touchPos;

				// Use a circle instead of a box.
				Vector3 pivot = m_origin;
				Vector3 point; 
				Vector3 pointRelatedToPivot;
				point = m_touchPos;
				pointRelatedToPivot = point - pivot;
				if( pointRelatedToPivot.sqrMagnitude>Mathf.Pow(m_maxRadius,2))
				{  
				    point = pivot + pointRelatedToPivot.normalized * m_maxRadius;
				}
				m_touchPos = point;				
				
				m_offSet = m_touchPos - m_origin;	
				
				//RaiseEvent(enThumbStickEvent.MoveHold,   m_touchPos);					
				RaiseEvent(enThumbStickEvent.MoveChange, m_touchPos);					
			}	
			
			// Is the touch finished?
			if (Input.GetMouseButtonUp(1))
			{
				m_origin   = GetZoneCentre(m_zone);
				m_offSet   = Vector2.zero;
				m_fingerID = -1;
				m_active   = false;	

				m_lastTouchPos = m_origin;		
				
				RaiseEvent(enThumbStickEvent.MoveEnd, m_touchPos);							
			}						
		}
		
#elif UNITY_IPHONE || UNITY_ANDROID		
		// Is the user touching the display?
		if (Input.touchCount > 0)
		{
			// Loop over each touch
			foreach (Touch touch in Input.touches) 
			{		
				// Get the touch position
				Vector2 m_touchPos = touch.position;
	
				// Invert the Y axis 
				m_touchPos.y 
					= Screen.height - m_touchPos.y;				

				// Is this a new touch?
				if (touch.phase == TouchPhase.Began)
				{
					// Is it inside the active zone?
					if (m_zone.Contains(m_touchPos))
					{
						m_origin   = GetZoneCentre(m_zone);
						m_offSet   = Vector2.zero;
						m_fingerID = touch.fingerId;
						m_active   = true;

						m_lastTouchPos = m_touchPos;

						RaiseEvent(enThumbStickEvent.MoveStart, m_touchPos);
					}
				}		
				
				// Is this an existing touch?
				if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
				{
					// Is it the same touch we are currently tracking?
					if (touch.fingerId == m_fingerID)
					{	
						/*
						m_touchPos.x = Mathf.Clamp(m_touchPos.x, m_zone.xMin, m_zone.xMax);
						m_touchPos.y = Mathf.Clamp(m_touchPos.y, m_zone.yMin, m_zone.yMax);						
					
						m_offSet = m_touchPos - m_origin;	
						 */
						
						// Use a circle instead of a box.
						Vector3 pivot = m_origin;
						Vector3 point; 
						Vector3 pointRelatedToPivot;
						point = m_touchPos;
						pointRelatedToPivot = point - pivot;
						if( pointRelatedToPivot.sqrMagnitude>Mathf.Pow(m_maxRadius,2))
						{  
						    point = pivot + pointRelatedToPivot.normalized * m_maxRadius;
						}
						m_touchPos = point;				
						
						m_offSet = m_touchPos - m_origin;	
						
						m_lastTouchPos = m_touchPos;

						if (touch.phase == TouchPhase.Stationary) RaiseEvent(enThumbStickEvent.MoveHold,   m_touchPos);					
						if (touch.phase == TouchPhase.Moved     ) RaiseEvent(enThumbStickEvent.MoveChange, m_touchPos);					
					}
				}	
				
				// Is the touch finished?
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					// Is it the same touch we are currently tracking?
					if (touch.fingerId == m_fingerID)
					{
						m_origin   = GetZoneCentre(m_zone);
						m_offSet   = Vector2.zero;
						m_fingerID = -1;
						m_active   = false;	

						m_lastTouchPos = m_origin;		
						
						RaiseEvent(enThumbStickEvent.MoveEnd, m_touchPos);							
					}
				}		
			}
		}
#endif		
	}
	
	// Calculates the on-screen rectangle to render the thumbstick texture
	private Rect GetStickRect(Rect zone, Vector2 offset)
	{
		Vector2 centre = 
			GetZoneCentre(zone);

		float halfW = ((float)m_StickTexture.width  / 2.0f) * m_StickScale;
		float halfH = ((float)m_StickTexture.height / 2.0f) * m_StickScale;

		Rect result = new Rect
			(centre.x - halfW + offset.x, 
			 centre.y - halfH + offset.y,
			 halfW    * 2.0f  ,
			 halfH    * 2.0f  );		

		return result;
	}	
	
	// Calculate the mid-point of our active zone
	private Vector2 GetZoneCentre(Rect zone)
	{
		Vector2 centre = 
			new Vector2(
				zone.xMin + ( (zone.xMax - zone.xMin) / 2.0f),
				zone.yMin + ( (zone.yMax - zone.yMin) / 2.0f));

		return centre;
	}

	// Helper function for returning a resolution-independant screen rect
	private static Rect screenRect
		(float tx,
		 float ty,
	     float tw,
		 float th) 
    {
        float x1 = tx * Screen.width;
        float y1 = ty * Screen.height;     
        float sw = tw * Screen.width;
        float sh = th * Screen.height;
        return new Rect(x1,y1,sw,sh);
    }
	
	// Used to notify subscribers of notifications
	private void RaiseEvent (enThumbStickEvent tsE, Vector2 pos)
	{
		if (evThumbStickEvent != null)
			evThumbStickEvent(tsE, pos);
	}
	
	// Keep an angle in the 0-360 range
	public static float WrapAngle
        (float angle)
    {
        while (angle < 0)   angle += 360;
        while (angle > 360) angle -= 360;
        return angle;
    }
  
	// Determine the angle between a source and target vector (2D)
    public static float AngleToTarget
        (Vector2 origin, Vector2 target)
    {
        float x = target.x - origin.x;
        float y = target.y - origin.y;
        float a = (float)Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        return WrapAngle(a);		
    }       
	
	// Determine the angle from a direction vector
    public static float AngleFromVector
        (Vector2 inVector)
    {
        if (inVector == Vector2.zero) return 0.0f;
        inVector.Normalize();
        return AngleToTarget(Vector2.zero, inVector);
    }
	
	// Determines the angle between two existing angles
    public static float GetAngle
        (float a1, float a2)
    { return (Mathf.Abs(a1 - a2)) % 360; }
	
	// Determine the shortest angle between two existing angles
    public static float GetShortAngle
        (float a1, float a2)
    {
        float angle = GetAngle(a1, a2);

        if (angle > 180)
            angle = 360 - angle;

        return angle;
    }	
}
