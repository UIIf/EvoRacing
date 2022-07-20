using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] GameObject[] cars;
    [SerializeField] Transform start;
    [SerializeField] GameObject carPrefab;

    [SerializeField] GameObject ScrollView;
    [SerializeField] GameObject carItem;
    [SerializeField] GameObject UITime;
    private Vector3 spawnPoint;

    [Min(1)]
    [SerializeField] int carNum = 10;

    [Min(2)]
    [SerializeField] int fromTwoParents = 5;

    [Range(0f, 1f)]
    [SerializeField] float percentOfMutation = 0.1f;

    // [Range(0f, 0.5f)]
    [SerializeField] float mutationValue = 0.5f;

    [DelayedAttribute]
    [SerializeField] float time = 60;
    [SerializeField] private float curTime;
    [SerializeField] private bool isStartedTraining;
    [SerializeField] private bool AutoStart = true;

    [Range(1f, 5f)]
    [SerializeField] float timeScale = 1f;
    [SerializeField] string defaultSlot = "quick";

    private bool pauseCars = true;

    public float GetMaxTime()
    {
        return time;
    }
    public float GetCurrentTime()
    {
        return curTime;
    }
    void Start()
    {
        isStartedTraining = false;
        cars = new GameObject[carNum];
        spawnPoint = new Vector3(start.position.x, start.position.y + 1.3f, start.position.z);
        for (int i = 0; i < carNum; i++)
        {
            cars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);

        }
        string temp_loaded = PlayerPrefs.GetString("SlotNN" + defaultSlot);
        if(temp_loaded == ""){
            Refresh();
        }
        else{
            print("Good load, bro");
            print(temp_loaded);
            LoadNeurons(defaultSlot);
        }
    }


    void Training()
    {
        isStartedTraining = false;
        curTime = 0;
        // float[] scores = new float[cars.Length];
        List<Vector2> scores = new List<Vector2>();
        float total_score = 0;
        float temp;
        // string output = "Scores : ";
        for (int i = 0; i < cars.Length; i++)
        {
            temp = cars[i].GetComponent<DistanceFinder>().GetDist();
            scores.Add(new Vector2(temp * temp, i));
            total_score += scores[i][0];
            // output += scores[i][0].ToString() + " ";
        }

        // print(output);

        //Shuffle
        scores = scores.OrderBy(x => Random.value).ToList();

        int newCarNum = carNum;
        GameObject[] newCars = new GameObject[carNum];
        int offset = 0;
        float priority, parent, temp_sum;
        float[][] p1W1, p1W2, p1W3, p2W1, p2W2, p2W3;
        int parent1, parent2;


        //First best parents
        {
            float p1Score;
            float p2Score;
            if (scores[0][0] > scores[1][0])
            {
                parent1 = (int)scores[0][1];
                p1Score = scores[0][0];
                parent2 = (int)scores[1][1];
                p2Score = scores[1][0];
            }
            else
            {
                parent1 = (int)scores[1][1];
                p1Score = scores[1][0];
                parent2 = (int)scores[0][1];
                p2Score = scores[0][0];
            }
            for (int i = 2; i < scores.Count; i++)
            {
                if (scores[i][0] > p1Score)
                {
                    parent2 = parent1;
                    p2Score = p1Score;
                    parent1 = (int)scores[i][1];
                    p1Score = scores[i][0];
                }
                else if (scores[i][0] > p2Score)
                {
                    parent2 = (int)scores[i][1];
                    p2Score = scores[i][0];
                }
            }

            // print(p1Score);

            // Current parent weights
            p1W1 = cars[parent1].GetComponent<CarNN>().getW1();
            p1W2 = cars[parent1].GetComponent<CarNN>().getW2();
            p1W3 = cars[parent1].GetComponent<CarNN>().getW3();

            p2W1 = cars[parent2].GetComponent<CarNN>().getW1();
            p2W2 = cars[parent2].GetComponent<CarNN>().getW2();
            p2W3 = cars[parent1].GetComponent<CarNN>().getW3();


            priority = 0.5f;
            for (int i = 0; i < fromTwoParents - 2; i++)
            {
                newCars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
                newCars[i].GetComponent<CarNN>().FillNN(NeuralNetwork.merge_mutate(p1W1, p2W1, percentOfMutation, mutationValue, priority), NeuralNetwork.merge_mutate(p1W2, p2W2, percentOfMutation, mutationValue, priority), NeuralNetwork.merge_mutate(p1W3, p2W3, percentOfMutation, mutationValue, priority));
            }
            // Mutated parents

            newCars[fromTwoParents - 2] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[fromTwoParents - 2].GetComponent<CarNN>().FillNN(p1W1, p1W2, p1W3);

            newCars[fromTwoParents - 1] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[fromTwoParents - 1].GetComponent<CarNN>().FillNN(p2W1, p2W2, p2W3);

            offset++;
            newCarNum -= fromTwoParents;
        }

        //Main part of mutation 
        {
            // Childrens that into
            while (newCarNum / ((float)(fromTwoParents)) >= 2)
            {


                // Find new parents
                parent = Random.Range(0f, total_score);
                temp_sum = 0;
                parent1 = 0;
                for (int i = 0; i < scores.Count; i++)
                {
                    if (temp_sum < parent && scores[i][0] + temp_sum >= parent)
                    {
                        parent1 = (int)(scores[i][1]);
                        // total_score -= scores[i][0];
                        // scores.Remove(scores[i]);
                        break;
                    }
                    temp_sum += scores[i][0];
                }


                parent = Random.Range(0f, total_score);
                temp_sum = 0;
                parent2 = 0;
                for (int i = 0; i < scores.Count; i++)
                {
                    if (temp_sum < parent && scores[i][0] + temp_sum >= parent)
                    {
                        parent2 = (int)(scores[i][1]);
                        // total_score -= scores[i][0];
                        // scores.Remove(scores[i]);
                        break;
                    }
                    temp_sum += scores[i][0];
                }


                // Current parent weights
                p1W1 = cars[parent1].GetComponent<CarNN>().getW1();
                p1W2 = cars[parent1].GetComponent<CarNN>().getW2();
                p1W3 = cars[parent1].GetComponent<CarNN>().getW3();

                p2W1 = cars[parent2].GetComponent<CarNN>().getW1();
                p2W2 = cars[parent2].GetComponent<CarNN>().getW2();
                p2W3 = cars[parent1].GetComponent<CarNN>().getW3();

                // Mutated childrens

                priority = 0.5f;
                for (int i = offset * fromTwoParents; i < (offset + 1) * fromTwoParents - 2; i++)
                {
                    newCars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
                    newCars[i].GetComponent<CarNN>().FillNN(NeuralNetwork.merge_mutate(p1W1, p2W1, percentOfMutation, mutationValue, priority), NeuralNetwork.merge_mutate(p1W2, p2W2, percentOfMutation, mutationValue, priority), NeuralNetwork.merge_mutate(p1W3, p2W3, percentOfMutation, mutationValue, priority));
                }
                // Mutated parents

                newCars[(offset + 1) * fromTwoParents - 2] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
                newCars[(offset + 1) * fromTwoParents - 2].GetComponent<CarNN>().FillNN(p1W1, p1W2, p1W3);

                newCars[(offset + 1) * fromTwoParents - 1] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
                newCars[(offset + 1) * fromTwoParents - 1].GetComponent<CarNN>().FillNN(p2W1, p2W2, p2W3);

                offset++;
                newCarNum -= fromTwoParents;
            }
        }

        //Other childrens
        {

            parent = Random.Range(0f, total_score);
            temp_sum = 0;
            parent1 = 0;
            for (int i = 0; i < scores.Count; i++)
            {
                if (temp_sum < parent && scores[i][0] + temp_sum >= parent)
                {
                    parent1 = (int)(scores[i][1]);
                    // total_score -= scores[i][0];
                    // scores.Remove(scores[i]);
                    break;
                }
                temp_sum += scores[i][0];
            }


            parent = Random.Range(0f, total_score);
            temp_sum = 0;
            parent2 = 0;
            for (int i = 0; i < scores.Count; i++)
            {
                if (temp_sum < parent && scores[i][0] + temp_sum >= parent)
                {
                    parent2 = (int)(scores[i][1]);
                    // total_score -= scores[i][0];
                    // scores.Remove(scores[i]);
                    break;
                }
                temp_sum += scores[i][0];
            }

            // print(max_.ToString() + " : " + scores[max_].ToString() + "; " + semi_max.ToString()+ " : " + scores[semi_max].ToString() );

            p1W1 = cars[parent1].GetComponent<CarNN>().getW1();
            p1W2 = cars[parent1].GetComponent<CarNN>().getW2();
            p1W3 = cars[parent1].GetComponent<CarNN>().getW3();

            p2W1 = cars[parent2].GetComponent<CarNN>().getW1();
            p2W2 = cars[parent2].GetComponent<CarNN>().getW2();
            p2W3 = cars[parent1].GetComponent<CarNN>().getW3();

            // Mutated childrens
            priority = 0.5f;
            for (int i = offset * fromTwoParents; i < offset * fromTwoParents + newCarNum - 2; i++)
            {
                newCars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);

                newCars[i].GetComponent<CarNN>().FillNN(NeuralNetwork.merge_mutate(p1W1, p2W1, percentOfMutation, mutationValue, priority), NeuralNetwork.merge_mutate(p1W2, p2W2, percentOfMutation, mutationValue, priority), NeuralNetwork.merge_mutate(p1W3, p2W3, percentOfMutation, mutationValue, priority));
            }

            // Mutated parents

            newCars[offset * fromTwoParents + newCarNum - 2] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[offset * fromTwoParents + newCarNum - 2].GetComponent<CarNN>().FillNN(p1W1, p1W2, p1W3);

            newCars[offset * fromTwoParents + newCarNum - 1] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[offset * fromTwoParents + newCarNum - 1].GetComponent<CarNN>().FillNN(p2W1, p2W2, p2W3);
        }

        for (int i = 0; i < cars.Length; i++)
        {
            Destroy(cars[i]);
        }

        cars = new GameObject[carNum];
        for (int i = 0; i < carNum; i++)
        {
            cars[i] = newCars[i];
        }

    }


    public void pauseTrainingSession()
    {
        if (isStartedTraining && !pauseCars)
        {
            foreach (GameObject car in cars)
            {
                car.GetComponent<CarScript>().run = false;
            }
            pauseCars = true;
            // isStartedTraining  = true;
            // curTime = 0;
        }
    }

    public void changeAutoStart()
    {
        AutoStart = !AutoStart;
    }

    [ContextMenu("CreateTable")]
    public void CreateTable()
    {
        pauseTrainingSession();
        ScrollView.SetActive(true);
        Transform content = ScrollView.transform.GetChild(0).transform.GetChild(0);
        cars = cars.OrderBy((car) => car.GetComponent<DistanceFinder>().GetDist()).Reverse<GameObject>().ToArray<GameObject>();
        foreach (GameObject car in cars)
        {
            GameObject temp = Instantiate(carItem, content);
            temp.transform.GetChild(1).GetComponent<Text>().text = "Score: " + car.GetComponent<DistanceFinder>().GetDist().ToString("0,000");
            temp.transform.parent = content;
        }
    }

    public void Refresh(){
        if(!isStartedTraining){
            for (int i = 0; i < carNum; i++)
            {
                cars[i].GetComponent<CarNN>().InitialiseNN();
            }
        }
    }
    void AutoSave(string slot)
    {
        float maxScore = cars[0].GetComponent<DistanceFinder>().GetDist();
        int ind = 0;
        float temp;
        for (int i = 1; i < cars.Length; i++)
        {
            temp = cars[i].GetComponent<DistanceFinder>().GetDist();
            if (temp > maxScore)
            {
                maxScore = temp;
                ind = i;
            }
        }
        cars[ind].GetComponent<CarNN>().SaveNN(slot);
    }

    public void LoadNeurons(string slot)
    {
        if (!isStartedTraining)
        {

            CarNN tempNN = cars[0].GetComponent<CarNN>();
            tempNN.LoadNN(slot);
            float[][] w1 = tempNN.getW1();
            float[][] w2 = tempNN.getW2();
            float[][] w3 = tempNN.getW3();

            for (int i = 1; i < cars.Length; i++)
            {
                cars[i].GetComponent<CarNN>().FillNN(NeuralNetwork.mutate(w1, percentOfMutation, mutationValue), NeuralNetwork.mutate(w2, percentOfMutation, mutationValue), NeuralNetwork.mutate(w3, percentOfMutation, mutationValue));
            }
        }

    }
    void Update()
    {
        // Time.timeScale = timeScale;

        if (isStartedTraining && !pauseCars)
        {

            if (curTime > time)
            {
                if (isStartedTraining)
                {
                    AutoSave(defaultSlot);
                    isStartedTraining = false;
                    curTime = time;
                    Time.timeScale = 0;
                }

                if (AutoStart)
                    startTrainingSession();
            }
            else
            {
                curTime += Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    [ContextMenu("Start")]
    public void startTrainingSession()
    {
        if (pauseCars)
        {
            foreach (GameObject car in cars)
            {
                car.GetComponent<CarScript>().run = true;
            }
            pauseCars = false;
            isStartedTraining = true;
            Time.timeScale = timeScale;
        }
        if (!isStartedTraining)
        {
            print("OH NO, CRINGE");
            Training();
            foreach (GameObject car in cars)
            {
                car.GetComponent<CarScript>().run = true;
            }
            isStartedTraining = true;
            pauseCars = false;
            curTime = 0;
            Time.timeScale = timeScale;
        }


    }

    public void IncreaseTime()
    {
        if (time < 180)
            time += 2.5f;
        RefreshUITime();
    }

    public void ReduceTime()
    {
        if (time > 5)
            time -= 2.5f;
        RefreshUITime();
    }

    private void RefreshUITime() { UITime.GetComponent<Text>().text = time.ToString() + "s"; }
}
