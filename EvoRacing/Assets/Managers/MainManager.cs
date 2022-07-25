using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    bool _pause = false;
    [HideInInspector]
    public bool pause{get => _pause;}
    [SerializeField] float _timeScale = 1f;
    
    public float timeScale{get => _timeScale;}
    [SerializeField] float minTimeScale = 0.25f;

    [SerializeField] float maxTimeScale = 4f;

    public void PauseGame(){
        _pause = true;
        Time.timeScale = 0;
    }

    public void UnPauseGame(){
        _pause = false;
        Time.timeScale = _timeScale;
    }

    public void ChangeTimeScale(float val){
        if(_timeScale + val < minTimeScale){
            _timeScale = minTimeScale;
            return;
        }
        if(_timeScale + val > maxTimeScale){
            _timeScale = maxTimeScale;
            return;
        }
        
        _timeScale += val;
        if(!_pause) {Time.timeScale = _timeScale;}
    }

    public void SetTimeScale(float val){
        if(val < minTimeScale){
            _timeScale = minTimeScale;
            return;
        }
        if(val > maxTimeScale){
            _timeScale = maxTimeScale;
            return;
        }
        _timeScale = val;
        if(!_pause) {Time.timeScale = _timeScale;}
    }
}
