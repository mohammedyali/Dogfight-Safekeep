using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float MaxGs = 10;
    public float CurrentGs;
    public Aircraft aircraft;
    public Radar radar;
    public Gun[] guns;
    public LandingGear[] gears;
    public WheelCollider tailwheel;
    [Range(0,1)]
    public float Throttel;
    public bool WorkingEngine = true;
    // Start is called before the first frame update
    void Start()
    {
        WorkingEngine = true;
    }

    // Update is called once per frame
    void Update()
    {

        Throttel += Input.mouseScrollDelta.y * 5f / 100f;
        if (Throttel < 0)
        {
            Throttel = 0f;
        }
        if (Throttel > 1)
        {
            Throttel = 1;
        }

        aircraft.Throttel = Throttel;
        if (!WorkingEngine)
            aircraft.Throttel = 0f;
        if (Input.GetKey(KeyCode.B)) 
        {
            foreach (LandingGear gear in gears)
            {
                gear.WheelCol.brakeTorque = 1000f;
            }
        }
        else
        {
            foreach (LandingGear gear in gears)
            {
                gear.WheelCol.brakeTorque = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && radar != null)
        {
            radar.Fire();
        }

        float Gmul = 0;
        if(CurrentGs < MaxGs)
        {
            Gmul = 1;
        }
        aircraft.InputX = Input.GetAxis("Horizontal") * Gmul;
        aircraft.InputY = Input.GetAxis("Vertical") * Gmul;
        aircraft.InputYaw = Input.GetAxis("Yaw") * Gmul;


        if(Input.GetKey(KeyCode.F)) 
        { 
            aircraft.Flaps = Mathf.Lerp(aircraft.Flaps, 1, .2f); 
        } 
        else 
        { 
            aircraft.Flaps = Mathf.Lerp(aircraft.Flaps, 0, .2f); 
        }
        if (Input.GetKey(KeyCode.Space))
        {

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
        if (Input.GetKeyDown(KeyCode.G))
        {
            foreach (LandingGear item in gears)
            {
                item.Gear();
            }
        }
    }

    Vector3 lastVel;
    private void FixedUpdate()
    {
        Vector3 vel = aircraft.Velocity;
        Vector3 accel = (vel - lastVel) / Time.fixedDeltaTime;

        CurrentGs = (accel / 9.8f).magnitude;

        lastVel = vel;
    }
}
