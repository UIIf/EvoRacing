using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] GameObject[] cars;
    [SerializeField] Transform start;
    [SerializeField] GameObject carPrefab;
    private Vector3 spawnPoint;

    [Min(1)]
    [SerializeField] int carNum = 10;
    void Start()
    {
        cars = new GameObject[carNum];
        spawnPoint = new Vector3(start.position.x, start.position.y + 1.3f, start.position.z);
        for(int i = 0; i < carNum; i++)
        {
            cars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            
        }
        
        for(int i = 0; i < carNum; i++)
        {
            cars[i].GetComponent<CarScript>().run = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
