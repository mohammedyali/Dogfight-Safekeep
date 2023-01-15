using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum Country
{
    Japan,
    USA,
    UK,
    Germany,
    SovietUnion
}

[Serializable]
public struct DamageComponent
{
    public Airfoil airfoil;
    public float MaxCollisionSpeed;
    public float DisconnectedObjectWeight;
    public MeshCollider collider;
    public Airfoil[] ChildAirfoils;
    public ParticleSystem FlameTrailOnOGParent;
    public float LifeTime;
}

[Serializable]
public struct ExternalDamageComponent
{
    public MeshCollider collider;
    public GameObject WorkingObject;
    public GameObject NotWorkingObjec;
}


public class DamageModel : MonoBehaviour
{
    public Country country;

    bool IsPlayer;

    public Aircraft aircraft;
    public DamageComponent[] damageComponents;
    public DamageComponent PlaneRoot;
    public float RootHealth = 1;
    public float MaxRootHealth = 100;
    public ExternalDamageComponent prop;
    public GameObject PlaneDestroyedExplosion;

    [Header("Audio")]
    public AudioSource EngineSource;
    public float PitchOffset = 0.2f;

    private void Start()
    {
        IsPlayer = GetComponent<InputManager>() != null;
    }

    void OnCollisionEnter(Collision CollisionInfo)
    {
        DamageModelDetachPartUsingCollision(CollisionInfo);
    }

    private void Update()
    {
        if(RootHealth < 0.5f && PlaneRoot.FlameTrailOnOGParent != null)
        {
            PlaneRoot.FlameTrailOnOGParent.gameObject.SetActive(true);
        }
        aircraft.PowMul = RootHealth;
        if (RootHealth < 0)
        {
            RootHealth = 0f;
            DestroyPlaneRoot(PlaneRoot.collider);
        }
        EngineSource.pitch = aircraft.Throttel * (1 + PitchOffset);
    }

