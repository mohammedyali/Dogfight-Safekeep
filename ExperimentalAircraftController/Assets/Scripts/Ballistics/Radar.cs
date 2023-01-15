using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [HideInInspector]
    public float MaxRange = 500f;
    public Rigidbody rigidbody;
    public Transform FocusPoint;
    public int CurrentMissle = 0;
    public Missle[] missles;
    public float lockRange = 2000f;
    public float lockAngle = 50f;
    [Header("Is the Player")]
    public bool IsPlayer;
    public Transform LockImage;
    public Transform LeadImage;
    [HideInInspector]
    public DamageModel _Target;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = transform.root.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        DamageModel target = new DamageModel();

        if (GetClosestAircraft() != null)
            target = GetClosestAircraft();

        if (target != null)
        {
            //default neutral position is forward
            Vector3 targetDir = Vector3.forward;
            if (target != null)
            {
                var error = target.PlaneRoot.collider.transform.position - rigidbody.position;
                var errorDir = Quaternion.Inverse(rigidbody.rotation) * error.normalized; //transform into local space
                if (error.magnitude <= lockRange && Vector3.Angle(Vector3.forward, errorDir) < lockAngle)
                {
                    if(!IsPlayer)
                        Debug.Log("Radar lock");

                    targetDir = errorDir;
                    if (IsPlayer)
                    {
                        Vector3 Lead = Missle.FirstOrderIntercept(transform.position, rigidbody.velocity, 1000, target.PlaneRoot.collider.transform.position, target.gameObject.GetComponent<Aircraft>().Velocity);
                        LockImage.gameObject.SetActive(true);
                        LockImage.position = Camera.main.WorldToScreenPoint(target.PlaneRoot.collider.transform.position);
                        LeadImage.position = Camera.main.WorldToScreenPoint(Lead);
                    }
                    _Target = target;
                }
                else
                {
                    _Target = null;
                    if (IsPlayer)
                        LockImage.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if(IsPlayer)
                LockImage.gameObject.SetActive(false);
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
    public DamageModel GetClosestAircraft()
    {
        float check = MaxRange;
        DamageModel closestObject = new DamageModel();
        if (AircraftInScene != null)
        {
            for (int i = 0; i < AircraftInScene.Length; i++)  //list of gameObjects to search through
            {
                float dist = Vector3.Distance(AircraftInScene[i].transform.position, FocusPoint.position);
                if (dist < check)
                {
                    check = dist;
                    closestObject = AircraftInScene[i];
                }
            }
        }
        return closestObject;
    }

    public void ActivateMissle(DamageModel _target)
    {
        if(CurrentMissle < missles.Length)
        {
            missles[CurrentMissle].Target = _target;
            missles[CurrentMissle].FireMissle();
            CurrentMissle++;
        }
    }

    public void Fire()
    {

        Debug.Log("Fired Missle");
        if (_Target != null)
            ActivateMissle(_Target);
    }

}
