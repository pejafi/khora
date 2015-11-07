using UnityEngine;
using System.Collections;

public class ObjBase : MonoBehaviour 
{
	public 	bool	m_decelerate	= true;
	public	float	m_decelerateVal	= 1.0f;
	
	protected virtual void Update()
	{
		if(m_decelerate)
			if(GetComponent<Rigidbody>().velocity.magnitude < 5.0f)
				GetComponent<Rigidbody>().velocity = Vector3.Slerp(GetComponent<Rigidbody>().velocity, new Vector3(0.0f, 0.0f, 0.0f), 1.5f * Time.deltaTime);
	}
}
