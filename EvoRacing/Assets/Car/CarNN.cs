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

    [SerializeField] int countOfRays;
    [SerializeField] float maxVel;

    private Rigidbody rb;
    private CarRayInfo cri;

    private float[][] w1;
    private float[] hidenLayer;
    private float[][] w2;
    


    void InitialiseNN(){
        w1 = new float[countOfRays + 2][];
        for(int i = 0; i < countOfRays + 2; i++){
            w1[i] = new float[countOfRays + 3];
            for(int j = 0; j < countOfRays + 3; j++){
                w1[i][j] = Random.Range(-5,5);
            }
        }

        hidenLayer = new float[countOfRays + 3];

        w2 = new float[countOfRays + 4][];
        for (int i = 0; i < countOfRays + 4; i++){
            w2[i] = new float[2];
            for(int j = 0; j < 2; j++){
                w2[i][j] = Random.Range(-5,5);
            }
        }
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

    void Awake(){
        InitialiseNN();
        rb = GetComponent<Rigidbody>();
        cri = GetComponent<CarRayInfo>();
    }

    public Vector2 predict(){
        float[] input = new float[countOfRays + 1];
        float[] rays = cri.GetRaysInfo();
        for(int i = 0; i < countOfRays; i++){
            input[i] = rays[i];
        }
        input[countOfRays] = rb.velocity.magnitude/maxVel;
        return predict(input);
    }
}
