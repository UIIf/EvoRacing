using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour
{

    [SerializeField] CarManager carManag;
    [SerializeField] TrainingSettings trSettings;
    [SerializeField] Transform start;
    [SerializeField] GameObject carPrefab;

    [SerializeField] GameObject ScrollView;
    [SerializeField] GameObject carItem;
    [SerializeField] GameObject UITime;
    [SerializeField] GameObject UITimeScale;
    [SerializeField] GameObject saveWindow;
    [SerializeField] GameObject loadWindow;
    private Vector3 spawnPoint;
    [SerializeField] private float curTime;
    [SerializeField] private bool isStartedTraining;
    [SerializeField] private bool AutoStart = true;

    [Range(1f, 5f)]
    [SerializeField] float timeScale = 1f;
    [SerializeField] string defaultSlot = "main";

    private bool pauseCars = true;


    void Start()
    {
        isStartedTraining = false;
        GameObject[] cars = new GameObject[trSettings.carNum];
        spawnPoint = new Vector3(start.position.x, start.position.y + 1.3f, start.position.z);
        for (int i = 0; i < trSettings.carNum; i++)
        {
            cars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);

        }

        carManag.FillNewCars(cars);

        //Fill nn for new cars
        string temp_loaded = PlayerPrefs.GetString("SlotNN" + defaultSlot);
        if (temp_loaded == "")
        {
            Refresh();
        }
        else
        {
            LoadAndMutateNN(defaultSlot);
        }
        print("FinishStart");
        
    }

    void Update()
    {
        if (isStartedTraining && !pauseCars)
        {

            if (curTime >= trSettings.time)
            {
                curTime = trSettings.time;
                if (isStartedTraining)
                {
                    carManag.SaveMaxNN(defaultSlot);
                    isStartedTraining = false;
                    curTime = trSettings.time;
                }

                if (AutoStart)
                    startTrainingSession();
                else
                    Time.timeScale = 0;
            }
            else
            {
                curTime += Time.deltaTime;
            }
        }
    }

    void Training()
    {
        isStartedTraining = false;
        curTime = 0;
        GameObject[] cars = carManag.GetAllCars();
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

        int newCarNum = trSettings.carNum;
        GameObject[] newCars = new GameObject[trSettings.carNum];
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
            for (int i = 0; i < trSettings.from2Parents - 2; i++)
            {
                newCars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
                newCars[i].GetComponent<CarNN>().FillNN(NeuralNetwork.merge_mutate(p1W1, p2W1, trSettings.percentOfMutation, trSettings.mutationValue, priority), NeuralNetwork.merge_mutate(p1W2, p2W2, trSettings.percentOfMutation, trSettings.mutationValue, priority), NeuralNetwork.merge_mutate(p1W3, p2W3, trSettings.percentOfMutation, trSettings.mutationValue, priority));
            }
            // Mutated parents

            newCars[trSettings.from2Parents - 2] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[trSettings.from2Parents - 2].GetComponent<CarNN>().FillNN(p1W1, p1W2, p1W3);

            newCars[trSettings.from2Parents - 1] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[trSettings.from2Parents - 1].GetComponent<CarNN>().FillNN(p2W1, p2W2, p2W3);

            offset++;
            newCarNum -= trSettings.from2Parents;
        }

        //Main part of mutation 
        {
            // Childrens that into
            while (newCarNum / ((float)(trSettings.from2Parents)) >= 2)
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
                for (int i = offset * trSettings.from2Parents; i < (offset + 1) * trSettings.from2Parents - 2; i++)
                {
                    newCars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
                    newCars[i].GetComponent<CarNN>().FillNN(NeuralNetwork.merge_mutate(p1W1, p2W1, trSettings.percentOfMutation, trSettings.mutationValue, priority), NeuralNetwork.merge_mutate(p1W2, p2W2, trSettings.percentOfMutation, trSettings.mutationValue, priority), NeuralNetwork.merge_mutate(p1W3, p2W3, trSettings.percentOfMutation, trSettings.mutationValue, priority));
                }
                // Mutated parents

                newCars[(offset + 1) * trSettings.from2Parents - 2] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
                newCars[(offset + 1) * trSettings.from2Parents - 2].GetComponent<CarNN>().FillNN(p1W1, p1W2, p1W3);

                newCars[(offset + 1) * trSettings.from2Parents - 1] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
                newCars[(offset + 1) * trSettings.from2Parents - 1].GetComponent<CarNN>().FillNN(p2W1, p2W2, p2W3);

                offset++;
                newCarNum -= trSettings.from2Parents;
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
            for (int i = offset * trSettings.from2Parents; i < offset * trSettings.from2Parents + newCarNum - 2; i++)
            {
                newCars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);

                newCars[i].GetComponent<CarNN>().FillNN(NeuralNetwork.merge_mutate(p1W1, p2W1, trSettings.percentOfMutation, trSettings.mutationValue, priority), NeuralNetwork.merge_mutate(p1W2, p2W2, trSettings.percentOfMutation, trSettings.mutationValue, priority), NeuralNetwork.merge_mutate(p1W3, p2W3, trSettings.percentOfMutation, trSettings.mutationValue, priority));
            }

            // Mutated parents

            newCars[offset * trSettings.from2Parents + newCarNum - 2] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[offset * trSettings.from2Parents + newCarNum - 2].GetComponent<CarNN>().FillNN(p1W1, p1W2, p1W3);

            newCars[offset * trSettings.from2Parents + newCarNum - 1] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[offset * trSettings.from2Parents + newCarNum - 1].GetComponent<CarNN>().FillNN(p2W1, p2W2, p2W3);
        }
        carManag.FillNewCars(newCars);
    }

    public void changeAutoStart()
    {
        AutoStart = !AutoStart;
    }

    //VINESTY ---------------------------------------------------------------------
    public void CreateTable()
    {
        if (ScrollView.activeSelf)
        {
            ScrollView.SetActive(false);
            Transform temp = ScrollView.transform.GetChild(0).transform;
            for (int i = 0; i < temp.childCount; i++)
            {
                Destroy(temp.GetChild(0));
            }
        }
        else
        {
            GameObject[] cars = carManag.GetAllCars();
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

    }

    public void Refresh()
    {
        if (!isStartedTraining)
        {
            GameObject[] cars = carManag.GetAllCars();
            foreach(GameObject car in carManag.GetAllCars()){
                car.GetComponent<CarNN>().InitialiseNN();
            }
        }
    }


    public void startTrainingSession()
    {
        Time.timeScale = timeScale;

        if (pauseCars)
        {
            carManag.StartAllCars();
            pauseCars = false;
            isStartedTraining = true;
        }
        if (!isStartedTraining)
        {
            Training();
            carManag.StartAllCars();
            isStartedTraining = true;
            pauseCars = false;
            curTime = 0;
        }
        
    }

    //UI Time change
    public void IncreaseTime()
    {
        trSettings.ChangeTime(2.5f);
        RefreshUITime();
    }

    public void ReduceTime()
    {
        trSettings.ChangeTime(-2.5f);
        RefreshUITime();
    }

    private void RefreshUITime() { UITime.GetComponent<Text>().text = trSettings.time.ToString() + "s"; }

    //UI TimeScale change
    public void IncreaseTimeScale()
    {
        if (timeScale < 5)
            timeScale += 1f;
        RefreshUITimeScale();
        if (isStartedTraining)
            Time.timeScale = timeScale;
    }

    public void ReduceTimeScale()
    {
        if (timeScale > 1)
            timeScale -= 1f;
        RefreshUITimeScale();
        if (isStartedTraining)
            Time.timeScale = timeScale;
    }

    private void RefreshUITimeScale() { UITimeScale.GetComponent<Text>().text = timeScale.ToString() + "x"; }

    //Work with saving and loading NN

    public void LoadAndMutateNN(string slot)
    {
        if (!isStartedTraining)
        {
            
            carManag.LoadNNTo(slot,0);
            GameObject[] cars = carManag.GetAllCars();
            CarNN tempNN = cars[0].GetComponent<CarNN>();
            float[][] w1 = tempNN.getW1();
            float[][] w2 = tempNN.getW2();
            float[][] w3 = tempNN.getW3();

            for (int i = 1; i < cars.Length; i++)
            {
                cars[i].GetComponent<CarNN>().FillNN(NeuralNetwork.mutate(w1, trSettings.percentOfMutation, trSettings.mutationValue), NeuralNetwork.mutate(w2, trSettings.percentOfMutation, trSettings.mutationValue), NeuralNetwork.mutate(w3, trSettings.percentOfMutation, trSettings.mutationValue));
            }
        }

    }

    public void SubmitSave()
    {
        Text saveText = saveWindow.transform.Find("InputField").Find("Text").GetComponent<Text>();

        if (saveText.text != "" && saveText.text.Length < 10)
        {
            saveText.text.Replace(' ', '_');

            carManag.SaveMaxNN(saveText.text);
            SaveSlotName(saveText.text);
            saveText.text = "";
            SaveWindowSwitch();
        }
    }

    void BuildLoadWindow()
    {
        Transform content = loadWindow.transform.Find("Viewport").Find("Content");
        GameObject template = content.Find("Template").gameObject;

        for (int i = 0; i < loadWindow.transform.Find("Viewport").Find("Content").childCount; i++)
        {
            if (content.GetChild(i).name != "Template")
                Destroy(content.GetChild(i).gameObject);
        }

        string[] slots = PlayerPrefs.GetString("AllTheSlots").Split(' ');

        foreach (string element in slots)
        {
            if (element == "") continue;

            GameObject newElement = Instantiate(template, content);
            Text newElementNameText = newElement.transform.Find("SlotName").GetComponent<Text>();
            //Text newElementGenNumText = newElement.transform.Find("GenerationsNum").GetComponent<Text>();
            //Button newElementDeleteButton = newElement.transform.Find("DeleteButton").GetComponent<Button>();
            Button newElementButton = newElement.GetComponent<Button>();

            newElementNameText.text = element;
            newElementButton.onClick.AddListener(() => LoadAndMutateNN(element));
            newElementButton.onClick.AddListener(() => LoadWindowSwitch());

            newElement.SetActive(true);
        }
    }
    void SaveSlotName(string slot)
    {
        string str = PlayerPrefs.GetString("AllTheSlots");

        if (str == "")
            PlayerPrefs.SetString("AllTheSlots", slot);
        else
        {
            if (str.Contains(slot)) return;

            PlayerPrefs.SetString("AllTheSlots", str + " " + slot);
        }

    }
    public void SaveWindowSwitch()
    {

        if (saveWindow.activeSelf)
            saveWindow.SetActive(false);
        else
            saveWindow.SetActive(true);
    }
    public void LoadWindowSwitch()
    {

        if (loadWindow.activeSelf)
            loadWindow.SetActive(false);
        else
        {
            loadWindow.SetActive(true);
            BuildLoadWindow();
        }
    }

    public void ResetCarPosition()
    {
        foreach (GameObject car in carManag.GetAllCars())
        {
            car.transform.position = spawnPoint;
            car.transform.rotation = carPrefab.transform.rotation;
        }
    }

    public float GetMaxTime()
    {
        return trSettings.time;
    }
    public float GetCurrentTime()
    {
        return curTime;
    }

}
