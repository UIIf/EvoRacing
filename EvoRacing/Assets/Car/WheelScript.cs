using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : MonoBehaviour
{
    public bool powered = false;
    public float maxAngle = 45f;

    private float turnAngle;
    private WheelCollider wcol;
    private Transform wmesh;

    void Start()
    {
        wcol = GetComponentInChildren<WheelCollider>();
        wmesh = transform.Find("wheel_mesh");
        gameObject.transform.parent.GetComponent<CarScript>().wheel_is_ready();
    }

    public void Steer(float steerInput)
    {
        turnAngle = steerInput * maxAngle;
        wcol.steerAngle = turnAngle;
    }

    public void Accelerate(float powerInput, float brakeTorque)
    {
        if(powerInput > 0)
        {
            wcol.brakeTorque = 0;
            if(powered) wcol.motorTorque = powerInput;
        }
        else
            wcol.brakeTorque = -brakeTorque + 0.001f;

    }

    public void UpdatePosition()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        wcol.GetWorldPose(out pos, out rot);
        wmesh.transform.position = pos;
        wmesh.transform.rotation = rot;
    }
}
