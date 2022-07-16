using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
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
}
