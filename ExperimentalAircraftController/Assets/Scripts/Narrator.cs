using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Narration
{
    public String Text;
    public float StartTime;
    public float Duration;
    public bool Warning;
}

public class Narrator : MonoBehaviour
{
    public float time;
    public Text text;
    public int Narration;
    public Narration[] Narations;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;

        if(time > Narations[Narration].StartTime && time < Narations[Narration].StartTime + Narations[Narration].Duration)
        {
            text.text = Narations[Narration].Text;
            if (Narations[Narration].Warning == true)
            {
                text.fontStyle = FontStyle.Bold;
            }
            else
            {
                text.fontStyle = FontStyle.Normal;
            }


            if (time + Time.deltaTime > Narations[Narration].StartTime + Narations[Narration].Duration && Narration + 1 < Narations.Length)
            {
                Narration++;
            }
        }
        else
        {
            text.text = null;
        }
    }
}
