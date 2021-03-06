using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    CarNN nn;
    public float power = 1500f;
    public float brakeTorque;

    public float horInput;
    public float verInput;
    private float steerAngle;

    public WheelScript[] wheels;
    private int fc;
    private Vector2 output;
    private int ready_wheels = 0;
    void Start(){
        
        nn = GetComponent<CarNN>();
    }

    //NN Control
    // void FixedUpdate()
    // {
    //     if (ready_wheels == 4 && run)
    //     {
    //         ProcessForces();
    //     }

    // }

    public void wheel_is_ready()
    {
        ready_wheels++;
    }

    // void ProcessForces()
    // {
    //     output = nn.predict();
    //     verInput = (output[1] - 0.5f) * 2;
    //     horInput = (output[0] - 0.5f) * 2;
    //     foreach (WheelScript w in wheels)
    //     {
    //         w.Steer((output[0] - 0.5f) * 2);
    //         w.Accelerate((output[1] - 0.5f) * 2 * power, (output[1] - 0.5f) * 2 * brakeTorque);
    //         w.UpdatePosition();
    //     }
    // }

    IEnumerator ProcessForcesCoroutine(){
        while(true){
            output = nn.predict();
            verInput = (output[1] - 0.5f) * 2;
            horInput = (output[0] - 0.5f) * 2;
            foreach (WheelScript w in wheels)
            {
                w.Steer((output[0] - 0.5f) * 2);
                w.Accelerate((output[1] - 0.5f) * 2 * power, (output[1] - 0.5f) * 2 * brakeTorque);
                w.UpdatePosition();
            }
            for(int i = 0; i < fc; i ++){
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public void StopCar(){
        StopAllCoroutines();
        GetComponent<Rigidbody>().isKinematic = true;
    }
    public void StartCar(int offset, int frameCount){
        //if (ready_wheels != 4) { print(ready_wheels);  return; }
        fc = frameCount;
        StartCoroutine(StartCarCoroutine(offset));
    }

    IEnumerator StartCarCoroutine(int offset){
        while ((int)(Time.fixedTime / Time.fixedDeltaTime)%fc != offset || ready_wheels != 4){         
            yield return new WaitForFixedUpdate();
        }
        // print("Started");
        StartCoroutine(ProcessForcesCoroutine());
    }

    //Manual Control

    //void FixedUpdate()
    //{
    //    ProcessForces();
    //}

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
    //        w.Accelerate(verInput * power, verInput * brakeTorque);
    //        w.UpdatePosition();
    //    }
    //}
}
