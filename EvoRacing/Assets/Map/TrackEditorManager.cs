using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.EventSystems;

enum Elements
{
    Straight,
    Straight_Long,
    Turn_60_R,
    Turn_60_L,
    Turn_120_R,
    Turn_120_L,
    U_Turn,
    Zigzag,
    Zigzag_Sharp,
    Finish
}
public class TrackEditorManager : MonoBehaviour
{
    //delete
    [SerializeField] Transform temp;

    [SerializeField] Transform currentEnd;
    [SerializeField] Transform button;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] GameObject[] checkPrefabs;
    [SerializeField] int selectedPrefab = 0;
    [SerializeField] GameObject saveWindow;
    [SerializeField] GameObject loadWindow;

    List<Transform> listOfEnds = new List<Transform>();
    List<GameObject> listOfPrefabs = new List<GameObject>();
    List<int> listForSave = new List<int>();

    public void Start()
    {
        listOfEnds.Add(currentEnd);
    }
    public void Check()
    {
        GameObject check = Instantiate(checkPrefabs[selectedPrefab], currentEnd.position, currentEnd.rotation);
        bool clear = true;
        for(int i = 0; i < check.transform.childCount; i++)
        {
            Collider[] col = Physics.OverlapSphere(check.transform.GetChild(i).transform.position, 4f, LayerMask.GetMask("TrackPrefab"));
            if (col.Length != 0) clear = false;
        }

        Destroy(check);

        if (clear)  Create();
        else    print("NOT OK");
    }

    public void DeleteLast()
    {
        if (listOfPrefabs.Count == 0) return;

        Destroy(listOfPrefabs[listOfPrefabs.Count - 1]);
        listOfPrefabs.RemoveAt(listOfPrefabs.Count - 1);
        listOfEnds.RemoveAt(listOfEnds.Count - 1);
        listForSave.RemoveAt(listForSave.Count - 1);
        currentEnd = listOfEnds[listOfEnds.Count - 1];
        RefreshButtonPosition();

        //delete
        temp.transform.position = button.transform.position;
    }
    
    public void setSelectedPrefab(int value) { selectedPrefab = value; }

    public void SaveButton() { saveWindow.SetActive(true); }
    public void SaveTrack(int slot)
    {
        if (listForSave.Count != 0 && listForSave[listForSave.Count - 1] != 12)
        {
            print("Нет финиша");
        }
        else
        {
            string saveStr = "";
            foreach (int element in listForSave)
            {
                saveStr += element.ToString() + " ";
            }

            PlayerPrefs.SetString("Slot" + slot.ToString(), saveStr);
        }

        saveWindow.SetActive(false);
    }

    public void LoadButton() { loadWindow.SetActive(true); } 
    public void LoadTrack(int slot)
    {
        while (listOfPrefabs.Count > 0) DeleteLast();

        string[] loadStr = PlayerPrefs.GetString("Slot" + slot.ToString()).Split(' ');

        foreach (string element in loadStr)
        {
            selectedPrefab = int.Parse(element);
            Create();
        }
    }
    void Create()
    {
        GameObject newPrefab = Instantiate(prefabs[selectedPrefab], currentEnd.position, currentEnd.rotation);     
        listOfPrefabs.Add(newPrefab);
        listForSave.Add(selectedPrefab);

        currentEnd = newPrefab.transform.Find("End");
        listOfEnds.Add(currentEnd);

        RefreshButtonPosition();

        //delete
        temp.transform.position = button.transform.position;
    }

    void RefreshButtonPosition()
    {
        button.transform.position = new Vector3(currentEnd.position.x, currentEnd.position.y, currentEnd.position.z);
        button.transform.rotation = Quaternion.Euler(currentEnd.rotation.eulerAngles + new Vector3(90f, 0f, 0f));
        button.transform.position += button.right * 3f;
    }
}
