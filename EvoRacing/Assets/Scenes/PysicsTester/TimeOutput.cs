using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeOutput : MonoBehaviour
{
    float time = 0f;
    bool stop = true;
    Text text;
    void Start()
    {
        text = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        if(stop)
            time += Time.fixedDeltaTime;
    }
    void Update()
    {
        text.text = time.ToString();
    }

    public void Stop()
    {
        stop = false;
    }
}
