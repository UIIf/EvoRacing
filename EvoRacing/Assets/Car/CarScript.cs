using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    CarNN nn;
    public float power = 1500f;

    public float horInput;
    public float verInput;
    private float steerAngle;

    public WheelScript[] wheels;

    void Start(){
        
        nn = GetComponent<CarNN>();
    }

    void FixedUpdate()
    {
        ProcessForces();
    }

    void ProcessForces()
    {
        try
        {
            Vector2 output = nn.predict();
            // Debug.Log((output[0 ]-0.5f * 2).ToString() +" : "+ (output[1 ]-0.5f * 2).ToString() );
            //Debug.Log(((output[0]-0.5f) * 2).ToString() +" : "+ ((output[1]-0.5f) * 2).ToString());
            verInput = (output[1] - 0.5f) * 2;
            horInput = (output[0] - 0.5f) * 2;
            foreach (WheelScript w in wheels)
            {
                w.Steer((output[0] - 0.5f) * 2);
                w.Accelerate((output[1] - 0.5f) * 2 * power);
                w.UpdatePosition();
            }
        }
        catch { }
    }

    //void Update()
    //{
    //    ProcessInput();
    //}

    //void ProcessInput()
    //{
    //    verInput = Input.GetAxis("Vertical");
    //    horInput = Input.GetAxis("Horizontal");
    //}

    //void ProcessForces()
    //{
    //    foreach (WheelScript w in wheels)
    //    {
    //        w.Steer(horInput);
    //        w.Accelerate(verInput * power);
    //        w.UpdatePosition();
    //    }
    //}
}
