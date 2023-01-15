using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAI : MonoBehaviour
{
    public Rigidbody rigidbody;
    public Transform[] Points;
    public Transform Radar;
    public int pointIndex;
    public float Torque;
    public float Force = 100000f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 CurrentPoint = Points[pointIndex].position;

        if (Vector3.Distance(transform.position, CurrentPoint) < 100f)
        {
            pointIndex++;
        }
        if(pointIndex > Points.Length - 1)
        {
            pointIndex = 0;
        }

        Radar.LookAt(CurrentPoint);
        Vector3 _Torque = Vector3.Cross(Radar.forward, transform.forward);
        rigidbody.AddRelativeTorque(transform.up * _Torque.y * rigidbody.mass * -Torque);
        rigidbody.AddForce(transform.forward * rigidbody.mass * Force);
    }
}
