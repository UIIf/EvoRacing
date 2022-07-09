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

    void Awake()
    {
        
        ViewAngles = new float[countOfRays];
        
        for(int i = 0; i < countOfRays; i++){
            ViewAngles[i] = FOVAngle * (0.5f  - i/(countOfRays - 1f))*Mathf.Deg2Rad;
             
        }
    }

    public float[] GetRaysInfo()
    {
        RaycastHit rayHit;
        float[] ret = new float[countOfRays];
        for(int i = 0; i < countOfRays; i++){
            Vector3 ViewVectro = Eye.right * Mathf.Sin(ViewAngles[i]) + Eye.forward * Mathf.Cos(ViewAngles[i]);
            if(Physics.Raycast(Eye.position, ViewVectro, out rayHit, lenOfRay, eyeLayerMask)){
                ret[i] = rayHit.distance/ lenOfRay;
            }
            else
            {
                ret[i] = 1f;
            }
        }
        return ret;
    }

    public int GetRaysCount() { return countOfRays; }
    //delete
    void Update(){
        float[] dists = GetRaysInfo();

        if(showRays){
            for(int i = 0; i < dists.Length; i++){
                Vector3 ViewVectro = Eye.right * Mathf.Sin(ViewAngles[i]) + Eye.forward * Mathf.Cos(ViewAngles[i]);
                Debug.DrawLine(Eye.position, Eye.position + ViewVectro * dists[i] * lenOfRay, Color.blue);
            }
        }
    }
}
