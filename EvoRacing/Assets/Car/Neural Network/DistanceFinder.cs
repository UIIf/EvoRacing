using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFinder : MonoBehaviour
{
    [Tooltip("Treat that given to car when it collides with check point(i think it must be more then any distance between to chek points)")]
    [SerializeField] float treat;
    [Tooltip("Penalty for crashing in the wall")]
    [SerializeField] float trick;
    [SerializeField] LayerMask wallMask;
    private Vector3 prevPos = Vector3.zero;
    [SerializeField] private Vector3 curPos = Vector3.zero;
    private Vector3 curNorm;
    [SerializeField] private float distance = 0;
    private float temp_dist_sqr = 0;
    [SerializeField] private bool valid = true;
    private bool touching = false;
    
    private void OnTriggerEnter(Collider other){
        switch (other.tag)
        {
            case "CheckPoint":
                if(!valid || touching){
                    break;
                }

                //First check point
                if(curPos == prevPos){
                    touching = true;
                    curPos = other.transform.position;
                    curNorm = other.transform.forward;
                    break;
                }

                if(curPos == other.transform.position){
                    distance += Mathf.Sqrt(temp_dist_sqr);
                    print("Stop " + distance.ToString());
                    valid = false;
                    break;
                }

                if(prevPos == other.transform.position){
                    distance -= treat*2 ;
                    print("Stop " + distance.ToString());
                    valid = false;
                }


                
                touching = true;
                prevPos = curPos;
                curPos = other.transform.position;
                curNorm = other.transform.forward;
                distance += (prevPos - other.transform.position).magnitude;
                distance += treat;

                break;
            
        }
        
    }

    void OnCollisionStay(Collision collisionInfo){
        if(valid)
            if(collisionInfo.gameObject.layer == wallMask)
                distance -= trick*Time.deltaTime;
    }

    private void OnTriggerExit(Collider other){
        touching = false;
    }

    private void Update(){
        if(valid){
            float temp = (curPos - transform.position).sqrMagnitude;
            if(temp > temp_dist_sqr){
                temp_dist_sqr = temp;
            }
        }

    }

    public float GetDist(){
        if(valid)
            return distance + Mathf.Sqrt(temp_dist_sqr);
        return distance;
    }
}
