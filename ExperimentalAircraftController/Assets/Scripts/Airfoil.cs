using System;
using UnityEditor;
using UnityEngine;

public enum FoilType
{
    Horizontal,
    Vertical
}

public enum ControlType
{
    Aleron,
    Elevator,
    Rudder,
    Flaps,
    None
}

public class Airfoil : MonoBehaviour
{
    [HideInInspector]
    public Vector3 InputMul;
    public float Input;
    public FoilType type;
    public ControlType controlType;
    public float aera;
    public float MaxHealth = 100f;
    [Range(0,1)]
    public float Effectiveness = 1;
    public float power = 10;
    public float cotrolSurfaceAngle = 10;
}
