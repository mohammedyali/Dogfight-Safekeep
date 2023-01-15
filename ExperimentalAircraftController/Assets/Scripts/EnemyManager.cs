using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public float MaxGs = 10;
    public float CurrentGs;

    public Country targetCountry;

    [Range(0, 100)]
    public float Throttel;
    public Aircraft aircraftController;
    [Tooltip("When the target plane is more than the 'pitchUpThreshold' degrees below AI plane, the AI will always try to pitch up")]
    public float pitchUpThreshold = 15;
    [Tooltip("rollFactor is a small number like 0.01, to reduce oscillation when rolling.")]
    public float rollFactor = 0.01f;
    [Tooltip("If the AI plane is pointing within a small angle, 'fineSteeringAngle' of the target, then it will aim using the rudders for finer control")]
    public float fineSteeringAngle = 5;
    public Vector3 targetInput;
    Vector3 lastInput;
    public float steeringSpeed;
    public float MinSpeed = 100f;
    public LayerMask Ground;
    public bool HasTakenOff = false;
    public float takeOffHeight = 30;
    public bool WorkingEngine = true;

    [Header("Shoting Mecanics")]
    public Gun[] guns;
    public LandingGear[] gears;
    public float CheckDist = 1000;
    public float CheckConeBaseRad = 1000f;
    public float bulletSpeed = 1000;

    [Header("Path")]
    public Transform[] path;
    public int pathIndex;
    public bool IsAggressive;

    [Header("For Jets Only")]
    public Radar radar;
    bool MisselCoolDown;
    float CoolDowntime;
    public float FireDelay = 2;



    // Start is called before the first frame update
    void Start()
    {


        WorkingEngine = true;

        if (aircraftController.Speed == 0)
        {
            HasTakenOff = false;
        }
    }
    // Update is called once per frame
    Vector3 lastVel;
    void FixedUpdate()
    {
        Vector3 vel = aircraftController.Velocity;
        Vector3 accel = (vel - lastVel) / Time.fixedDeltaTime;

        CurrentGs = (accel / 9.8f).magnitude;

        lastVel = vel;


        if (!WorkingEngine)
            aircraftController.Throttel = 0f;
        foreach (LandingGear item in gears)
        {
            if (HasTakenOff && item.IsDown)
            {
                item.Gear();
            }
        }
        aircraftController.Flaps = 0f;
        if (transform.position.y > takeOffHeight)
        {
            HasTakenOff = true;
        }


        //follow the path;
        Vector3 _theTargetPosition = new Vector3();

        Vector3 ClosestAircraft = GetClosestAircraft();

        if (ClosestAircraft != Vector3.zero)
            IsAggressive = true;
        else
            IsAggressive = false;


        if (IsAggressive)
        {
            _theTargetPosition = ClosestAircraft;
        }
        else
        {
            //follosingPath
            _theTargetPosition = path[pathIndex].position;
            if (Vector3.Distance(transform.position, _theTargetPosition) < 100f)
            {
                pathIndex++;
            }
            if (pathIndex > path.Length - 1)
            {
                pathIndex = 0;
            }
        }


        Vector3 _targetPosition = _theTargetPosition;
        Vector3 error = _targetPosition - aircraftController.rigidbody.position;
        error = Quaternion.Inverse(aircraftController.rigidbody.rotation) * error;
        Vector3 errorDir = error.normalized;


        Vector3 pitchError = new Vector3(0, error.y, error.z).normalized;
        float pitch = Vector3.SignedAngle(Vector3.forward, pitchError, Vector3.right);
        if (-pitch < pitchUpThreshold) pitch += 360f;
        targetInput.x = pitch;
        Vector3 rollError = new Vector3(error.x, error.y, 0).normalized;
        if (Vector3.Angle(Vector3.forward, errorDir) < fineSteeringAngle)
        {
            targetInput.y = error.x;
        }
        else
        {
            float roll = Vector3.SignedAngle(Vector3.up, rollError, Vector3.forward);
            targetInput.z = roll * rollFactor;
        }
        Vector3 input = new Vector3();

        if (!HasTakenOff || Physics.Raycast(transform.position, transform.forward, 500f, Ground) || Physics.Raycast(transform.position, Vector3.down, 50f, Ground))
        {
            input = AvoidGround();
        }
        else if (aircraftController.Speed < MinSpeed)
        {
            RecoverSpeed();
        }
        else
        {
            targetInput.x = Mathf.Clamp(targetInput.x, -1, 1);
            targetInput.y = Mathf.Clamp(targetInput.y, -1, 1);
            targetInput.z = Mathf.Clamp(targetInput.z, -1, 1);
            input = Vector3.MoveTowards(lastInput, targetInput, steeringSpeed * Time.deltaTime);
            lastInput = input;
        }

        float Gmul = 0;
        if (CurrentGs < MaxGs)
        {
            Gmul = 1;
        }

        aircraftController.InputX = -input.z * Gmul;
        aircraftController.InputY = input.x * Gmul;
        aircraftController.InputYaw = input.y * Gmul;


        //Radar Missels
        if (radar != null)
        {
            if(radar._Target != null)
            {
                if (MisselCoolDown == false && radar._Target.transform.position == _targetPosition)
                {
                    Debug.Log("Radar lock");
                    radar.Fire();
                    MisselCoolDown = true;
                }
            }
        }

        //tip of cone
        Vector3 x = transform.position;
        Vector3 dir = transform.forward;
        Vector3 p = _targetPosition;
        float cone_dist = Vector3.Dot(p - x, dir);
        float cone_radius = (cone_dist / CheckDist) * CheckConeBaseRad;
        float orth_distance = Vector3.Magnitude((p - x) - cone_dist * dir);

        bool is_Target_inside_cone = orth_distance < cone_radius;
        if (is_Target_inside_cone && IsAggressive)
        {

            Debug.Log("target in range");
            foreach (Gun item in guns)
            {
                item.IsShootingForEffects = true;
                item.Shoot();
            }
        }
        else
        {
            foreach (Gun item in guns)
            {
                item.IsShootingForEffects = false;
            }
        }

    }

    DamageModel[] AircraftInScene;

    float __time;
    private void LateUpdate()
    {

        __time += Time.deltaTime;
        if(MisselCoolDown)
            CoolDowntime += Time.deltaTime;
        if(CoolDowntime > FireDelay)
        {
            MisselCoolDown = false;
            CoolDowntime = 0f;
        }

        if(__time > 2f)
        {
            __time = 0f;
            AircraftInScene = FindObjectsOfType<DamageModel>();

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, path[pathIndex].position);

        for (int i = 0; i < path.Length; i++)
        {
            if (i != path.Length - 1)
                Gizmos.DrawLine(path[i].position, path[i + 1].position);
            else
                Gizmos.DrawLine(path[i].position, path[0].position);
        }
    }

    private Vector3 GetClosestAircraft()
    {
        float check = CheckDist;
        Vector3 closestObject = new Vector3();
        if(AircraftInScene != null)
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

    Vector3 RecoverSpeed()
    {
        //roll and pitch level
        var roll = aircraftController.rigidbody.rotation.eulerAngles.z;
        var pitch = aircraftController.rigidbody.rotation.eulerAngles.x;
        if (roll > 180f) roll -= 360f;
        if (pitch > 180f) pitch -= 360f;
        return new Vector3(Mathf.Clamp(-pitch, -1, 1), 0, Mathf.Clamp(-roll * rollFactor, -1, 1));

        aircraftController.Flaps = 1f;

    }

    Vector3 AvoidGround()
    {
        //roll level and pull up
        var roll = aircraftController.rigidbody.rotation.eulerAngles.z;
        if (roll > 180f) roll -= 360f;
        return new Vector3(-1, 0, Mathf.Clamp(-roll * rollFactor, -1, 1));
    }


}
