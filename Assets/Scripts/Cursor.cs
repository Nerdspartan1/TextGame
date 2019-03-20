using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
	Camera cam;
	Plane fightPlane;

	float bulletDelay = 0;
	float disp = 0;
	float recoil = 0;
	float rof = 1;
	bool auto = false;
	int dmg = 1;

	float timeBeforeNextShot = 0;

	public void Start()
	{
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		fightPlane.SetNormalAndPosition(Vector3.back, Vector3.zero);
	}

	public void LoadStats(Unit unit)
	{
		if(unit.weapon == null)
		{
			Debug.Log("[LoadStats] Error: no equipped weapon");
		}
		else
		{
			disp = unit.weapon.dispersion;
			recoil = unit.weapon.recoil;
			dmg = unit.weapon.maxDmg;
			auto = unit.weapon.auto;
			rof = unit.weapon.rof;
		}
	}
    
    void Update()
    {
		Ray cursorRay = cam.ScreenPointToRay(Input.mousePosition);
		float distance;
		if(fightPlane.Raycast(cursorRay,out distance))
		{
			transform.position = cursorRay.GetPoint(distance);
			transform.Translate(Vector3.back);
		}
		/*
		transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector3(transform.position.x, transform.position.y, -1);
		*/

		if (Time.time > timeBeforeNextShot && Input.GetMouseButtonDown(0))
		{
			Shoot();
		}

    }

	void Shoot()
	{
		Debug.Log("Bang !");
		timeBeforeNextShot = Time.time + 1f / rof;
	}
}
