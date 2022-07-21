using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum camMoveState{
    wait,
    move,
    scale
}
public class CamMoveScript : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] int count;//TEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEMP
    [SerializeField] bool isActive = true;
    camMoveState state = camMoveState.wait;
    Vector3 prevPos;
    Touch touch1;



    void Update(){
        FindTouch();
        count = Input.touchCount;
        switch(state){
            case camMoveState.move:
                MoveCam();
                break;
        }
    }

    void FindTouch(){
        if(Input.touchCount < 1){
            state = camMoveState.wait;
        }
        if(Input.touchCount > 0){
            print("Touched");
            touch1 = Input.GetTouch(0);
            state = camMoveState.move;
        }
    }

    void MoveCam(){
        transform.position += new Vector3(touch1.deltaPosition[0], transform.position.y, touch1.deltaPosition[1]);
    }

    public void Activete(){
        isActive = true;
    }

    public void Disactivete(){
        isActive = false;
    }
}
