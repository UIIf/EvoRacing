using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    public float power = 1500f;

    public float horInput;
    public float verInput;
    private float steerAngle;

    public WheelScript[] wheels;

    void Update()
    {
        ProcessInput();
    }

    void FixedUpdate()
    {
        ProcessForces();
    }

    void ProcessInput()
    {
        verInput = Input.GetAxis("Vertical");
        horInput = Input.GetAxis("Horizontal");
    }

    void ProcessForces()
    {
        foreach(WheelScript w in wheels)
        {
            w.Steer(horInput);
            w.Accelerate(verInput * power);
            w.UpdatePosition();
        }
    }
}
