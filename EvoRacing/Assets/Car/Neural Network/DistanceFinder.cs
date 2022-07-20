using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFinder : MonoBehaviour
{
    [Tooltip("Treat that given to car when it collides with check point(i think it must be more then any distance between to chek points)")]
    [SerializeField] float treat;

    [Tooltip("Treat that given to car when it collides with finish")]
    [SerializeField] float finTreat = 50;

    [Tooltip("Score for second after finish")]
    [SerializeField] float scoreAfterFin = 15;

    [Tooltip("Penalty for crashing in the wall")]
    [SerializeField] float trick;
    [SerializeField] LayerMask wallMask;
    private Vector3 prevPos = Vector3.zero;
    [SerializeField] private Vector3 curPos = Vector3.zero;
    private Vector3 curNorm;
    [SerializeField] private float distance = 0;
    private float temp_dist_sqr = 0;
    [SerializeField] private bool valid = true;
    [SerializeField] private Material invalidMaterial;
    [SerializeField] private MeshRenderer carBody;
    private bool touching = false;

    private bool finished = false;

    private void ChangeMat(){
        carBody.material = invalidMaterial;
        print("Stop " + distance.ToString());
    }
    public void Refresh(){
        
    }

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
                    ChangeMat();
                    valid = false;
                    break;
                }

                if(prevPos == other.transform.position){
                    distance -= treat*2 ;
                    ChangeMat();
                    valid = false;
                }


                
                touching = true;
                prevPos = curPos;
                curPos = other.transform.position;
                curNorm = other.transform.forward;
                distance += (prevPos - other.transform.position).magnitude;
                distance += treat;

                break;

            case "Fin":
                distance += finTreat;
                finished = true;
            break;
            
        }
        
    }

    void OnCollisionStay(Collision collisionInfo){
        if(valid && !finished)
            if(collisionInfo.gameObject.layer == wallMask){
                distance -= trick*Time.deltaTime;
                if(distance < 0){
                    ChangeMat();
                    valid = false;
                }
            }
    }

    private void OnTriggerExit(Collider other){
        touching = false;
    }

    private void Update(){
        if(valid && !finished){
            float temp = (curPos - transform.position).sqrMagnitude;
            if(temp > temp_dist_sqr){
                temp_dist_sqr = temp;
            }
        }
        if(finished){
            distance += scoreAfterFin * Time.deltaTime;
        }
    }

    public float GetDist(){
        if(valid && !finished)
            return distance + Mathf.Sqrt(temp_dist_sqr);
        return distance;
    }
}
