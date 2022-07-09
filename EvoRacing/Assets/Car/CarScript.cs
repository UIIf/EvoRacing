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

    public bool run = false;
    private int ready_wheels = 0;
    void Start(){
        
        nn = GetComponent<CarNN>();
    }

    void FixedUpdate()
    {
        if(ready_wheels == 4 && run){
            ProcessForces();
        }
            
    }

    public void wheel_is_ready(){
        ready_wheels++;
    }

    void ProcessForces()
    {        
        Vector2 output = nn.predict();
        verInput = (output[1] - 0.5f) * 2;
        horInput = (output[0] - 0.5f) * 2;
        foreach (WheelScript w in wheels)
        {
            w.Steer((output[0] - 0.5f) * 2);
            w.Accelerate((output[1] - 0.5f) * 2 * power);
            w.UpdatePosition();
        }
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
