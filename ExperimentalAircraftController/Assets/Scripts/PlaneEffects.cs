using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneEffects : MonoBehaviour
{
    public float maxAngle;
    [Tooltip("Set particle start size to 1 ")]
    public ParticleSystem[] engineParticles;

    float lightPower;
    public Light[] engineLights;

    public GameObject[] props;
    public float EngineMul = 1;
    public float EngineAcc = 10;
    public float engineSpeed = 0;
    public float maxSpeed = 1000f;
    public bool UseX;

    public Aircraft controller;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Light light in engineLights)
        {
            lightPower = light.intensity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.Pause)
        {
            engineSpeed += EngineAcc * controller.Throttel * Time.deltaTime;
            if (controller.Throttel == 0)
            {
                engineSpeed -= EngineAcc * Time.deltaTime;
            }
            if (engineSpeed > maxSpeed)
            {
                engineSpeed = maxSpeed;
            }
            if (engineSpeed < 0)
            {
                engineSpeed = 0;
            }
            foreach (ParticleSystem jet in engineParticles)
            {
                jet.startSize = controller.Throttel;
            }

            foreach (Light light in engineLights)
            {
                light.intensity = lightPower * controller.Throttel;
            }

            foreach (GameObject prop in props)
            {
                if (!UseX)
                    prop.transform.Rotate(engineSpeed * EngineMul, 0, 0);
                else if (UseX)
                    prop.transform.Rotate(0, engineSpeed * EngineMul, 0);
            }
        }
    }
}
