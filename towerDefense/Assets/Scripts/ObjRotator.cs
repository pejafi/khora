using UnityEngine;
using System.Collections;

public class ObjRotator : MonoBehaviour 
{
	public Vector3 m_rotationAxis = new Vector3(0.0f, 1.0f, 0.0f);
	public float m_speed = 25.0f;
	
	void Update()
	{
		transform.Rotate(m_rotationAxis, m_speed * Time.deltaTime);
	}
}
