using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarManager : MonoBehaviour
{
    public delegate void CarAction(GameObject car);
    [SerializeField] GameObject[] cars = new GameObject[0];
    [SerializeField] int frameCount = 4;
    bool isDrive = false;

    public void FillNewCars(GameObject[] newCars)
    {
        foreach (GameObject car in cars)
        {
            Destroy(car);
        }
        cars = new GameObject[newCars.Length];
        for(int i = 0; i < cars.Length; i ++){
            cars[i] = newCars[i];
        }
        isDrive = false;
        print("Filled:");
        print(newCars.Length);
    }

    public void StartAllCars()
    {
        if (isDrive) { return; }
        for(int i = 0; i < cars.Length; i++)
        {
            cars[i].GetComponent<CarScript>().StartCar(i%frameCount,frameCount);

        }
        isDrive = true;
    }

    public void StopAllCars()
    {
        if (!isDrive) { return; }
        foreach (GameObject car in cars)
        {
            car.GetComponent<CarScript>().StopCar();
            car.GetComponent<DistanceFinder>().StopDF();
        }
        isDrive = false;
    }

    public void SaveMaxNN(string slot)
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

    public bool LoadNNToAll(string slot)
    {
        CarNN tempNN = cars[0].GetComponent<CarNN>();

        if (!tempNN.LoadNN(slot)) { return false; }
        float[][] w1 = tempNN.getW1();
        float[][] w2 = tempNN.getW2();
        float[][] w3 = tempNN.getW3();

        foreach (GameObject car in cars)
        {
            car.GetComponent<CarNN>().FillNN(w1, w2, w3);
        }
        return true;
    }

    public bool LoadNNTo(string slot, int index)
    {
        if (index > cars.Length && index < 0) { return false; }
        return cars[index].GetComponent<CarNN>().LoadNN(slot);
    }
    public GameObject[] GetAllCars()
    {
        return cars;
    }

    public void ApplyToAll(CarAction act){
        foreach(GameObject car in cars){
            act.Invoke(car);
        }
    }
    public GameObject GetCar(int index)
    {
        if(index > cars.Length && index < 0) {return null;}
        return cars[index];
    }
}
