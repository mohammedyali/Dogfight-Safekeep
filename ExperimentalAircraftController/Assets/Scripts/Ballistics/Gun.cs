using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public GameObject Bullet;
    public GameObject ShootPoint;
    public GameObject MuzzelFlash;
    public AudioSource GunSound;
    public float BulletVelocity;
    public float GravityForce;
    public float WeaponSpread = 0.01f;
    public int Ammo = 391;
    private bool loaded;
    public float ReloadTime = 1;
    public float OffsetMin = 0.0001f, OffsetMax = 0.005f;
    public float _time;
    public bool CanShoot;
    [HideInInspector]
    public bool IsShootingForEffects;
    GameObject S;

    // Start is called before the first frame update
    void Start()
    {
        if(GunSound != null)
            S = Instantiate(GunSound.gameObject, transform);

        CanShoot = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float Offset = Random.Range(OffsetMin, OffsetMax);

        _time += Time.deltaTime;
        if (_time < 0)
        {
            _time = 0;
        }
        if (_time > ReloadTime + Offset)
        {
            CanShoot = true;
            _time = ReloadTime;
        }
        else if (_time < ReloadTime)
        {
            CanShoot = false;
        }
        if (IsShootingForEffects && Ammo > 0)
        {
            if (GunSound != null)
            {
                S.gameObject.SetActive(true);
            }
        }
        else if(!IsShootingForEffects || Ammo < 0)
        {
            if (GunSound != null)
            {
                S.gameObject.SetActive(false);
            }
        }
    }
    public void Shoot()
    {
        float x = Random.Range(-WeaponSpread, WeaponSpread) / 57.2957795f;
        float y = Random.Range(-WeaponSpread, WeaponSpread) / 57.2957795f;
        float z = Random.Range(-WeaponSpread, WeaponSpread) / 57.2957795f;
        if (Ammo > 0 && CanShoot && !PlayerManager.Pause)
        {
            _time = 0f;
            GameObject bullet = Instantiate(Bullet, ShootPoint.transform.position, ShootPoint.transform.rotation);
            if(MuzzelFlash != null)
                Instantiate(MuzzelFlash, ShootPoint.transform.position, ShootPoint.transform.rotation);
            bullet.transform.rotation = new Quaternion(bullet.transform.rotation.x + x, bullet.transform.rotation.y + y, bullet.transform.rotation.z + z, bullet.transform.rotation.w);
            Shell shellScript = bullet.GetComponent<Shell>();
            Ammo -= 1;
            shellScript.Initialize(bullet.transform.transform, BulletVelocity, GravityForce);
        }
    }
}
