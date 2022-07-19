using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GetTime : MonoBehaviour
{
    [SerializeField] string Before;
    [SerializeField] string Into;
    [SerializeField] string After;
    [Min(0)]
    [SerializeField] int r;
    [SerializeField] TrainingManager tm;
    [SerializeField] Text txt;

    void Update(){
        txt.text = Before + (tm.GetMaxTime()).ToString("0.0") + Into + (tm.GetCurrentTime()).ToString("0.00") + After;
    }
}
