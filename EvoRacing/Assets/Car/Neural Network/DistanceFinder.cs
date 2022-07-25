using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFinder : MonoBehaviour
{

    public DFSettings dfSettings;
    [SerializeField] LayerMask wallMask;
    private Vector3 prevPos = Vector3.zero;
    [SerializeField] private Vector3 curPos = Vector3.zero;
    [SerializeField] private float distance = 0;
    [SerializeField] private bool valid = true;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private MeshRenderer carBody;
    private bool touching = false;

    private bool finished = false;

    private void ChangeMat()
    {
        carBody.material = invalidMaterial;
        print("Stop " + distance.ToString());
    }
    public void Refresh()
    {
        touching = false;
        finished = false;
        valid = true;
        prevPos = Vector3.zero;
        curPos = Vector3.zero;
        distance = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "CheckPoint":
                if (!valid || touching)
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

                if (curPos == other.transform.position)
                {

                    ChangeMat();
                    valid = false;
                    break;
                }

                if (prevPos == other.transform.position)
                {
                    distance -= dfSettings.treatFCheckPoint * 2;
                    ChangeMat();
                    valid = false;
                }



                touching = true;
                prevPos = curPos;
                curPos = other.transform.position;
                distance += (prevPos - other.transform.position).magnitude;
                distance += dfSettings.treatFCheckPoint;

                break;

            case "Fin":
                distance += dfSettings.finTreat;
                finished = true;
                break;

        }

    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (valid && !finished)
            if (collisionInfo.gameObject.layer == wallMask)
            {
                distance -= dfSettings.wallTrick * Time.deltaTime;
                if (distance < 0)
                {
                    ChangeMat();
                    valid = false;
                }
            }
    }

    private void OnTriggerExit(Collider other)
    {
        touching = false;
    }

    private void Update()
    {
        if (finished)
        {
            distance += dfSettings.scoreAfterFin * Time.deltaTime;
        }
    }

    public float GetDist()
    {
        if (valid && !finished)
            return distance + (curPos - transform.position).magnitude;
        return distance;
    }
}
