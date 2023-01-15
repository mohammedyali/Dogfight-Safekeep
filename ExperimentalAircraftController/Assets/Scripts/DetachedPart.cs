using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachedPart : MonoBehaviour
{
    public ParticleSystem Flame;
    public float LifeTime = 10;
    float time;
    // Start is called before the first frame update
    void Start()
    {
        if(Flame != null)
        {
            Flame.transform.parent = transform;
            Flame.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time > LifeTime) 
        {
            //Flame.transform.parent = null;
            gameObject.SetActive(false);
        }
    }
}
