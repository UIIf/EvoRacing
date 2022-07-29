using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

enum TrainingState{
    setNN,
    onTraining,
    endTraining,
    pauseTraining
}

public class TrainingManager : MonoBehaviour
{

    [SerializeField] CarManager carManag;
    [SerializeField] TrainingSettings trSettings;
    [SerializeField] DFSettings dFSettings;
    [SerializeField] MainManager MM;
    [SerializeField] Transform start;
    [SerializeField] GameObject carPrefab;
    [SerializeField] GameObject[] newCars;
    [SerializeField] int newCarLastIndex = 0;

    private Vector3 spawnPoint;
    [SerializeField] private float curTime;
    [SerializeField] private bool AutoStart = true;
    [SerializeField] string defaultSlot = " main";

    [SerializeField] TrainingState trState = TrainingState.pauseTraining;


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
        trState = TrainingState.pauseTraining;
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

    List<Vector2> GetScores(){
        List<Vector2> scores = new List<Vector2>();
        GameObject[] cars = carManag.GetAllCars();
        for (int i = 0; i < cars.Length; i++)
        {
            scores.Add(new Vector2(cars[i].GetComponent<DistanceFinder>().GetDist(), i));
        }
        
        return scores;
    }

    int findNewParent(List<Vector2> scores, float total_score){
        float parent = Random.Range(0f, total_score);
        float temp_sum = 0;
        int ret = 0;
        for (int i = 0; i < scores.Count; i++)
        {
            if (temp_sum < parent && scores[i][0] + temp_sum >= parent)
            {
                ret = (int)(scores[i][1]);
                break;
            }
            temp_sum += scores[i][0];
        }
        return ret;
    }

    void Selection(List<Vector2> scores){
        GameObject[] cars = carManag.GetAllCars();
        CarNN tempNN;
        for(int i = 0; i < trSettings.selCarNum && i < newCarLastIndex + 1 + trSettings.carNum; i++){
            tempNN = cars[(int)scores[i][1]].GetComponent<CarNN>();
            newCars[i + newCarLastIndex + 1] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            newCars[i + newCarLastIndex + 1].GetComponent<CarNN>().FillNN(tempNN.getW1(), tempNN.getW2(),tempNN.getW3());
        }
        newCarLastIndex += trSettings.selCarNum;
    }

    void NextGen(List<Vector2> scores){
        float total_score = scores.Sum(x => x[0]);
        GameObject[] cars = carManag.GetAllCars();
        CarNN temp;
        float priority = 0.5f;
        float[][] p1W1, p1W2, p1W3, p2W1, p2W2, p2W3;
        int parent1, parent2;
        int addedPerParents;
        
        while(newCarLastIndex + 1 < trSettings.carNum){
            parent1 = findNewParent(scores, total_score);

            temp = cars[parent1].GetComponent<CarNN>();
            p1W1 = temp.getW1();
            p1W2 = temp.getW2();
            p1W3 = temp.getW3();

            parent2 = findNewParent(scores, total_score);

            temp = cars[parent2].GetComponent<CarNN>();
            p2W1 = temp.getW1();
            p2W2 = temp.getW2();
            p2W3 = temp.getW3();

            addedPerParents = 0;
            for(int i = 0; i < trSettings.from2Parents && i + newCarLastIndex + 1 <  trSettings.carNum; i++){
                newCars[i + newCarLastIndex + 1] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
                newCars[i + newCarLastIndex + 1].GetComponent<CarNN>().FillNN(NeuralNetwork.merge_mutate(p1W1,p2W1, trSettings.percentOfMutation, priority), NeuralNetwork.merge_mutate(p1W2,p2W2, trSettings.percentOfMutation, priority), NeuralNetwork.merge_mutate(p1W3,p2W3, trSettings.percentOfMutation, priority));
                addedPerParents++;
            }
            newCarLastIndex += addedPerParents;
        }
    }

    void Training(){
        trState = TrainingState.setNN;

        newCars = new GameObject[trSettings.carNum];
        newCarLastIndex = -1;

        List<Vector2> scores = GetScores();

        scores = scores.OrderBy(x => x[0]).Reverse().ToList();
        Selection(scores);

        scores = scores.OrderBy(x => Random.value).ToList();
        NextGen(scores);

        carManag.FillNewCars(newCars);
    }

    public void changeAutoStart()
    {
        AutoStart = !AutoStart;
    }


    public void RefreshNN()
    {
        
        // if (trState == TrainingState.endTraining || trState == TrainingState.pauseTraining)
        // {
            trState = TrainingState.setNN;
            carManag.ApplyToAll(new CarManager.CarAction(x =>x.GetComponent<CarNN>().InitialiseNN()));
            carManag.SaveMaxNN(defaultSlot);
            RestartTraining();
        // }
    }


    
    public void startTrainingSession()
    {
        MM.UnPauseGame();

        if(trState == TrainingState.pauseTraining){
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

    public void SaveInSlot(string slot){
        CarNN.CopySave(defaultSlot, slot);
    }

    public void LoadAndMutateNN(string slot)
    {
        if (trState == TrainingState.endTraining || trState == TrainingState.pauseTraining)
        {
            
            TrainingState temp = trState;
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
            trState = temp;
        }
        RestartTraining();
    }

    public void RestartTraining(){
        trState = TrainingState.setNN;
        curTime = 0f;

        carPrefab.GetComponent<DistanceFinder>().dFSettings = dFSettings;

        //Initialise cars for car manager
        GameObject[] cars =  carManag.GetAllCars();
        GameObject[] newCars = new GameObject[trSettings.carNum];
        spawnPoint = new Vector3(start.position.x, start.position.y + 1.3f, start.position.z);
        for (int i = 0; i < trSettings.carNum; i++)
        {
            newCars[i] = Instantiate(carPrefab, spawnPoint, carPrefab.transform.rotation);
            CarNN tempNN = cars[i].GetComponent<CarNN>();
            newCars[i].GetComponent<CarNN>().FillNN(tempNN.getW1(), tempNN.getW2(), tempNN.getW3());
        }

        carManag.FillNewCars(newCars);

        trState = TrainingState.pauseTraining;
    }


}
