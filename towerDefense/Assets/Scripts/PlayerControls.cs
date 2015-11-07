using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour 
{
	public 	GameObject 	m_build1		= null;
	public 	GameObject 	m_build2		= null;
	public  Camera		m_camera		=	null;
	public 	LayerMask 	m_maskPlaceBuilding;
	public  float       m_buildingGroundPos = 1.0f;
	public Transform GearCamera;
	private GameObject 	m_spawnedBuilding 	= null;
	public 	LayerMask 	m_maskSelectBuilding;
	private GameObject  m_buildingSelected  = null;
	
	private SpawnController	m_spawnController			= null;
	
	void Awake()
	{
		m_spawnController = GameObject.FindWithTag("SpawnController").GetComponent<SpawnController>();
		
		if(!m_camera)
			m_camera = Camera.main;
	}
	
	void Update()
	{
		if(m_spawnedBuilding)
		{
			RaycastHit hit;
			if (Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out hit, Mathf.Infinity, m_maskPlaceBuilding))
			{
				string hitTag = hit.transform.gameObject.tag;
				
				if( hitTag == "Ground" )
				{
					m_spawnedBuilding.GetComponent<Building>().Reposition(new Vector3(hit.point.x, m_buildingGroundPos, hit.point.z));
				}
			}
			
			if(Input.GetButtonDown("Fire1"))
			{
				if(m_spawnController)
				{
					if(m_spawnController.CanPurchase(m_spawnedBuilding.GetComponent<Building>().m_cost))
					{
						bool didPlace = m_spawnedBuilding.GetComponent<Building>().PlaceBuilding();
										
						if(didPlace)
						{
							m_spawnController.MakePurchase(m_spawnedBuilding.GetComponent<Building>().m_cost);
							m_spawnedBuilding = null;
						}
					}
				}
			}
		}
		else
		{
			if(!m_buildingSelected)
			{
				if(Input.GetButtonDown("Fire1"))
				{
					RaycastHit hit;
					if (Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out hit, Mathf.Infinity, m_maskSelectBuilding))
					{
						string hitTag = hit.transform.gameObject.tag;
					
						if( hitTag == "Building" )
						{
							GearCamera.position = hit.transform.position + Vector3.up*2;
							WeaponSystem weapon = hit.transform.gameObject.GetComponentInChildren<WeaponSystem>();
							weapon.m_autoAim = false;
							weapon.transform.SetParent(GearCamera.GetComponentInChildren<Shoot>().transform);
							weapon.transform.position = GearCamera.position;
							weapon.transform.eulerAngles = GearCamera.eulerAngles; 
							GearCamera.GetComponentInChildren<Shoot>().weapon = weapon;
							//weapon.transform.SetParent(GearCamera.GetComponentInChildren<Shoot>().transform);
							//if(!hit.transform.gameObject.GetComponent<Building>().HasUpgraded())
							//	m_buildingSelected = hit.transform.gameObject;
						}
					}
				}
			}
			else
			{
				if(Input.GetButtonDown("Fire1"))
				{
					DeselectBuilding();
				}
			}
		}
	}
	
	void DeselectBuilding()
	{
		m_buildingSelected = null;
	}

	public void SpawnBuilding(){
		m_spawnedBuilding = Instantiate (m_build1, m_build1.transform.position, m_build1.transform.rotation) as GameObject;

	}

	public void UpgradeBuilding(){
		if (!m_buildingSelected)
			return;
		if (m_buildingSelected.GetComponent<Building> ().HasUpgraded ()) {
			// ignore
		} else {
			if (m_spawnController.CanPurchase (m_buildingSelected.GetComponent<Building> ().m_upgradeCost)) {
				//if (GUI.Button (new Rect (upgradeGUIStartPos.x, upgradeGUIStartPos.y, buttonSize.x, buttonSize.y), "BUY - " + m_buildingSelected.GetComponent<Building> ().m_upgradeCost + "\nPower Upgrade")) {
					m_buildingSelected.GetComponent<Building> ().PurchaseUpgrade ();
					m_spawnController.MakePurchase (m_buildingSelected.GetComponent<Building> ().m_upgradeCost);
					DeselectBuilding ();
			/*	}
			
				if (GUI.Button (new Rect (upgradeGUIStartPos.x + buttonUpgradeSize.x + buttonUpgradeSpace.x, upgradeGUIStartPos.y, buttonSize.x, buttonSize.y), "Cancel Purchase")) {
					DeselectBuilding ();
				}
			} else {
				GUI.Button (new Rect (upgradeGUIStartPos.x, upgradeGUIStartPos.y, buttonSize.x, buttonSize.y), "OUT OF CASH\nPower Upgrade");
			
				if (GUI.Button (new Rect (upgradeGUIStartPos.x + buttonUpgradeSize.x + buttonUpgradeSpace.x, upgradeGUIStartPos.y, buttonSize.x, buttonSize.y), "Cancel Purchase")) {
					DeselectBuilding ();
				}				
			}	*/
			}
		}
	}
	/*void OnGUI()
	{
		if(!m_spawnController)
			return;
		
		if(m_spawnController.HasWon())
		{
			if(GUI.Button(new Rect((Screen.width * 0.5f) - 50, Screen.height - 100, 100, 80), "You WON!\n\nRetry?"))
				Application.LoadLevel(Application.loadedLevelName);			
			
			return;
		}
		else if(m_spawnController.IsGameOver())
		{
			if(GUI.Button(new Rect((Screen.width * 0.5f) - 50, Screen.height - 100, 100, 80), "You're DEAD!\n\nRetry?"))
				Application.LoadLevel(Application.loadedLevelName);			
			
			return;
		}
		
		Vector2 buttonSize = new Vector2(120, 40);
		Vector2 buttonSpace = new Vector2(20, 0);
		Rect startPosTop = new Rect(90, 30, buttonSize.x, buttonSize.y);
		Rect startPosBottom = new Rect(90, Screen.height - buttonSize.y - 10.0f, buttonSize.x, buttonSize.y);
		int buttonInc = 0;
		
		GUI.Box(new Rect(80, -20, 200, 50), "");
		
		GUI.Label(new Rect(startPosTop.x, startPosTop.y - 25.0f, 100.0f, 30.0f), "WAVE: " + m_spawnController.m_startWave);
		
		GUI.Label(new Rect(startPosTop.x + 80.0f, startPosTop.y - 25.0f, 100.0f, 30.0f), "CASH: " + m_spawnController.m_playerCash);
				
		if(m_spawnedBuilding)
			return;
		
		GUI.color = Color.white;
		
		if(m_spawnController.CanPurchase(m_build1.GetComponent<Building>().m_cost))
		{
			if(GUI.Button(startPosBottom, "BUY - " + m_build1.GetComponent<Building>().m_cost + "\nPistol Building"))
			{
				if(m_build1)
					m_spawnedBuilding = Instantiate(m_build1, m_build1.transform.position, m_build1.transform.rotation) as GameObject;
				
				DeselectBuilding();
			}
		}
		else
		{
			GUI.Button(startPosBottom, "OUT OF CASH\nPistol Building");
		}
		
		buttonInc++;
		if(m_spawnController.CanPurchase(m_build2.GetComponent<Building>().m_cost))
		{
			if(GUI.Button(new Rect(startPosBottom.x + ((buttonSize.x + buttonSpace.x) * buttonInc), startPosBottom.y, buttonSize.x, buttonSize.y), "BUY - " + m_build2.GetComponent<Building>().m_cost + "\nRocket Building"))
			{
				if(m_build2)
					m_spawnedBuilding = Instantiate(m_build2, m_build2.transform.position, m_build2.transform.rotation) as GameObject;
				
				DeselectBuilding();
			}
		}
		else
		{
			GUI.Button(new Rect(startPosBottom.x + ((buttonSize.x + buttonSpace.x) * buttonInc), startPosBottom.y, buttonSize.x, buttonSize.y), "OUT OF CASH\nRocket Building");
		}
		
		if(!m_buildingSelected)
			return;
		
		int numOfUpgradeBtns = 2;
		Vector2 buttonUpgradeSize = new Vector2(120, 40);
		Vector2 buttonUpgradeSpace = new Vector2(20, 20);
		Vector3 buildingScreenPos = m_camera.WorldToScreenPoint(m_buildingSelected.transform.position);
		Rect upgradeGUIStartPos = new Rect(buildingScreenPos.x - (buttonUpgradeSize.x * (numOfUpgradeBtns / 2)), Screen.height - (buildingScreenPos.y - buttonUpgradeSpace.y), buttonUpgradeSize.x, buttonUpgradeSize.y);
		int buttonUpgradeInc = 0;
		
		if(m_buildingSelected.GetComponent<Building>().HasUpgraded())
		{
			// ignore
		}
		else
		{
			if(m_spawnController.CanPurchase(m_buildingSelected.GetComponent<Building>().m_upgradeCost))
			{
				if(GUI.Button(new Rect(upgradeGUIStartPos.x, upgradeGUIStartPos.y, buttonSize.x, buttonSize.y), "BUY - " + m_buildingSelected.GetComponent<Building>().m_upgradeCost + "\nPower Upgrade"))
				{
					m_buildingSelected.GetComponent<Building>().PurchaseUpgrade();
					m_spawnController.MakePurchase(m_buildingSelected.GetComponent<Building>().m_upgradeCost);
					DeselectBuilding();
				}

				if(GUI.Button(new Rect(upgradeGUIStartPos.x + buttonUpgradeSize.x + buttonUpgradeSpace.x, upgradeGUIStartPos.y, buttonSize.x, buttonSize.y), "Cancel Purchase"))
				{
					DeselectBuilding();
				}
			}
			else
			{
				GUI.Button(new Rect(upgradeGUIStartPos.x, upgradeGUIStartPos.y, buttonSize.x, buttonSize.y), "OUT OF CASH\nPower Upgrade");
				
				if(GUI.Button(new Rect(upgradeGUIStartPos.x + buttonUpgradeSize.x + buttonUpgradeSpace.x, upgradeGUIStartPos.y, buttonSize.x, buttonSize.y), "Cancel Purchase"))
				{
					DeselectBuilding();
				}				
			}	
		}
	}*/
}
