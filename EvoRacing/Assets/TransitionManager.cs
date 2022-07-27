using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ToGarage()
    {
        SceneManager.LoadScene(1);
    }

    public void ToTraining()
    {
        SceneManager.LoadScene(2);
    }

    public void ToEditor()
    {
        SceneManager.LoadScene(3);
    }
}
