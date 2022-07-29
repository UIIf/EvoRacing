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

    //Count of cars
    public void ChangeCarNum(int val){
        if(_carNum + val < minCarNum){
            _carNum = minCarNum;
            return;
        }
        if(_carNum + val > maxCarNum){
            _carNum = maxCarNum;
            return;
        }
        
        _carNum += val;
    }

    public void SetCarNum(int val){
        if(val < minCarNum){
            _carNum = minCarNum;
            return;
        }
        if(val > maxCarNum){
            _carNum = maxCarNum;
            return;
        }
        
        _carNum = val;
    }

    [Header("Count of slected cars")]
    [SerializeField] int _selCarNum = 10;
    [HideInInspector]
    public int selCarNum{
        get => _selCarNum;
    }
    [SerializeField] int minSelCarNum = 0;
    

    //Count of cars
    public void ChangeSelCarNum(int val){
        if(_selCarNum + val < minSelCarNum){
            _selCarNum = minSelCarNum;
            return;
        }
        if(_selCarNum + val > maxCarNum){
            _selCarNum = maxCarNum;
            return;
        }
        
        _selCarNum += val;
    }

    public void SetSelCarNum(int val){
        if(val < minSelCarNum){
            _selCarNum = minSelCarNum;
            return;
        }
        if(val > maxCarNum){
            _selCarNum = maxCarNum;
            return;
        }
        
        _selCarNum = val;
    }

    [Header("Count of childrens")]
    [SerializeField] int _from2Parents = 5;
    [HideInInspector]
    public int from2Parents{
        get => _from2Parents;
    }

    [SerializeField] int minFrom2Parents = 3;

    [SerializeField] int maxFrom2Parents = 200;

    //Count of childrenss
    public void ChangeFrom2P(int val){
        if(_from2Parents + val < minFrom2Parents){
            _from2Parents = minFrom2Parents;
            return;
        }
        if(_from2Parents + val > maxFrom2Parents){
            _from2Parents = maxFrom2Parents;
            return;
        }
        
        _from2Parents += val;
    }

    public void SetFrom2P(int val){
        if(val < minFrom2Parents){
            _from2Parents = minFrom2Parents;
            return;
        }
        if(val > maxFrom2Parents){
            _from2Parents = maxFrom2Parents;
            return;
        }
        _from2Parents = val;
    }

    [Header("PersentOfMutation")]

    [SerializeField] float _percentOfMutation = 0.1f;
    [HideInInspector]
    public float percentOfMutation{
        get => _percentOfMutation;
    }
    float minPercentOfMutation = 0f;
    float maxPcentOfMutation = 1f;

     //PersentOfMutation
    public void ChangePerсOfMut(float val){
        if(_percentOfMutation + val < minPercentOfMutation){
            _percentOfMutation = minPercentOfMutation;
            return;
        }
        if(_percentOfMutation + val > maxPcentOfMutation){
            _percentOfMutation = maxPcentOfMutation;
            return;
        }
        
        _percentOfMutation += val;
    }

    public void SetPerсOfMut(float val){
        if(val < minPercentOfMutation){
            _percentOfMutation = minPercentOfMutation;
            return;
        }
        if(val > maxPcentOfMutation){
            _percentOfMutation = maxPcentOfMutation;
            return;
        }
        _percentOfMutation = val;
    }


    [Header("Mutation value")]
    [SerializeField] float _mutationValue = 0.5f;
    [HideInInspector]
    public float mutationValue{
        get => _mutationValue;
    }
    [SerializeField] float minMutationValue = 0f;
    [SerializeField] float maxMutationValue = 10f;

     //Mutation value

    public void ChangeMutationValue(float val){
        if(_mutationValue + val < minMutationValue){
            _mutationValue = minMutationValue;
            return;
        }
        if(_mutationValue + val > maxMutationValue){
            _mutationValue = maxMutationValue;
            return;
        }
        
        _mutationValue += val;
    }

    public void SetMutationValue(float val){
        if(val < minMutationValue){
            _mutationValue = minMutationValue;
            return;
        }
        if(val > maxMutationValue){
            _mutationValue = maxMutationValue;
            return;
        }
        _mutationValue = val;
    }

    [Header("Time of training")]
    [SerializeField] float _time = 60;
    [HideInInspector]
    public float time{
        get => _time;
    }
    [SerializeField] float minTime = 2.5f;
    [SerializeField] float maxTime = 300f;

    //Time

    public void ChangeTime(float val){
        if(_time + val < minTime){
            _time = minTime;
            return;
        }
        if(_time + val > maxTime){
            _time = maxTime;
            return;
        }
        
        _time += val;
    }

    public void SetTime(float val){
        if(val < minTime){
            _time = minTime;
            return;
        }
        if(val > maxTime){
            _time = maxTime;
            return;
        }
        _time = val;
    }
}
