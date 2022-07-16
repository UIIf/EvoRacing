using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour
{
    public GameObject togo;
    TimeOutput to;

    private void Start()
    {
        to = togo.GetComponent<TimeOutput>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "car")
            to.Stop();

    }
}
