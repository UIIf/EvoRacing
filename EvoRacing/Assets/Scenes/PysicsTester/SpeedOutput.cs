using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedOutput : MonoBehaviour
{
    public GameObject car;
    Rigidbody carRb;
    Text text;
    void Start()
    {
        carRb = car.GetComponent<Rigidbody>();
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = carRb.velocity.ToString();
    }
}
