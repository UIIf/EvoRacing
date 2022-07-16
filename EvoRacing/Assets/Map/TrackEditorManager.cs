using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.EventSystems;

public class TrackEditorManager : MonoBehaviour
{
    //delete
    [SerializeField] Transform temp;

    [SerializeField] Transform currentEnd;
    [SerializeField] GameObject button;
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
            Collider[] col = Physics.OverlapSphere(check.transform.GetChild(i).transform.position, 5f, LayerMask.GetMask("TrackPrefab", "Wall"));
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

        if (button != null)
        {
            if (!button.activeSelf)
                button.SetActive(true);

            RefreshButtonPosition();
        }

        //delete
        if (button != null)
            temp.transform.position = button.transform.position;
    }
    
    public void setSelectedPrefab(int value) { selectedPrefab = value; }

    public void SaveButton() { saveWindow.SetActive(true); }
    public void SaveTrack(int slot)
    {
        if (listForSave.Count == 0)
            print("Нет объектов");
        else if (listForSave[listForSave.Count - 1] != 12)
            print("Нет финиша");
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
            if (element == " " || element == "") continue;

            selectedPrefab = int.Parse(element);
            Create();
        }

        loadWindow.SetActive(false);
    }
    void Create()
    {
        GameObject newPrefab = Instantiate(prefabs[selectedPrefab], currentEnd.position, currentEnd.rotation);     
        listOfPrefabs.Add(newPrefab);
        listForSave.Add(selectedPrefab);

        currentEnd = newPrefab.transform.Find("End");
        listOfEnds.Add(currentEnd);

        if (button != null) { 
            RefreshButtonPosition();}

        //delete
        if(button != null)
            temp.transform.position = button.transform.position;

        if (button != null && selectedPrefab == 12)
            button.SetActive(false);
    }

    void RefreshButtonPosition()
    {
        button.transform.position = new Vector3(currentEnd.position.x, currentEnd.position.y, currentEnd.position.z);
        button.transform.rotation = Quaternion.Euler(currentEnd.rotation.eulerAngles + new Vector3(90f, 0f, 0f));
        button.transform.position += button.transform.right * 3f;
    }
}
