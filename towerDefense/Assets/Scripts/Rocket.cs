using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour 
{
	public float 		m_traverseSpeed;
	private bool		m_active = false;
	
	void Awake()
	{
		GetComponent<Renderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_active)
		{
			transform.position += transform.forward.normalized * ( m_traverseSpeed * Time.deltaTime );
		}
	}
	
	public void Activate(Vector3 a_startPos, Vector3 a_direction)
	{
		if(m_active)
			return;
		
		m_active = true;
		GetComponent<Renderer>().enabled = true;
		GetComponent<Collider>().enabled = true;
		transform.position = a_startPos;
		transform.forward = a_direction;
		transform.LookAt(a_direction);
	}
	
	void OnTriggerEnter(Collider a_obj)
	{
		if(a_obj.gameObject.name == "Player")
		{
			// damage player
		}
		else if(a_obj.gameObject.name == "Shield")
		{
			// damage shield
		}
		
		Explode();
	}
	
	public void Explode()
	{
		Reset();
	}
	
	void Reset()
	{
		m_active = false;
		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
	}
}

