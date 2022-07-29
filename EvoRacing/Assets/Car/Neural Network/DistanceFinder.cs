using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum DFStates{
    valid,
    invalid,
    finished
}

public class DistanceFinder : MonoBehaviour
{
    public DFSettings dFSettings;
    private float treatFCheckPoint;
    private float finTreat;
    private float scoreAfterFin;
    private float wallTrick;
    [SerializeField] LayerMask wallMask;
    private Vector3 prevPos = Vector3.zero;
    [SerializeField] private Vector3 curPos = Vector3.zero;
    [SerializeField] private float distance = 0;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private MeshRenderer carBody;

    [SerializeField] DFStates dfState = DFStates.valid;
    private bool touching = false;

    private void ChangeMat()
    {
        carBody.material = invalidMaterial;
        print("Stop " + distance.ToString());
    }
    private void Start(){
        treatFCheckPoint = dFSettings.treatFCheckPoint;
        finTreat = dFSettings.finTreat;
        scoreAfterFin = dFSettings.scoreAfterFin;
        wallTrick = dFSettings.wallTrick;
    }
    
    public void Refresh()
    {
        touching = false;
        dfState = DFStates.valid;
        prevPos = Vector3.zero;
        curPos = Vector3.zero;
        distance = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "CheckPoint":
                if (dfState == DFStates.invalid || touching)
                {
                    break;
                }

                //First check point
                if (curPos == prevPos)
                {
                    touching = true;
                    curPos = other.transform.position;
                    break;
                }

                if (prevPos == other.transform.position)
                {
                    distance -= treatFCheckPoint * 2;
                    ChangeMat();
                    dfState = DFStates.invalid;
                }



                touching = true;
                prevPos = curPos;
                curPos = other.transform.position;
                distance += (prevPos - other.transform.position).magnitude;
                distance += treatFCheckPoint;
                break;

            case "Fin":
                distance += finTreat;
                dfState = DFStates.finished;
                break;

        }

    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (dfState == DFStates.valid)
            if (collisionInfo.gameObject.layer == wallMask)
            {
                distance -= wallTrick * Time.deltaTime;
                if (distance < 0)
                {
                    ChangeMat();
                    dfState = DFStates.invalid;
                }
            }
    }

    private void OnTriggerExit(Collider other)
    {
        touching = false;
    }

    private void Update()
    {
        if (dfState == DFStates.finished)
        {
            distance += scoreAfterFin * Time.deltaTime;
        }
    }

    public float GetDist()
    {
        if (dfState == DFStates.valid)
            return distance + (curPos - transform.position).magnitude;
        return distance;
    }

    public void StopDF(){
        dfState = DFStates.invalid;
    }
}
