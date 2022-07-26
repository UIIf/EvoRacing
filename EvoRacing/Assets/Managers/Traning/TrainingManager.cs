using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

enum TrainingState{
    beforeTraining,
    setNN,
    onTraining,
    endTraining
}

public class TrainingManager : MonoBehaviour
{

    [SerializeField] CarManager carManag;
    [SerializeField] TrainingSettings trSettings;
    [SerializeField] DFSettings dFSettings;
    [SerializeField] MainManager MM;
    [SerializeField] Transform start;
    [SerializeField] GameObject carPrefab;

    [SerializeField] GameObject ScrollView;
    [SerializeField] GameObject carItem;
    [SerializeField] GameObject UITime;
    [SerializeField] GameObject saveWindow;
    [SerializeField] GameObject loadWindow;
    private Vector3 spawnPoint;
    [SerializeField] private float curTime;
    [SerializeField] private bool AutoStart = true;
    [SerializeField] string defaultSlot = "main";

    [SerializeField] TrainingState trState = TrainingState.beforeTraining;


    void Start()
    {
        carPrefab.GetComponent<DistanceFinder>().dFSettings = dFSettings;

        //Initialise cars for car manager
        GameObject[] cars = new GameObject[trSettings.carNum];
        spawnPoint = new Vector3(start.position.x, start.position.y + 1.3f, start.position.z);
        for (int i = 0; i < trSettings.carNum; i++)
        {
            cars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            // cars[i].GetComponent<DistanceFinder>().SetDFSettings(dFSettings);
        }

        carManag.FillNewCars(cars);

        //Fill nn for new cars
        string temp_loaded = PlayerPrefs.GetString("SlotNN" + defaultSlot);
        if (temp_loaded == "")
        {
            RefreshNN();
        }
        else
        {
            LoadAndMutateNN(defaultSlot);
        }
        trState = TrainingState.beforeTraining;
    }

    void Update()
    {
        if(trState == TrainingState.onTraining){
            if (curTime >= trSettings.time)
            {
                StopTrainingSession();
            }
            else
            {
                curTime += Time.deltaTime;
            }
        }
    }

    void Training()
    {
        trState = TrainingState.setNN;
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
        trState = TrainingState.onTraining;
    }

    public void changeAutoStart()
    {
        AutoStart = !AutoStart;
    }

    //VINESTY ---------------------------------------------------------------------
    // public void CreateTable()
    // {
    //     if (ScrollView.activeSelf)
    //     {
    //         ScrollView.SetActive(false);
    //         Transform temp = ScrollView.transform.GetChild(0).transform;
    //         for (int i = 0; i < temp.childCount; i++)
    //         {
    //             Destroy(temp.GetChild(0));
    //         }
    //     }
    //     else
    //     {
    //         GameObject[] cars = carManag.GetAllCars();
    //         ScrollView.SetActive(true);
    //         Transform content = ScrollView.transform.GetChild(0).transform.GetChild(0);
    //         cars = cars.OrderBy((car) => car.GetComponent<DistanceFinder>().GetDist()).Reverse<GameObject>().ToArray<GameObject>();
    //         foreach (GameObject car in cars)
    //         {
    //             GameObject temp = Instantiate(carItem, content);
    //             temp.transform.GetChild(1).GetComponent<Text>().text = "Score: " + car.GetComponent<DistanceFinder>().GetDist().ToString("0,000");
    //             temp.transform.parent = content;
    //         }
    //     }

    // }

    public void RefreshNN()
    {
        if (trState == TrainingState.endTraining || trState == TrainingState.beforeTraining)
        {
            trState = TrainingState.setNN;
            carManag.ApplyToAll(new CarManager.CarAction(x =>x.GetComponent<CarNN>().InitialiseNN()));
        }
    }


    public void startTrainingSession()
    {
        MM.UnPauseGame();

        if(trState == TrainingState.beforeTraining){
            carManag.StartAllCars();
        }
        else if(trState == TrainingState.endTraining){
            Training();
            carManag.StartAllCars();
            curTime = 0;
        }
        trState = TrainingState.onTraining;
    }

    public void StopTrainingSession(){
        if(trState == TrainingState.onTraining){
            carManag.SaveMaxNN(defaultSlot);
            carManag.StopAllCars();
            curTime = trSettings.time;
            trState = TrainingState.endTraining;
            if (AutoStart)
                startTrainingSession();
            else{
                carManag.StopAllCars();
            }  
        }
        
    }

    public float GetCurrentTime()
    {
        return curTime;
    }

    //PERENESTI V DRUGOI SCRIPT

    public void LoadAndMutateNN(string slot)
    {
        
        if (trState == TrainingState.endTraining || trState == TrainingState.beforeTraining)
        {
            trState = TrainingState.setNN;
            carManag.LoadNNTo(slot, 0);
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
        InputField saveText = saveWindow.transform.Find("InputField").GetComponent<InputField>();

        if (saveText.text != "" && saveText.text.Length < 10)
        {
            string slot = saveText.text.Replace(' ', '_');
            saveText.text = "";
            carManag.SaveMaxNN(slot);
            SaveSlotName(slot);
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
            Button newElementDeleteButton = newElement.transform.Find("DeleteButton").GetComponent<Button>();
            Button newElementButton = newElement.GetComponent<Button>();

            newElementNameText.text = element;
            newElementButton.onClick.AddListener(() => LoadAndMutateNN(element));
            newElementButton.onClick.AddListener(() => LoadWindowSwitch());

            newElementDeleteButton.onClick.AddListener(() => DeleteSlot(element));

            newElement.SetActive(true);
        }
    }
    void SaveSlotName(string slot)
    {
        string str = PlayerPrefs.GetString("AllTheSlots");

        if (str == "")
            PlayerPrefs.SetString("AllTheSlots", slot + " ");
        else
        {
            if (str.Contains(slot + " ")) return;

            PlayerPrefs.SetString("AllTheSlots", str + slot + " ");
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

    public void DeleteSlot(string element){
        

        PlayerPrefs.DeleteKey(element);

        string slots = PlayerPrefs.GetString("AllTheSlots");
        

        PlayerPrefs.SetString("AllTheSlots", slots.Remove(slots.IndexOf(element), element.Length + 1));
        
        BuildLoadWindow();
    }
}
