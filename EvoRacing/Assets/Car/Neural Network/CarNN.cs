using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CarRayInfo))]



public class CarNN : MonoBehaviour
{
    float sigmoid(float x){
        return 1/(1 + Mathf.Exp(-x));
    }

    private int countOfRays;
    [SerializeField] float maxVel;
    [SerializeField] float cur_Vel;

    private Rigidbody rb;
    private CarRayInfo cri;

    private float[][] w1;
    private float[] hidenLayer;
    private float[][] w2;
    private bool initialized = false;
    


    public void InitialiseNN(){
        w1 = new float[countOfRays + 2][];
        for(int i = 0; i < countOfRays + 2; i++){
            w1[i] = new float[countOfRays + 3];
            for(int j = 0; j < countOfRays + 3; j++){
                w1[i][j] = Random.Range(-5f,5f);
            }
        }

        hidenLayer = new float[countOfRays + 3];

        w2 = new float[countOfRays + 4][];
        for (int i = 0; i < countOfRays + 4; i++){
            w2[i] = new float[2];
            for(int j = 0; j < 2; j++){
                w2[i][j] = Random.Range(-5f,5f);
            }
        }

        initialized = true;
    }

    public void FillNN(float[][]a, float[][]b){
        w1 = new float[countOfRays + 2][];
        for(int i = 0; i < countOfRays + 2; i++){
            w1[i] = new float[countOfRays + 3];
            a[i].CopyTo(w1[i], 0);
        }

        hidenLayer = new float[countOfRays + 3];

        w2 = new float[countOfRays + 4][];
        for (int i = 0; i < countOfRays + 4; i++){
            w2[i] = new float[2];
            b[i].CopyTo(w2[i], 0);
        }

        initialized = true;
    }

    Vector2 predict(float[] input){
        for(int i = 0; i < hidenLayer.Length; i++){
            hidenLayer[i] = 0;
            for(int j = 0; j < input.Length; j++){
                hidenLayer[i] += w1[j][i] * input[j];
            }
            hidenLayer[i] = sigmoid(hidenLayer[i] + w1[w1.Length - 1][i]);
        }
        Vector2 output = Vector2.zero;

        for(int i = 0; i < 2; i++){
            for(int j = 0; j < hidenLayer.Length; j++){
                output[i] += w2[j][i]*hidenLayer[j];
            }
            output[i] = sigmoid(output[i] + w2[w2.Length - 1][i]);
        }
        return output;
    }

    [ContextMenu("Do Something")]
    public void printW()
    {
        string output = "";
        for (int i = 0; i < countOfRays + 2; i++)
        {
            for (int j = 0; j < countOfRays + 3; j++)
            {
                output += w1[i][j].ToString() + " ";
            }
            output += "\n";
        }

        print(output);
        output = "";

        for (int i = 0; i < countOfRays + 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                output += w2[i][j].ToString() + " ";
            }
            output += "\n";
        }
        print(output);
    }
    void Awake(){
        rb = GetComponent<Rigidbody>();
        cri = GetComponent<CarRayInfo>();
        countOfRays = cri.GetRaysCount();
    }

    public Vector2 predict(){

        float[] input = new float[countOfRays + 1];
        float[] rays = cri.GetRaysInfo();
        for(int i = 0; i < countOfRays; i++){
            input[i] = rays[i];
        }
        cur_Vel =  rb.velocity.magnitude/maxVel;
        input[countOfRays] = rb.velocity.magnitude/maxVel;
        return predict(input);
    }

    public float[][] getW1(){
        return w1;
    }
    public float[][] getW2(){
        return w2;
    }
}
