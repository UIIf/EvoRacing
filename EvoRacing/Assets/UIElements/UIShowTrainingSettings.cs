using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowTrainingSettings : MonoBehaviour
{
    [SerializeField] TrainingSettings trSettings;
    [Header("CarNum")]
    [SerializeField] List<Text> CarNum;
    [SerializeField] string AfterCarNum;
    public void UpdateCarNumText(){
        foreach(Text txt in CarNum){
            txt.text = (trSettings.carNum).ToString() + AfterCarNum;
        }
    }
    [SerializeField] List<Text> from2Parents;
    [SerializeField] string AfterFrom2Parents;
    public void UpdateFrom2ParentsText(){
        foreach(Text txt in from2Parents){
            txt.text = (trSettings.from2Parents).ToString() + AfterFrom2Parents;
        }
    }
    [SerializeField] List<Text> percentOfMutation;
    [SerializeField] string AfterPercentOfMutation;
    public void UpdatePercentOfMutationText(){
        foreach(Text txt in percentOfMutation){
            txt.text = (trSettings.percentOfMutation * 100).ToString() + AfterPercentOfMutation;
        }
    }
    [SerializeField] List<Text> mutationValue;
    [SerializeField] string AfterMutationValue;
    public void UpdateMutationValueText(){
        foreach(Text txt in mutationValue){
            txt.text = (trSettings.mutationValue).ToString() + AfterMutationValue;
        }
    }
}
