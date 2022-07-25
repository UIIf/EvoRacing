using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowMainManager : MonoBehaviour
{
    [SerializeField] MainManager MM;
    [Header("TimeScale")]
    [SerializeField] List<Text> TStext;
    [SerializeField] string TSBefore;
    [SerializeField] string TSAfter;

    public void UpdateTSText(){
        foreach(Text txt in TStext){
            txt.text = TSBefore + (MM.timeScale).ToString() + TSAfter;
        }
    }

}
