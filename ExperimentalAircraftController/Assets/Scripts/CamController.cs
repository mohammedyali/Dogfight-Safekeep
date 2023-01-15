﻿using UnityEngine;

public class CamController : MonoBehaviour
{
    Transform rootNode;
    Transform carCam;
    public Transform car;
    Rigidbody carPhysics;
    bool Rotatable;

    [Tooltip("If car speed is below this value, then the camera will default to looking forwards.")]
    public float rotationThreshold = 1f;

    [Tooltip("How closely the camera follows the car's position. The lower the value, the more the camera will lag behind.")]
    public float cameraStickiness = 10.0f;

    [Tooltip("How closely the camera matches the car's velocity vector. The lower the value, the smoother the camera rotations, but too much results in not being able to see where you're going.")]
    public float cameraRotationSpeed = 5.0f;

    void Awake()
    {
        carCam = Camera.main.GetComponent<Transform>();
        rootNode = GetComponent<Transform>();
        //car = rootNode.parent.GetComponent<Transform>();
        carPhysics = car.GetComponent<Rigidbody>();
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        // Detach the camera so that it can move freely on its own.
        rootNode.parent = null;
    }
    void Update()
    {
        Rotatable = Input.GetMouseButton(0);
    }
    void FixedUpdate()
    {
        Quaternion look;
        if (!Rotatable)
        {
            // Moves the camera to match the car's position.
            rootNode.position = Vector3.Lerp(rootNode.position, car.position, cameraStickiness * Time.fixedDeltaTime);

            // If the car isn't moving, default to looking forwards. Prevents camera from freaking out with a zero velocity getting put into a Quaternion.LookRotation
            if (carPhysics.velocity.magnitude < rotationThreshold)
                look = Quaternion.LookRotation(car.forward);
            else
                look = Quaternion.LookRotation(carPhysics.velocity.normalized);

            // Rotate the camera towards the velocity vector.
            look = Quaternion.Slerp(rootNode.rotation, look, cameraRotationSpeed * Time.fixedDeltaTime);
            rootNode.rotation = look;
        }
        else
        {

            rootNode.transform.Rotate(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            // Moves the camera to match the car's position.
            rootNode.position = Vector3.Lerp(rootNode.position, car.position, cameraStickiness * Time.fixedDeltaTime);
        }

    }
}