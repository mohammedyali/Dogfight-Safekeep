using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft : MonoBehaviour
{
    public float Speed;
    public Rigidbody rigidbody;
    public float EnginePower;
    [HideInInspector]
    public float PowMul = 1;
    [Range(0,1)]
    public float Throttel;
    [Range(-1, 1)]
    public float InputX;
    [Range(-1, 1)]
    public float InputY;
    [Range(-1, 1)]
    public float InputYaw;
    public Transform CM;
    public Transform[] EnginePoints;
    public Airfoil[] airfoils;
    public Vector3 Velocity;
    public Vector3 LocalVelocity;
    public Vector3 LocalAngularVelocity;
    public float AirPressure = 14.7f;
    public float AngleOfAttack;
    public float AngleOfAttackYaw;
    public AnimationCurve CL;
    public AnimationCurve CSD;
    public AnimationCurve CD;
    public float sideAera;
    public float frontAera;
    public float UpperAera;
    [Range(0, 1)]
    public float Flaps;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody.centerOfMass = CM.localPosition;
    }

    private void Update()
    {
        foreach (Airfoil foil in airfoils)
        {
            Debug.DrawRay(foil.transform.position, foil.transform.up * (CalculateTheLiftOfAirfoil(foil) / rigidbody.mass), Color.green);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalculateState();
        ApplyForces();
        foreach (Transform point in EnginePoints)
        {
            rigidbody.AddForceAtPosition(point.forward * EnginePower * Throttel * PowMul, point.position);
        }
    }

    public float CalculateLift(float aera, float CL, float Speed)
    {

        float l = CL * (0.5f * AirPressure * (Speed * Speed) * aera);
        return l;
    }
    public Vector3 InputCal(Airfoil airfoil)
    {
        Vector3 I = new Vector3(InputY * airfoil.InputMul.x, InputYaw * airfoil.InputMul.y, InputX * airfoil.InputMul.z);
        return I;
    }
    public float CalculateTheLiftOfAirfoil(Airfoil airfoil)
    {
        float l = 0;
        if(airfoil.controlType == ControlType.Elevator)
        {
            airfoil.Input = InputCal(airfoil).x;

            if (airfoil.type == FoilType.Horizontal)
                l = CalculateLift(airfoil.aera * airfoil.power, CL.Evaluate(AngleOfAttack + airfoil.cotrolSurfaceAngle * airfoil.Input), Speed    );
            if (airfoil.type == FoilType.Vertical)
                l = CalculateLift(airfoil.aera, CL.Evaluate(AngleOfAttackYaw + airfoil.cotrolSurfaceAngle * airfoil.Input), Speed);
        }
        if(airfoil.controlType == ControlType.Rudder)
        {
            airfoil.Input = InputCal(airfoil).y;

            if (airfoil.type == FoilType.Horizontal)
                l = CalculateLift(airfoil.aera * airfoil.power, CL.Evaluate(AngleOfAttack + airfoil.cotrolSurfaceAngle * airfoil.Input), Speed);
            if (airfoil.type == FoilType.Vertical)
                l = CalculateLift(airfoil.aera, CL.Evaluate(AngleOfAttackYaw + airfoil.cotrolSurfaceAngle * airfoil.Input), Speed);
        }
        if(airfoil.controlType == ControlType.Aleron)
        {
            airfoil.Input = InputCal(airfoil).z;

            if (airfoil.type == FoilType.Horizontal)
                l = CalculateLift(airfoil.aera * airfoil.power, CL.Evaluate(AngleOfAttack + airfoil.cotrolSurfaceAngle * airfoil.Input), Speed);
            if (airfoil.type == FoilType.Vertical)
                l = CalculateLift(airfoil.aera, CL.Evaluate(AngleOfAttackYaw + airfoil.cotrolSurfaceAngle * airfoil.Input), Speed);
        }
        if (airfoil.controlType == ControlType.Flaps)
        {

            airfoil.Input = Flaps;

            if (airfoil.type == FoilType.Horizontal)
                l = CalculateLift(airfoil.aera * airfoil.power, CL.Evaluate(AngleOfAttack + airfoil.cotrolSurfaceAngle * airfoil.Input), Speed);
            if (airfoil.type == FoilType.Vertical)
                l = CalculateLift(airfoil.aera, CL.Evaluate(AngleOfAttackYaw + airfoil.cotrolSurfaceAngle * airfoil.cotrolSurfaceAngle * airfoil.Input), Speed);
        }
        if (airfoil.controlType == ControlType.None)
        {
            if (airfoil.type == FoilType.Horizontal)
                l = CalculateLift(airfoil.aera * airfoil.power, CL.Evaluate(AngleOfAttack), Speed);
            if (airfoil.type == FoilType.Vertical)
                l = CalculateLift(airfoil.aera, CL.Evaluate(AngleOfAttackYaw + airfoil.cotrolSurfaceAngle * airfoil.cotrolSurfaceAngle * airfoil.Input), Speed);
        }
        return l * airfoil.Effectiveness;
    }
    void ApplyForces()
    {
        Drag();
        foreach (Airfoil foil in airfoils)
        {
            if(foil != null)
                rigidbody.AddForceAtPosition(foil.transform.up * CalculateTheLiftOfAirfoil(foil), foil.transform.position);
        }
    }
    void Drag()
    {
        float DRight;
        float DUp;
        float Dfor;
        DRight = CSD.Evaluate(LocalVelocity.x) * (0.5f * AirPressure * (LocalVelocity.z * LocalVelocity.z) * sideAera);
        DUp = CSD.Evaluate(LocalVelocity.y) * (0.5f * AirPressure * (LocalVelocity.z * LocalVelocity.z) * UpperAera);
        Dfor = (CD.Evaluate(LocalVelocity.z)) * (0.5f * AirPressure * (LocalVelocity.z * LocalVelocity.z) * frontAera);
        rigidbody.AddForce(-transform.right * DRight);
        rigidbody.AddForce(transform.up * -DUp);
        rigidbody.AddForce(-transform.forward * Dfor);
        Debug.DrawRay(transform.position, transform.forward * (-Dfor / rigidbody.mass), Color.red);
        Debug.DrawRay(transform.position, transform.up * (-DUp / rigidbody.mass), Color.red);
        Debug.DrawRay(transform.position, transform.right * (DRight / rigidbody.mass), Color.red);


    }
    void CalculateState()
    {
        Speed = LocalVelocity.z * 5.76f;
        CalculateAngleOfAttack();
        var invRotation = Quaternion.Inverse(rigidbody.rotation);
        Velocity = rigidbody.velocity;
        LocalVelocity = invRotation * Velocity;  //transform world velocity into local space
        LocalAngularVelocity = invRotation * rigidbody.angularVelocity;  //transform into local space
    }
    void CalculateAngleOfAttack()
    {
        AngleOfAttack = Mathf.Atan2(-LocalVelocity.y, LocalVelocity.z) * 57.2957795f;
        AngleOfAttackYaw = Mathf.Atan2(LocalVelocity.x, LocalVelocity.z) * 57.2957795f;
    }
}
