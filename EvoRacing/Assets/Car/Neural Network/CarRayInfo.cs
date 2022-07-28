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
    private Vector2[] SinCosAngle;

    void Awake()
    {
        
        ViewAngles = new float[countOfRays];
        SinCosAngle = new Vector2[countOfRays];
        
        for(int i = 0; i < countOfRays; i++){
            ViewAngles[i] = FOVAngle * (0.5f  - i/(countOfRays - 1f))*Mathf.Deg2Rad;
            SinCosAngle[i] = new Vector2(Mathf.Sin(ViewAngles[i]), Mathf.Cos(ViewAngles[i]));
        }
    }

    public float[] GetRaysInfo()
    {
        RaycastHit rayHit;
        float[] ret = new float[countOfRays];

        for(int i = 0; i < countOfRays; i++){
            Vector3 ViewVectro = Eye.right * SinCosAngle[i][0] + Eye.forward * SinCosAngle[i][1];
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
        
        if(showRays){
            float[] dists = GetRaysInfo();
            for(int i = 0; i < dists.Length; i++){
                Vector3 ViewVectro = Eye.right * SinCosAngle[i][0] + Eye.forward * SinCosAngle[i][1];
                Debug.DrawLine(Eye.position, Eye.position + ViewVectro * dists[i] * lenOfRay, Color.blue);
            }
        }
    }
}
