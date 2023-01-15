using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Transform Croshair;
    public Transform TargetDist;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Croshair.position = Camera.main.WorldToScreenPoint(TargetDist.position);
    }
}
