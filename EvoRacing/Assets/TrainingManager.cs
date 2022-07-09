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

    [Min(2)]
    [SerializeField] int fromTwoParents = 5;

    [Range(0f, 1f)]
    [SerializeField] float persrntOfMutation = 0.1f;

    [SerializeField] float mutationValue = 0.5f;

    [DelayedAttribute]
    [SerializeField] float time = 60;
    [SerializeField ]private float curTime;
    [SerializeField]private bool startTrainingSession;
    void Start()
    {
        startTrainingSession = false;
        cars = new GameObject[carNum];
        spawnPoint = new Vector3(start.position.x, start.position.y + 1.3f, start.position.z);
        for(int i = 0; i < carNum; i++)
        {
            cars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            cars[i].GetComponent<CarNN>().InitialiseNN();
        }
        
        for(int i = 0; i < carNum; i++)
        {
            cars[i].GetComponent<CarScript>().run = true;
        }
        startTrainingSession  = true;
        curTime = 0;
    }

    void Training(){
        startTrainingSession = false;
        float[] scors = new float[cars.Length];
        string output = "Scores : ";
        for(int i = 0; i < cars.Length; i++){
            scors[i] = cars[i].GetComponent<DistanceFinder>().GetDist();
            output +=  scors[i].ToString() + " ";
        }

        print(output);
        


        int newCarNum = carNum;
        GameObject[] newCars = new GameObject[carNum];
        int offset = 0;
        float priority;
        float[][] maxW1, maxW2, semi_maxW1, semi_maxW2;
        int max_, semi_max;
        // Childrens that into
        while(newCarNum/((float)(fromTwoParents)) >= 2){

            // Find new parents
            
            if(scors[0] > scors[1]){
                max_ = 0;
                semi_max = 1;
            }
            else{
                max_ = 1;
                semi_max = 0;
            }
            for(int i = 2; i < scors.Length; i++){
                if(scors[i] > scors[max_]){
                    semi_max = max_;
                    max_ = i;
                }
                else if(scors[i] > scors[semi_max]){
                    semi_max = i;
                }
            }

            print(max_.ToString() + " : " +scors[max_].ToString() + "; " + semi_max.ToString()+ " : " +scors[semi_max].ToString() );

            

            // Current parent weights
            maxW1 = cars[max_].GetComponent<CarNN>().getW1();
            maxW2 = cars[max_].GetComponent<CarNN>().getW2();

            semi_maxW1 = cars[semi_max].GetComponent<CarNN>().getW1();
            semi_maxW2 = cars[semi_max].GetComponent<CarNN>().getW2();

            // Mutated childrens

            priority = scors[max_]/(scors[max_] + scors[semi_max]);
            for(int i = offset*fromTwoParents; i < (offset + 1)*fromTwoParents - 2; i++){
                newCars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
                newCars[i].GetComponent<CarNN>().FillNN(NeuralNetwork.merege_mutate(maxW1, semi_maxW1, persrntOfMutation, mutationValue, priority), NeuralNetwork.merege_mutate(maxW2, semi_maxW2, persrntOfMutation, mutationValue, priority));
            }
            scors[max_] = -100000000000000;
            scors[semi_max] = -100000000000000;
            // Mutated parents
            
            newCars[(offset + 1)*fromTwoParents - 2] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[(offset + 1)*fromTwoParents - 2].GetComponent<CarNN>().FillNN(NeuralNetwork.mutate(maxW1, persrntOfMutation, mutationValue), NeuralNetwork.mutate(maxW2, persrntOfMutation, mutationValue));

            newCars[(offset + 1)*fromTwoParents - 1] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[(offset + 1)*fromTwoParents - 1].GetComponent<CarNN>().FillNN(NeuralNetwork.mutate(maxW1, persrntOfMutation, mutationValue), NeuralNetwork.mutate(maxW2, persrntOfMutation, mutationValue));

            offset ++;
            newCarNum -= fromTwoParents;
        }

        //Other childrens

        if(scors[0] > scors[1]){
            max_ = 0;
            semi_max = 1;
        }
        else{
            max_ = 1;
            semi_max = 0;
        }
        for(int i = 2; i < scors.Length; i++){
            if(scors[i] > max_){
                semi_max = max_;
                max_ = i;
            }
        }

        print(max_.ToString() + " : " +scors[max_].ToString() + "; " + semi_max.ToString()+ " : " +scors[semi_max].ToString() );

        maxW1 = cars[max_].GetComponent<CarNN>().getW1();
        maxW2 = cars[max_].GetComponent<CarNN>().getW2();

        semi_maxW1 = cars[semi_max].GetComponent<CarNN>().getW1();
        semi_maxW2 = cars[semi_max].GetComponent<CarNN>().getW2();

        // Mutated childrens
        priority = scors[max_]/(scors[max_] + scors[semi_max]);
        for(int i = offset*fromTwoParents; i < offset*fromTwoParents  + newCarNum - 2; i++){
            newCars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);

            newCars[i].GetComponent<CarNN>().FillNN(NeuralNetwork.merege_mutate(maxW1, semi_maxW1, persrntOfMutation, mutationValue, priority), NeuralNetwork.merege_mutate(maxW2, semi_maxW2, persrntOfMutation, mutationValue,priority));
        }
        
        // Mutated parents
        
        newCars[offset*fromTwoParents  + newCarNum - 2] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
        newCars[offset*fromTwoParents  + newCarNum - 2].GetComponent<CarNN>().FillNN(maxW1, maxW2);

        newCars[offset*fromTwoParents  + newCarNum - 1] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
        newCars[offset*fromTwoParents  + newCarNum - 1].GetComponent<CarNN>().FillNN(maxW1, maxW2);

        for(int i = 0; i < cars.Length; i++){
            Destroy( cars[i]);
            cars[i] = newCars[i];
        }


        for(int i = 0; i < carNum; i++)
        {
            cars[i].GetComponent<CarScript>().run = true;
        }

        startTrainingSession  = true;
        curTime = 0;
    }

    void Update()
    {
        if(startTrainingSession){
            curTime += Time.deltaTime;
            if(curTime > time){
                Training();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
