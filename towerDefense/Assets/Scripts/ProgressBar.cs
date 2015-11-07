using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour 
{
    public Vector2 pos = new Vector2(20, 40);
    public Vector2 size = new Vector2(60, 20);
    public Texture2D emptyTex;
    public Texture2D fullTex;
 
	public  float			m_barFullValue		= 1.0f;
	private float			m_barValue			= 1.0f;
	private	float			m_barFullWidth		= 100.0f;
	private	float			m_barFullHeight		= 1.0f;
	private float			m_ratio				= 1.0f;
	
	public void OnSetup(float a_fullVal)
	{
		m_barFullValue = a_fullVal;
		m_ratio = 1.0f / m_barFullValue;
		
		m_barValue = m_barFullValue * m_ratio;			
		
		SetProgress(a_fullVal);
	}
	
	public void SetProgress(float val)
	{
		m_barValue = val * m_ratio;	
	}
	
    void OnGUI() 
	{
       GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
         GUI.Box(new Rect(0,0, size.x, size.y), emptyTex);
 
         GUI.BeginGroup(new Rect(0,0, size.x * m_barValue, size.y));
          GUI.Box(new Rect(0,0, size.x, size.y), fullTex);
         GUI.EndGroup();
       GUI.EndGroup();

		GUI.color = Color.black;
		GUI.Label(new Rect(pos.x + size.x + 10, pos.y, 100, 40), "HEALTH");
    }
}
