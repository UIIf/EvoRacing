using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

enum camMoveState{
    wait,
    move,
    scale
}
public class CamMoveScript : MonoBehaviour
{
    [SerializeField] Camera[] cameras;
    bool rotate = true;
    int currentCam = 0;
    Plane plane;

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

        if (Input.touchCount >= 1)
            plane.SetNormalAndPosition(transform.up, transform.position);

        var delta1 = Vector3.zero;
        var delta2 = Vector3.zero;

        if (Input.touchCount >= 1)
        {
            delta1 = PlanePositionDelta(Input.GetTouch(0));
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
                cameras[0].transform.Translate(delta1, Space.World);
        }

        if (Input.touchCount >= 2)
        {
            var pos1 = PlanePosition(Input.GetTouch(0).position);
            var pos2 = PlanePosition(Input.GetTouch(1).position);
            var pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            var pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

            var zoom = Vector3.Distance(pos1, pos2) / Vector3.Distance(pos1b, pos2b);
            if (zoom == 0 || zoom > 10)
                return;

            cameras[0].transform.position = Vector3.LerpUnclamped(pos1, cameras[0].transform.position, 1 / zoom);

            if (rotate && pos2b != pos2)
                cameras[0].transform.RotateAround(pos1, plane.normal, Vector3.SignedAngle(pos2 - pos1, pos2b - pos1b, plane.normal));
        }
    }

    private Vector3 PlanePositionDelta(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved)
            return Vector3.zero;

        var rayBefore = cameras[0].ScreenPointToRay(touch.position - touch.deltaPosition);
        var rayNow = cameras[0].ScreenPointToRay(touch.position);
        if (plane.Raycast(rayBefore, out var enterBefore) && plane.Raycast(rayNow, out var enterNow))
        {
            return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);
        }

        return Vector3.zero;
    }
    private Vector3 PlanePosition(Vector2 screenPos)
    {
        //if (currentCam == 1) return Vector3.zero;

        var rayNow = cameras[0].ScreenPointToRay(screenPos);
        if (plane.Raycast(rayNow, out var enterNow))
            return rayNow.GetPoint(enterNow);

        return Vector3.zero;
    }
    public void MoveTo(Vector3 point)
    {

    }
    public void SwitchCam()
    {
        if (cameras[0].gameObject.activeSelf)
        {
            cameras[0].gameObject.SetActive(false);
            cameras[1].gameObject.SetActive(true);
        }
        else
        {
            cameras[1].gameObject.SetActive(false);
            cameras[0].gameObject.SetActive(true);
        }

        //need to change canvas
    }
}

//public class CamMoveScript : MonoBehaviour
//{
//    [SerializeField] Camera cam;
//    [SerializeField] int count;//TEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEMP
//    [SerializeField] bool isActive = true;
//    camMoveState state = camMoveState.wait;
//    Vector3 prevPos;
//    Touch touch1;



//    void Update()
//    {
//        FindTouch();
//        count = Input.touchCount;
//        switch (state)
//        {
//            case camMoveState.move:
//                MoveCam();
//                break;
//        }
//    }

//    void FindTouch()
//    {
//        if (Input.touchCount < 1)
//        {
//            state = camMoveState.wait;
//        }
//        if (Input.touchCount > 0)
//        {
//            print("Touched");
//            touch1 = Input.GetTouch(0);
//            state = camMoveState.move;
//        }
//    }

//    void MoveCam()
//    {
//        transform.position += new Vector3(-touch1.deltaPosition[0]/15, 0, -touch1.deltaPosition[1]/15);
//    }

//    public void Activete()
//    {
//        isActive = true;
//    }

//    public void Disactivete()
//    {
//        isActive = false;
//    }
//}
