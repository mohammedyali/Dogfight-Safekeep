using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public float Damage = 10f;
    public float speed;
    public float gravity;
    public float BulletLifeTime = 10f;
    public Vector3 StartPosition;
    public Vector3 startForward;
    public GameObject Impact;

    private bool IsInitialized = false;
    private float StartTime = -1;
    private float _time;

    public bool Explosive = false;
    public GameObject Explosion;
    public float ExplosionRange = 0;

    private void Awake()
    {
        
    }

    public void Initialize(Transform StartPoint, float speed, float gravity)
    {
        StartPosition = StartPoint.position;
        startForward = StartPoint.forward;
        this.speed = speed;
        this.gravity = gravity;
        IsInitialized = true;
    }

    private Vector3 FindPointOnParobla(float time)
    {
        Vector3 point = StartPosition + (startForward * speed * time);
        Vector3 GravityVector = Vector3.down * gravity * time * time;
        return point + GravityVector;
    }

    private bool CastRayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit)
    {
        return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude);
    }

    private void FixedUpdate()
    {
        _time += Time.deltaTime;
        if(_time >= BulletLifeTime)
        {

            Destroy(transform.gameObject);
        }
        if (!IsInitialized) return;
        if (StartTime < 0) StartTime = Time.time;

        RaycastHit _hit;
        float currentTime = Time.time - StartTime;
        float nextTime = currentTime + Time.fixedDeltaTime;

        Vector3 currentPosition = FindPointOnParobla(currentTime);
        Vector3 nextPoint = FindPointOnParobla(nextTime);
        transform.position = currentPosition;

        if (CastRayBetweenPoints(currentPosition, nextPoint, out _hit))
        {
            if (Impact != null)
            {
                GameObject impactEffect = Instantiate(Impact, _hit.point, Impact.transform.rotation, _hit.collider.transform);
            }
            if (_hit.collider.gameObject.layer == 6)
            {
                DamageModel damageModel = _hit.collider.gameObject.transform.root.gameObject.GetComponent<DamageModel>();
                if (damageModel != null)
                {
                    foreach (DamageComponent damageComponent in damageModel.damageComponents)
                    {
                        if (_hit.collider == damageComponent.collider && !Explosive)
                        {
                            if(damageComponent.airfoil != null)
                            {
                                if (damageComponent.airfoil.Effectiveness <= 0)
                                {
                                    damageModel.DamageModelDetachPartUsingRaycast(_hit);
                                    Debug.Log($"Destroyed '{damageComponent.airfoil.gameObject.name}'");
                                    Destroy(transform.gameObject);
                                }
                                else
                                {
                                    damageComponent.airfoil.Effectiveness -= Damage / damageComponent.airfoil.MaxHealth;
                                    Debug.Log($"Hit '{damageComponent.airfoil.gameObject.name}'");
                                    Destroy(transform.gameObject);
                                }
                            }
                        }
                        if (Explosive)
                        {
                            damageModel.RootHealth -= (Damage / Vector3.Distance(transform.position, damageModel.PlaneRoot.collider.ClosestPoint(_hit.point))) / damageModel.MaxRootHealth;
                            foreach (DamageComponent _damageComponent in damageModel.damageComponents)
                            {
                                _damageComponent.airfoil.Effectiveness -= (Damage / Vector3.Distance(transform.position, _damageComponent.collider.ClosestPoint(_hit.point))) / _damageComponent.airfoil.MaxHealth;
                                if(_damageComponent.airfoil.Effectiveness < 0)
                                {
                                    damageModel.DamageModelDetachPartUsingRaycast(_hit);
                                    Instantiate(Explosion, transform.position, transform.rotation);
                                    damageModel.DamageModelDetachPartUsingSphereCast(_damageComponent.collider);
                                    
                                    Debug.Log($"Destroyed '{_damageComponent.airfoil.gameObject.name}'");
                                    Destroy(transform.gameObject);
                                }
                                Debug.Log($"Explosively hit '{_damageComponent.airfoil.gameObject.name}'");
                                Instantiate(Explosion, transform.position, transform.rotation);
                                Destroy(transform.gameObject);
                            }
                        }
                    }
                }
                if(_hit.collider == damageModel.PlaneRoot.collider)
                {
                    damageModel.RootHealth -= Damage / damageModel.MaxRootHealth;
                    Debug.Log($"Hit '{damageModel.transform.gameObject.name}'s engine'");
                    Destroy(transform.gameObject);
                }
            }
            if(_hit.collider.gameObject.layer == 3)
            {   
                Destroy(transform.gameObject);
                Debug.Log("Missed Target");
            }
            if (Explosive)
                Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(transform.gameObject);
        }
    }
    void Update()
    {
        if (!IsInitialized || StartTime < 0) return;

        float currentTime = Time.time - StartTime;
        Vector3 currentPosition = FindPointOnParobla(currentTime);
        transform.position = currentPosition;
    }
}
