using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFSettings : MonoBehaviour
{
    [Header("Treat for checkpoint")]
    [SerializeField] float _treatFCheckPoint = 20;


    [HideInInspector]
    public float treatFCheckPoint
    {
        get => _treatFCheckPoint;
    }
    [SerializeField] float minTreatFCheckPoint = -2000f;
    [SerializeField] float maxTreatFCheckPoint = 2000f;


    //Treat for checkpoint
    public void ChangeTreatFCheckPoint(int val)
    {
        if (_treatFCheckPoint + val < minTreatFCheckPoint)
        {
            _treatFCheckPoint = minTreatFCheckPoint;
            return;
        }
        if (_treatFCheckPoint + val > maxTreatFCheckPoint)
        {
            _treatFCheckPoint = maxTreatFCheckPoint;
            return;
        }
        _treatFCheckPoint += val;
    }

    public void SetTreatFCheckPoint(int val)
    {
        if (val < minTreatFCheckPoint)
        {
            _treatFCheckPoint = minTreatFCheckPoint;
            return;
        }
        if (val > maxTreatFCheckPoint)
        {
            _treatFCheckPoint = maxTreatFCheckPoint;
            return;
        }
        _treatFCheckPoint = val;
    }

    [Header("Treat for Finish checkpoint")]
    [SerializeField] float _finTreat = 50;
    [HideInInspector]
    public float finTreat
    {
        get => _finTreat;
    }
    [SerializeField] float minFinTreat = -2000f;
    [SerializeField] float maxFinTreat = 2000f;

    public void ChangeFinTreat(int val)
    {
        if (_finTreat + val < minFinTreat)
        {
            _finTreat = minFinTreat;
            return;
        }
        if (_finTreat + val > maxFinTreat)
        {
            _finTreat = maxFinTreat;
            return;
        }
        _finTreat += val;
    }

    public void SetFinTreat(int val)
    {
        if (val < minFinTreat)
        {
            _finTreat = minFinTreat;
            return;
        }
        if (val > maxFinTreat)
        {
            _finTreat = maxFinTreat;
            return;
        }
        _finTreat = val;
    }

    [Header("Score for sec after finish")]
    [SerializeField] float _scoreAfterFin = 15;
    [HideInInspector]
    public float scoreAfterFin
    {
        get => _scoreAfterFin;
    }
    [SerializeField] float minScoreAfterFin = -2000f;
    [SerializeField] float maxScoreAfterFin = 2000f;

    public void ChangeScoreAfterFin(int val)
    {
        if (_scoreAfterFin + val < minScoreAfterFin)
        {
            _scoreAfterFin = minScoreAfterFin;
            return;
        }
        if (_scoreAfterFin + val > maxScoreAfterFin)
        {
            _scoreAfterFin = maxScoreAfterFin;
            return;
        }
        _scoreAfterFin += val;
    }

    public void SetScoreAfterFin(int val)
    {
        if (val < minScoreAfterFin)
        {
            _scoreAfterFin = minScoreAfterFin;
            return;
        }
        if (val > maxScoreAfterFin)
        {
            _scoreAfterFin = maxScoreAfterFin;
            return;
        }
        _scoreAfterFin = val;
    }

    [Header("Score for colliding with walls")]
    [SerializeField] float _wallTrick = 15;
    [HideInInspector]
    public float wallTrick
    {
        get => _wallTrick;
    }
    [SerializeField] float minWallTrick = -2000f;
    [SerializeField] float maxWallTrick = 2000f;

    public void ChangeWallTrick(int val)
    {
        if (_wallTrick + val < minWallTrick)
        {
            _wallTrick = minWallTrick;
            return;
        }
        if (_wallTrick + val > maxWallTrick)
        {
            _wallTrick = maxWallTrick;
            return;
        }
        _wallTrick += val;
    }

    public void SetWallTrick(int val)
    {
        if (val < minWallTrick)
        {
            _wallTrick = minWallTrick;
            return;
        }
        if (val > maxWallTrick)
        {
            _wallTrick = maxWallTrick;
            return;
        }
        _wallTrick = val;
    }
}
