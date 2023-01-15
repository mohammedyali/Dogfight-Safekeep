using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : MonoBehaviour
{
    public DamageModel Target;
    public float MaxSpeed = 1500;
    public float Speed;
    public float force = 1500;
    public float Torque = 15;
    public Rigidbody rigidbody;
    public Collider collider;
    public TrailRenderer Particles;
    public Transform OnboardRadar;
    public GameObject Audio;
    public GameObject Explosion;
    [Header("Fuse")]
    public float ProximityFuse = 2f;
    public float ExplosionDist = 10f;
    public float Damage = 200f;
    public float RocketDelay = 3f;
    public float FuseDelay = 3f;
    [Header("Drag")]
    public Vector3 Velocity;
    public Vector3 LocalVelocity;
    public Vector3 LocalAngularVelocity;
    public float AirPressure = 14.7f;
    public AnimationCurve CSD;
    public float sideAera;
    public float UpperAera;
    public bool Activated = false;
    private float time;
   
    void OnCollisionEnter(Collision CollisionInfo)
    {
        Explode();
    }

    private void FixedUpdate()
    {
        if (Activated)
        {
            if(time < RocketDelay)
            {
                rigidbody.useGravity = true;
                collider.enabled = false;
                Audio.SetActive(false);
            }
            else
            {
                Particles.emitting = (true);
                Audio.SetActive(true);
                Vector3 _target = FirstOrderIntercept(rigidbody.position, Vector3.zero, Speed * 0.27777778f, Target.transform.position, Target.gameObject.GetComponent<Aircraft>().Velocity);

                if(Target == null)
                {
                    //Target Has Died before missle could hit.
                    Destroy(transform.gameObject);
                }

                OnboardRadar.LookAt(_target);

                collider.enabled = true;
                if (Speed < MaxSpeed)
                    rigidbody.AddForce(transform.forward * force);
                rigidbody.useGravity = false;
                Track();
            }
            if (time > FuseDelay)
            {
                //canExplode
                if (Vector3.Distance(transform.position, Target.PlaneRoot.collider.transform.position) < ProximityFuse)
                {
                    Explode();
                }
                if (Vector3.Angle(transform.forward, OnboardRadar.forward) > 50)
                {
                    Explode();
                }
            }
            time += Time.deltaTime;
        }
        else
        {
            Particles.emitting = (false);
            rigidbody.isKinematic = true;
        }

        CalculateState();
        Drag();
    }

    void Track()
    {
        rigidbody.AddTorque(Vector3.Cross(transform.forward, OnboardRadar.forward) * Torque);
    }

    void Drag()
    {
        float DRight;
        float DUp;
        DRight = CSD.Evaluate(LocalVelocity.x) * (0.5f * AirPressure * (LocalVelocity.z * LocalVelocity.z) * sideAera);
        DUp = CSD.Evaluate(LocalVelocity.y) * (0.5f * AirPressure * (LocalVelocity.z * LocalVelocity.z) * UpperAera);
        rigidbody.AddForce(-transform.right * DRight);
        rigidbody.AddForce(transform.up * -DUp);
        Debug.DrawRay(transform.position, transform.up * (-DUp / rigidbody.mass), Color.red);
        Debug.DrawRay(transform.position, transform.right * (DRight / rigidbody.mass), Color.red);
    }
    void CalculateState()
    {
        Speed = LocalVelocity.z * 5.76f;
        var invRotation = Quaternion.Inverse(rigidbody.rotation);
        Velocity = rigidbody.velocity;
        LocalVelocity = invRotation * Velocity;  //transform world velocity into local space
        LocalAngularVelocity = invRotation * rigidbody.angularVelocity;  //transform into local space
    }

    public void FireMissle()
    {
        rigidbody.velocity = transform.root.GetComponent<Rigidbody>().velocity;
        rigidbody.AddForce(transform.up * -1000);
        transform.parent = null;
        rigidbody.isKinematic = false;
        Activated = true;
    }

    public void Explode()
    {
        if(Particles != null)
        {
            Particles.transform.parent = null;
        }
        foreach (DamageComponent _damageComponent in Target.damageComponents)
        {
            _damageComponent.airfoil.Effectiveness -= ((ExplosionDist / Vector3.Distance(transform.position, _damageComponent.collider.transform.position)) * Damage) / _damageComponent.airfoil.MaxHealth;
            Target.RootHealth -= ((ExplosionDist / Vector3.Distance(transform.position, Target.transform.position)) * Damage) / Target.MaxRootHealth;
            if (_damageComponent.airfoil.Effectiveness < 0)
            {
                Instantiate(Explosion, transform.position, transform.rotation);
                Target.DamageModelDetachPartUsingSphereCast(_damageComponent.collider);
                Debug.Log($"Destroyed '{_damageComponent.airfoil.gameObject.name}'");
                Destroy(transform.gameObject);
            }
            Debug.Log($"Explosively hit '{_damageComponent.airfoil.gameObject.name}'");
            Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(transform.gameObject);
        }
    }

    public static Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity)
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime(
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return targetPosition + t * (targetRelativeVelocity);
    }

    //first-order intercept using relative target position
    public static float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
        {
            return 0f;
        }

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                {
                    return t1; //only t1 is positive
                }
            }
            else
            {
                return Mathf.Max(t2, 0f); //don't shoot back in time
            }
        }
        else if (determinant < 0f)
        { //determinant < 0; no intercept path
            return 0f;
        }
        else
        { //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
        }
    }

}
