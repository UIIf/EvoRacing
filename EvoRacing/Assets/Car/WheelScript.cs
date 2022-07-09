using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : MonoBehaviour
{
    public bool powered = false;
    public float maxAngle = 45f;
    public float offset = 0f;

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
        turnAngle = steerInput * maxAngle + offset;
        wcol.steerAngle = turnAngle;
    }

    public void Accelerate(float powerInput)
    {
        if (powered) wcol.motorTorque = powerInput;
        //else wcol.brakeTorque = 0;
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
