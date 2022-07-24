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
    private float[] hiddenLayer1;
    private float[][] w2;

    private float[] hiddenLayer2;
    private float[][] w3;
    private bool initialized = false;
    


    public void InitialiseNN(){
        w1 = new float[countOfRays + 2][];
        for(int i = 0; i < countOfRays + 2; i++){
            w1[i] = new float[7];
            for(int j = 0; j < 7; j++){
                w1[i][j] = Random.Range(-1f,1f);
            }
        }

        hiddenLayer1 = new float[7];

        w2 = new float[8][];
        for (int i = 0; i < 8; i++){
            w2[i] = new float[7];
            for(int j = 0; j < 7; j++){
                w2[i][j] = Random.Range(-1f,1f);
            }
        }

        hiddenLayer2 = new float[7];

        w3 = new float[8][];
        for (int i = 0; i < 8; i++){
            w3[i] = new float[2];
            for(int j = 0; j < 2; j++){
                w3[i][j] = Random.Range(-1f,1f);
            }
        }

        initialized = true;
    }

    public void FillNN(float[][]a, float[][]b, float[][]c){
        w1 = new float[countOfRays + 2][];
        for(int i = 0; i < countOfRays + 2; i++){
            w1[i] = new float[7];
            a[i].CopyTo(w1[i], 0);
        }

        hiddenLayer1 = new float[7];

        w2 = new float[8][];
        for (int i = 0; i < 8; i++){
            w2[i] = new float[7];
            b[i].CopyTo(w2[i], 0);
        }

        hiddenLayer2 = new float[7];

        w3 = new float[8][];
        for (int i = 0; i < 8; i++){
            w3[i] = new float[2];
            c[i].CopyTo(w3[i], 0);
        }

        initialized = true;
    }

    Vector2 predict(float[] input){
        for(int i = 0; i < hiddenLayer1.Length; i++){
            hiddenLayer1[i] = 0;
            for(int j = 0; j < input.Length; j++){
                hiddenLayer1[i] += w1[j][i] * input[j];
            }
            hiddenLayer1[i] = sigmoid(hiddenLayer1[i] + w1[w1.Length - 1][i]);
        }

        for(int i = 0; i < hiddenLayer2.Length; i++){
            hiddenLayer2[i] = 0;
            for(int j = 0; j < hiddenLayer1.Length; j++){
                hiddenLayer2[i] += w2[j][i] * hiddenLayer1[j];
            }
            hiddenLayer2[i] = sigmoid(hiddenLayer2[i] + w2[w2.Length - 1][i]);
        }

        Vector2 output = Vector2.zero;

        for(int i = 0; i < 2; i++){
            for(int j = 0; j < hiddenLayer2.Length; j++){
                output[i] += w3[j][i]* hiddenLayer2[j];
            }
            output[i] = sigmoid(output[i] + w3[w3.Length - 1][i]);
        }
        return output;
    }

    [ContextMenu("Do Something")]
    public void printW()
    {
        string output = "";
        for (int i = 0; i < countOfRays + 2; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                output += w1[i][j].ToString() + " ";
            }
            output += "\n";
        }

        print(output);
        output = "";

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                output += w2[i][j].ToString() + " ";
            }
            output += "\n";
        }
        print(output);
        output = "";

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                output += w3[i][j].ToString() + " ";
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

    public float[][] getW3(){
        return w3;
    }

    [ContextMenu("Jopa")]
    public void SaveNN(string name)
    {
        if(!initialized){
            print("Error");
            return;
        }

        string saveStr = "";
        foreach (float[] element in w1)
        {
            
            foreach(float num in element)
            {
                saveStr += num.ToString() + " ";
            }
        }
        foreach (float[] element in w2)
        {
            
            foreach(float num in element)
            {
                saveStr += num.ToString() + " ";
            }
        }

        foreach (float[] element in w3)
        {
            
            foreach(float num in element)
            {
                saveStr += num.ToString() + " ";
            }
        }

        // print(saveStr);

        PlayerPrefs.SetString("SlotNN" + name, saveStr);

        // saveWindow.SetActive(false);
    }

    [ContextMenu("Load Jopa")]
    public bool LoadNN(string name)
    {
        string temp_loaded = PlayerPrefs.GetString("SlotNN" + name);
        if(temp_loaded == "") {return false;}
        string[] loadStr = temp_loaded.Split(' ');
        

        int counter = 0;

        w1 = new float[countOfRays + 2][];
        for(int i = 0; i < countOfRays + 2; i++){
            w1[i] = new float[7];
            for(int j = 0; j < 7; j++){
                w1[i][j] = float.Parse(loadStr[counter]);
                counter++;
            }
            
        }

        hiddenLayer1 = new float[7];

        w2 = new float[8][];
        for (int i = 0; i < 8; i++){
            w2[i] = new float[7];
            for(int j = 0; j < 7; j++){
                w2[i][j] = float.Parse(loadStr[counter]);
                counter++;
            }
        }

        hiddenLayer2 = new float[7];

        w3 = new float[8][];
        for (int i = 0; i < 8; i++){
            w3[i] = new float[2];
            for(int j = 0; j < 2; j++){
                w3[i][j] = float.Parse(loadStr[counter]);
                counter++;
            }
        }

        initialized = true;
        return true;
        // loadWindow.SetActive(false);
    }
}