    void DamageModelDetachPartUsingCollision(Collision CollisionInfo)
    {
        foreach (DamageComponent damageComponent in damageComponents)
        {
            Collider myCollider = CollisionInfo.GetContact(0).thisCollider;
            if (myCollider.gameObject.name == damageComponent.collider.name)
            {
                Debug.Log($"{myCollider.gameObject.name} has hit something.");
                if (aircraft.Velocity.magnitude * 5.76f > damageComponent.MaxCollisionSpeed)
                {
                    if (damageComponent.airfoil != null)
                    {
                        Debug.Log($"{damageComponent.airfoil.gameObject.name} is Destroyed.");
                        damageComponent.collider.transform.parent = damageComponent.airfoil.transform;
                        Rigidbody rb = damageComponent.airfoil.gameObject.AddComponent<Rigidbody>();
                        rb.velocity = aircraft.Velocity;
                        aircraft.rigidbody.mass -= damageComponent.DisconnectedObjectWeight;
                        rb.mass = damageComponent.DisconnectedObjectWeight;
                        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                        damageComponent.airfoil.transform.parent = null;
                        damageComponent.airfoil.Effectiveness = 0f;

                        DetachedPart detachedPart = damageComponent.airfoil.gameObject.AddComponent<DetachedPart>();
                        if (damageComponent.FlameTrailOnOGParent != null)
                            detachedPart.Flame = Instantiate(damageComponent.FlameTrailOnOGParent, damageComponent.FlameTrailOnOGParent.transform.position, damageComponent.FlameTrailOnOGParent.transform.rotation);
                        detachedPart.LifeTime = damageComponent.LifeTime;


                    }
                    else
                    {
                        Rigidbody rb = damageComponent.collider.gameObject.AddComponent<Rigidbody>();
                        rb.velocity = aircraft.Velocity;
                        aircraft.rigidbody.mass -= damageComponent.DisconnectedObjectWeight;
                        rb.mass = damageComponent.DisconnectedObjectWeight;
                        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                        damageComponent.collider.transform.parent = null;
                        GameObject _FlameTrailOnOGParent = Instantiate(damageComponent.FlameTrailOnOGParent.gameObject, damageComponent.collider.transform.position, damageComponent.collider.transform.rotation, damageComponent.collider.transform);
                        ParticleSystem flameTrailOnOGParent = _FlameTrailOnOGParent.GetComponent<ParticleSystem>();
                        foreach (Airfoil foil in damageComponent.ChildAirfoils)
                        {
                            foil.Effectiveness = 0;
                        }

                        DetachedPart detachedPart = damageComponent.collider.gameObject.AddComponent<DetachedPart>();
                        if (damageComponent.FlameTrailOnOGParent != null)
                            detachedPart.Flame = Instantiate(damageComponent.FlameTrailOnOGParent, damageComponent.FlameTrailOnOGParent.transform.position, damageComponent.FlameTrailOnOGParent.transform.rotation);
                        detachedPart.LifeTime = damageComponent.LifeTime;
                        detachedPart.Flame.transform.localScale = new Vector3(damageComponent.collider.transform.localScale.x / detachedPart.Flame.transform.localScale.x, damageComponent.collider.transform.localScale.y / detachedPart.Flame.transform.localScale.y, damageComponent.collider.transform.localScale.z / detachedPart.Flame.transform.localScale.z);
                    }
                    if (damageComponent.FlameTrailOnOGParent != null)
                        damageComponent.FlameTrailOnOGParent.gameObject.SetActive(true);
                }
            }
        }
        if (prop.collider != null)
        {
            Collider myCollider = CollisionInfo.GetContact(0).thisCollider;
            if (myCollider.gameObject.name == prop.collider.name)
            {
                Destroy(prop.WorkingObject);
                prop.NotWorkingObjec.SetActive(true);
                prop.collider.enabled = false;
                if (GetComponent<InputManager>().WorkingEngine == true)
                    GetComponent<InputManager>().WorkingEngine = false;
                else if (GetComponent<EnemyManager>().WorkingEngine == true)
                    GetComponent<EnemyManager>().WorkingEngine = false;
            }
        }
        if (PlaneRoot.collider != null)
        {
            DestroyPlaneRoot(CollisionInfo.GetContact(0).thisCollider);
        }
    }
    public void DamageModelDetachPartUsingRaycast(RaycastHit CollisionInfo)
    {
        foreach (DamageComponent damageComponent in damageComponents)
        {
            Collider myCollider = CollisionInfo.collider;
            if (myCollider.gameObject.name == damageComponent.collider.name)
            {
                Debug.Log($"{myCollider.gameObject.name} has been hit by something.");
                if (damageComponent.airfoil != null)
                {
                    Debug.Log($"{damageComponent.airfoil.gameObject.name} is Destroyed.");
                    damageComponent.collider.transform.parent = damageComponent.airfoil.transform;
                    Rigidbody rb = damageComponent.airfoil.gameObject.AddComponent<Rigidbody>();
                    rb.velocity = aircraft.Velocity;
                    aircraft.rigidbody.mass -= damageComponent.DisconnectedObjectWeight;
                    rb.mass = damageComponent.DisconnectedObjectWeight;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    damageComponent.airfoil.transform.parent = null;
                    damageComponent.airfoil.Effectiveness = 0f;

                    DetachedPart detachedPart = damageComponent.airfoil.gameObject.AddComponent<DetachedPart>();
                    if (damageComponent.FlameTrailOnOGParent != null)
                        detachedPart.Flame = Instantiate(damageComponent.FlameTrailOnOGParent, damageComponent.FlameTrailOnOGParent.transform.position, damageComponent.FlameTrailOnOGParent.transform.rotation);
                    detachedPart.LifeTime = damageComponent.LifeTime;


                }
                else
                {
                    Rigidbody rb = damageComponent.collider.gameObject.AddComponent<Rigidbody>();
                    rb.velocity = aircraft.Velocity;
                    aircraft.rigidbody.mass -= damageComponent.DisconnectedObjectWeight;
                    rb.mass = damageComponent.DisconnectedObjectWeight;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    damageComponent.collider.transform.parent = null;
                    GameObject _FlameTrailOnOGParent = Instantiate(damageComponent.FlameTrailOnOGParent.gameObject, damageComponent.collider.transform.position, damageComponent.collider.transform.rotation, damageComponent.collider.transform);
                    ParticleSystem flameTrailOnOGParent = _FlameTrailOnOGParent.GetComponent<ParticleSystem>();
                    foreach (Airfoil foil in damageComponent.ChildAirfoils)
                    {
                        foil.Effectiveness = 0;
                    }

                    DetachedPart detachedPart = damageComponent.collider.gameObject.AddComponent<DetachedPart>();
                    if (damageComponent.FlameTrailOnOGParent != null)
                        detachedPart.Flame = Instantiate(damageComponent.FlameTrailOnOGParent, damageComponent.FlameTrailOnOGParent.transform.position, damageComponent.FlameTrailOnOGParent.transform.rotation);
                    detachedPart.LifeTime = damageComponent.LifeTime;
                    detachedPart.Flame.transform.localScale = new Vector3(damageComponent.collider.transform.localScale.x / detachedPart.Flame.transform.localScale.x, damageComponent.collider.transform.localScale.y / detachedPart.Flame.transform.localScale.y, damageComponent.collider.transform.localScale.z / detachedPart.Flame.transform.localScale.z);
                }
                if (damageComponent.FlameTrailOnOGParent != null)
                    damageComponent.FlameTrailOnOGParent.gameObject.SetActive(true);
            }
        }
    }

