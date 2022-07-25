using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GetTime : MonoBehaviour
{
    [Header("Decorator of \"Time/Current Time\"")]
    [SerializeField] string doubleTBefore;
    [SerializeField] string doubleTInto;
    [SerializeField] string doubleTAfter;
     [Header("Decorator of \"Time\"")]
    [SerializeField] string oneTBefore;
    [SerializeField] string oneTAfter;
    [Min(0)]
    [SerializeField] int r;
    [SerializeField] TrainingManager tm;
    [SerializeField] TrainingSettings ts;
    [Header("Main time")]
    [SerializeField] Text txt;
    [Header("Max time")]
    [SerializeField] List<Text> maxText;

    void Update(){
        txt.text = doubleTBefore + (ts.time).ToString("0.0") + doubleTInto + (tm.GetCurrentTime()).ToString("0.00") + doubleTAfter;
    }

    public void SetMaxTimeText(){
        foreach(Text txt in maxText){
            txt.text = oneTBefore + (ts.time).ToString() + oneTAfter;
        }
    }
}
