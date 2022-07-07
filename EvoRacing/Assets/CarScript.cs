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

    void Awake(){
        nn = GetComponent<CarNN>();
    }

    void FixedUpdate()
    {
        ProcessForces();
    }

    void ProcessForces()
    {
        Vector2 output = nn.predict();
        // Debug.Log((output[0 ]-0.5f * 2).ToString() +" : "+ (output[1 ]-0.5f * 2).ToString() );
        Debug.Log(((output[0 ]-0.5f) * 2).ToString() +" : "+ ((output[1 ]-0.5f) * 2).ToString() );
        foreach(WheelScript w in wheels)
        {
            w.Steer((output[0]-0.5f) * 2);
            w.Accelerate((output[1]-0.5f) * 2 * power);
            w.UpdatePosition();
        }
    }

    // void Update()
    // {
    //     ProcessInput();
    // }

    // void FixedUpdate()
    // {
    //     ProcessForces();
    // }

    // void ProcessInput()
    // {
    //     verInput = Input.GetAxis("Vertical");
    //     horInput = Input.GetAxis("Horizontal");
    // }

    // void ProcessForces()
    // {
    //     foreach(WheelScript w in wheels)
    //     {
    //         w.Steer(horInput);
    //         w.Accelerate(verInput * power);
    //         w.UpdatePosition();
    //     }
    // }
}