    public void DamageModelDetachPartUsingSphereCast(Collider CollisionInfo)
    {
        foreach (DamageComponent damageComponent in damageComponents)
        {
            Collider myCollider = CollisionInfo;
            if (myCollider.gameObject.name == damageComponent.collider.name)
            {
                Debug.Log($"{myCollider.gameObject.name} has been hit by something.");
                if (damageComponent.airfoil != null)
                {
                    Debug.Log($"{damageComponent.airfoil.gameObject.name} is Destroyed.");
                    damageComponent.collider.transform.parent = damageComponent.airfoil.transform;
                    Rigidbody rb = damageComponent.airfoil.gameObject.AddComponent<Rigidbody>();
                    rb.velocity = aircraft.Velocity;
                    aircraft.rigidbody.mass -= damageComponent.DisconnectedObjectWeight;
                    rb.mass = damageComponent.DisconnectedObjectWeight;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    damageComponent.airfoil.transform.parent = null;
                    damageComponent.airfoil.Effectiveness = 0f;

                    DetachedPart detachedPart = damageComponent.airfoil.gameObject.AddComponent<DetachedPart>();
                    if (damageComponent.FlameTrailOnOGParent != null)
                        detachedPart.Flame = Instantiate(damageComponent.FlameTrailOnOGParent, damageComponent.FlameTrailOnOGParent.transform.position, damageComponent.FlameTrailOnOGParent.transform.rotation);
                    detachedPart.LifeTime = damageComponent.LifeTime;


                }
                else
                {
                    Rigidbody rb = damageComponent.collider.gameObject.AddComponent<Rigidbody>();
                    rb.velocity = aircraft.Velocity;
                    aircraft.rigidbody.mass -= damageComponent.DisconnectedObjectWeight;
                    rb.mass = damageComponent.DisconnectedObjectWeight;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    damageComponent.collider.transform.parent = null;
                    GameObject _FlameTrailOnOGParent = Instantiate(damageComponent.FlameTrailOnOGParent.gameObject, damageComponent.collider.transform.position, damageComponent.collider.transform.rotation, damageComponent.collider.transform);
                    ParticleSystem flameTrailOnOGParent = _FlameTrailOnOGParent.GetComponent<ParticleSystem>();
                    foreach (Airfoil foil in damageComponent.ChildAirfoils)
                    {
                        foil.Effectiveness = 0;
                    }

                    DetachedPart detachedPart = damageComponent.collider.gameObject.AddComponent<DetachedPart>();
                    if (damageComponent.FlameTrailOnOGParent != null)
                        detachedPart.Flame = Instantiate(damageComponent.FlameTrailOnOGParent, damageComponent.FlameTrailOnOGParent.transform.position, damageComponent.FlameTrailOnOGParent.transform.rotation);
                    detachedPart.LifeTime = damageComponent.LifeTime;
                    detachedPart.Flame.transform.localScale = new Vector3(damageComponent.collider.transform.localScale.x / detachedPart.Flame.transform.localScale.x, damageComponent.collider.transform.localScale.y / detachedPart.Flame.transform.localScale.y, damageComponent.collider.transform.localScale.z / detachedPart.Flame.transform.localScale.z);
                }
                if (damageComponent.FlameTrailOnOGParent != null)
                    damageComponent.FlameTrailOnOGParent.gameObject.SetActive(true);
            }
        }
    }

    public void DestroyPlaneRoot(Collider Col)
    {
        Collider myCollider = Col;
        if (myCollider.gameObject.name == PlaneRoot.collider.name)
        {
            Instantiate(PlaneDestroyedExplosion, transform.position, transform.rotation);

            if (IsPlayer)
            {
                //kill cam
                GameObject C = Camera.main.gameObject;
                C.transform.parent.GetComponent<CamController>().enabled = false;
                Vector3 CamPos = new Vector3(100, 100, 100);
                C.transform.position = C.transform.parent.position + CamPos;
                C.transform.LookAt(transform.position);
            }

            //destroy player
            Destroy(aircraft.transform.gameObject);
        }
    }

}
