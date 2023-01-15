using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
	public Country targetCountry;
	public Vector3 target;
	public Transform GunBreech;
    public Vector3 GunBreechRotOffset;
    public float cannonRange;
    public float bulletSpeed;
    //public float MaxAngle, MinAngle;

    public Transform Turret;
    public Vector3 TurretRotOffset;
	public bool UseZForTurret = false;
    public Gun[] guns;

    void FixedUpdate()
    {
		Vector3 _target = new Vector3();

		if(GetClosestAircraft() != Vector3.zero)
			_target = GetClosestAircraft();
		else
			_target = transform.forward * 100;

		Turret.LookAt(_target);
		if(!UseZForTurret)
			Turret.localEulerAngles = new Vector3(TurretRotOffset.x, Turret.localEulerAngles.y, TurretRotOffset.z);
		if(UseZForTurret)
			Turret.localEulerAngles = new Vector3(TurretRotOffset.x, TurretRotOffset.y, Turret.localEulerAngles.z);

		GunBreech.LookAt(_target);
        GunBreech.localEulerAngles = new Vector3(GunBreech.localEulerAngles.x, GunBreechRotOffset.y, GunBreechRotOffset.z);
        
		foreach (Gun item in guns)
        {
			if(GetClosestAircraft() != Vector3.zero)
				item.Shoot();
        }
    }

	DamageModel[] AircraftInScene;

	float __time;
	private void LateUpdate()
	{
		__time += Time.deltaTime;
		if (__time > 2f)
		{
			__time = 0f;
			AircraftInScene = FindObjectsOfType<DamageModel>();

		}

	}

	public Vector3 GetClosestAircraft()
	{
		float check = cannonRange;
		Vector3 closestObject = new Vector3();
		if (AircraftInScene != null)
		{
			for (int i = 0; i < AircraftInScene.Length; i++)  //list of gameObjects to search through
			{
				float dist = Vector3.Distance(AircraftInScene[i].transform.position, transform.position);
				if (dist < check && AircraftInScene[i].country == targetCountry)
				{
					check = dist;
					closestObject = AircraftInScene[i].transform.position;
				}
			}
		}
		return closestObject;
		//if there are no aircraft within range then it will return vector3.zero
	}

}
