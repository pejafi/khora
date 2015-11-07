using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
	public WeaponSystem weapon;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			if(weapon != null)
				weapon.Fire();
		}
	}
}
