using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSettings : MonoBehaviour
{
    [Header("Count of cars")]
    [SerializeField] int _carNum = 10;
    [HideInInspector]
    public int carNum{
        get => _carNum;
    }
    [SerializeField] int minCarNum = 10;
    [SerializeField] int maxCarNum = 200;
    [Header("Count of childrens")]
    [SerializeField] int _from2Parents = 5;
    [HideInInspector]
    public int from2Parents{
        get => _from2Parents;
    }

    [SerializeField] int minFrom2Parents = 3;

    [SerializeField] int maxFrom2Parents = 200;

    [Header("PersentOfMutation")]

    [SerializeField] float _percentOfMutation = 0.1f;
    [HideInInspector]
    public float percentOfMutation{
        get => _percentOfMutation;
    }
    float minPercentOfMutation = 0f;
    float maxPcentOfMutation = 1f;

    [Header("Mutation value")]
    [SerializeField] float _mutationValue = 0.5f;
    [HideInInspector]
    public float mutationValue{
        get => _mutationValue;
    }
    [SerializeField] float minMutationValue = 0f;
    [SerializeField] float maxMutationValue = 10f;

    [Header("Time of training")]
    [SerializeField] float _time = 60;
    [HideInInspector]
    public float time{
        get => _time;
    }
    [SerializeField] float minTime = 2.5f;
    [SerializeField] float maxTime = 300f;

    //Count of cars
    public bool ChangeCarNum(int val){
        if(_carNum + val < minCarNum || _carNum + val > maxCarNum){
            return false;
        }
        _carNum += val;
        return true;
    }

    public bool SetCarNum(int val){
        if(val < minCarNum || val > maxCarNum){
            return false;
        }
        _carNum = val;
        return true;
    }
    //Count of childrenss
    public bool ChangeFrom2P(int val){
        if(_from2Parents + val < minFrom2Parents || _from2Parents + val > maxFrom2Parents){
            return false;
        }
        _from2Parents += val;
        return true;
    }

    public bool SetFrom2P(int val){
        if(val < minFrom2Parents || val > maxFrom2Parents){
            return false;
        }
        _from2Parents = val;
        return true;
    }
    //PersentOfMutation
    public bool ChangePerсOfMut(float val){
        if(_percentOfMutation + val < minPercentOfMutation || _percentOfMutation + val > maxPcentOfMutation){
            return false;
        }
        _percentOfMutation += val;
        return true;
    }

    public bool SetPerсOfMut(float val){
        if(val < minPercentOfMutation || val > maxPcentOfMutation){
            return false;
        }
        _percentOfMutation = val;
        return true;
    }

    //Mutation value

    public bool ChangeMutationValue(float val){
        if(_mutationValue + val < minMutationValue || _mutationValue + val > maxMutationValue){
            return false;
        }
        _mutationValue += val;
        return true;
    }

    public bool SetMutationValue(float val){
        if(val < minMutationValue || val > maxMutationValue){
            return false;
        }
        _mutationValue = val;
        return true;
    }

    //Time

    public bool ChangeTime(float val){
        if(_time + val < minTime || _time + val > maxTime){
            return false;
        }
        _time += val;
        return true;
    }

    public bool SetTime(float val){
        if(val < minTime || val > maxTime){
            return false;
        }
        _time = val;
        return true;
    }
}
