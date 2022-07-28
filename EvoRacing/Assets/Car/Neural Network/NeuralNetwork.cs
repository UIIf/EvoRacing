using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    public static float[][] merge_mutate(float[][] a, float[][] b, float presentOfMutation = 0.01f, float mutateRange = 0.01f, float parentPriority = 0.5f){
        if(a.Length != b.Length || a[0].Length != b[0].Length){
            return null;
        }

        mutateRange = Mathf.Abs(mutateRange);
        float[][] ret = new float[a.Length][];
        for(int i = 0; i < a.Length; i++){
            ret[i] = new float[a[0].Length];
        }

        float rand;

        for(int i = 0; i < a.Length; i++){
            for(int j = 0; j < a[0].Length; j++){
                rand = Random.Range(0, 10000);
                ret[i][j] = (rand < 10000 * parentPriority?a[i][j]:b[i][j]);

                //Mutation
                if(rand < 10000 * presentOfMutation){
                    ret[i][j] += Random.Range(-mutateRange, mutateRange);
                }
            }
        }

        return ret;
    }

    public static float[][] merge(float[][] a, float[][] b){
        if(a.Length != b.Length || a[0].Length != b[0].Length){
            return null;
        }

        float[][] ret = new float[a.Length][];
        for(int i = 0; i < a.Length; i++){
            ret[i] = new float[a[0].Length];
        }

        float rand;

        for(int i = 0; i < a.Length; i++){
            for(int j = 0; j < a[0].Length; j++){
                rand = Random.Range(0, 10000);
                ret[i][j] = (rand % 2 == 0?a[i][j]:b[i][j]);
            }
        }

        return ret;
    }

    public static float[][] mutate(float[][] a, float presentOfMutation = 0.01f, float mutateRange = 0.01f){

        mutateRange = Mathf.Abs(mutateRange);
        float[][] ret = new float[a.Length][];
        for(int i = 0; i < a.Length; i++){
            ret[i] = new float[a[0].Length];
        }

        float rand;

        for(int i = 0; i < a.Length; i++){
            for(int j = 0; j < a[0].Length; j++){
                rand = Random.Range(0, 10000);
                ret[i][j] = a[i][j];

                //Mutation
                if(rand < 10000 * presentOfMutation){
                    ret[i][j] += Random.Range(-mutateRange, mutateRange);
                    // if(ret[i][j] > 1){
                    //     ret[i][j] = 1;
                    // }
                    // else if(ret[i][j] < -1){
                    //     ret[i][j] = -1;
                    // }
                }
            }
        }

        return ret;
    }
}