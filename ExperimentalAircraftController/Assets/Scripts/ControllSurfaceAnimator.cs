using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllSurfaceAnimator : MonoBehaviour
{
    public Aircraft controller;
    public Transform Elevator;
    public float MaxElevatorAngle = 10f;
    public bool UseYElevator;
    public Transform AleronLeft;
    public Transform AleronRight;
    public float MaxAleronAngle = 10f;
    public bool UseYAleron;
    public Transform Rudder;
    public WheelCollider Stearwheel;
    public float SterrwheelSence = 1;
    public float MaxRudderAngle = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {



        if(!UseYElevator)
            Elevator.localEulerAngles = new Vector3(MaxElevatorAngle * -controller.InputY, Elevator.localEulerAngles.y, Elevator.localEulerAngles.z);
        else
            Elevator.localEulerAngles = new Vector3(Elevator.localEulerAngles.x, MaxElevatorAngle * -controller.InputY, Elevator.localEulerAngles.z);

        if (!UseYAleron)
        {
            AleronLeft.localEulerAngles = new Vector3(MaxAleronAngle * -controller.InputX, AleronLeft.localEulerAngles.y, AleronLeft.localEulerAngles.z);
            AleronRight.localEulerAngles = new Vector3(MaxAleronAngle * controller.InputX, AleronRight.localEulerAngles.y, AleronRight.localEulerAngles.z);
        }
        else
        {
            AleronLeft.localEulerAngles = new Vector3(AleronLeft.localEulerAngles.x, MaxAleronAngle * -controller.InputX, AleronLeft.localEulerAngles.z);
            AleronRight.localEulerAngles = new Vector3(AleronRight.localEulerAngles.x, MaxAleronAngle * controller.InputX, AleronRight.localEulerAngles.z);
        }
        Rudder.localEulerAngles = new Vector3(Rudder.localEulerAngles.x, Rudder.localEulerAngles.y,MaxRudderAngle * -controller.InputYaw);
        Stearwheel.steerAngle = MaxRudderAngle * controller.InputYaw * -SterrwheelSence;
    }
}
