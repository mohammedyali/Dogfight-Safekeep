using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPositionChanger : MonoBehaviour
{
    public int Angle;
    private float time;
    public float TimeBetween = 4;
    public Transform[] Angles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        transform.position = Angles[Angle].position;
        transform.rotation = Angles[Angle].rotation;

        if(time > TimeBetween)
        {
            Angle++;
            time = 0;
        }
        if(Angle > Angles.Length - 1)
        {
            Angle = 0;
        }

    }
}
