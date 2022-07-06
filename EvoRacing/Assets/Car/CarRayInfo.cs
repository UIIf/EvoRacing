using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRayInfo : MonoBehaviour
{
    [Header("FOV")]
    [Range(0f, 360f)]
    [SerializeField] float FOVAngle;
    [SerializeField] int countOfRays;
    [SerializeField] float lenOfRay;
    [SerializeField] Transform Eye;

    //DELETE LATER(or make feature)
    [SerializeField] bool showRays = false;

    [SerializeField] LayerMask eyeLayerMask;

    private float[] ViewAngles;

    void Start()
    {
        ViewAngles = new float[countOfRays];
        
        for(int i = 0; i < countOfRays; i++){
            ViewAngles[i] = (FOVAngle * 0.5f  - FOVAngle *i/(countOfRays - 1))*Mathf.Deg2Rad;
             
        }
    }

    float[] GetRaysInfo()
    {
        RaycastHit rayHit;
        float[] ret = new float[countOfRays];
        for(int i = 0; i < countOfRays; i++){
            Vector3 ViewVectro = Eye.right * Mathf.Sin(ViewAngles[i]) + Eye.forward * Mathf.Cos(ViewAngles[i]);
            if(Physics.Raycast(Eye.position, ViewVectro, out rayHit, lenOfRay, eyeLayerMask)){
                if(showRays)
                    Debug.DrawLine(Eye.position, rayHit.point, Color.blue);
                ret[i] = rayHit.distance/ lenOfRay;
            }
            else
            {
                if(showRays)
                    Debug.DrawLine(Eye.position, Eye.position + ViewVectro * lenOfRay, Color.blue);
                ret[i] = 1f;
            }
        }
        return ret;
    }

    void Update(){
        GetRaysInfo();
    }
}
