using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingGear : MonoBehaviour
{
    public bool IsDown = true;
    [Header("Main Gear")]
    public Vector3 UpRot;
    public Vector3 DownRot;
    public Vector3 UpPos, DownPos;
    [Header("Wheel")]
    public WheelCollider WheelCol;
    public Transform Wheel;
    public Vector3 WheelUpRot, WheelDownRot;
    public Vector3 WheelUpPos, WheelDownPos;
    [Range(0,1)]
    public float time;
    float SusDist;
    // Start is called before the first frame update
    void Start()
    {
        SusDist = WheelCol.suspensionDistance;
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        if (!IsDown)
        {
            WheelCol.suspensionDistance = 0f;
        }
        else
        {
            WheelCol.suspensionDistance = SusDist;
        }

        if (IsDown)
        {
            WheelCol.gameObject.SetActive(true);
            transform.localPosition = Vector3.Lerp(transform.localPosition, DownPos, time);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(DownRot), time);

            Wheel.localPosition = Vector3.Lerp(Wheel.localPosition, WheelDownPos, time);
            Wheel.localRotation = Quaternion.Lerp(Wheel.localRotation, Quaternion.Euler(WheelDownRot), time);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, UpPos, time);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(UpRot), time);

            Wheel.localPosition = Vector3.Lerp(Wheel.localPosition, WheelUpPos, time);
            Wheel.localRotation = Quaternion.Lerp(Wheel.localRotation, Quaternion.Euler(WheelUpRot), time);
        }
    }

    public void Gear()
    {
        if(IsDown == true)
        {
            IsDown = false;
        }
        else if (IsDown == false)
        {
            IsDown = true;
        }
    }

}
