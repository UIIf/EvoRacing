using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] TrainingManager trManag;
    [SerializeField] GameObject LoadWindow;
    [SerializeField] GameObject SaveWindow;
    [SerializeField] GameObject LSContent;
    [SerializeField] GameObject LSTemplate;
    [SerializeField] MainManager mManager;


    void Awake(){
        BuildLoadWindow();
    }

    public void OpenSaveWindow(){
        SaveWindow.SetActive(true);
        mManager.PauseGame(); 
    }
    public void CloseSaveWindow(){
        SaveWindow.SetActive(false); 
        mManager.UnPauseGame();
    }

    public void ToggleLoadWindow(){
        if(!SaveWindow.activeSelf){
            LoadWindow.SetActive(!LoadWindow.activeSelf);
            if(LoadWindow.activeSelf){
                mManager.PauseGame();
            }
            else{
                mManager.UnPauseGame();
            }
        }
            
    }

    public void OpenLoadWindow(){
        LoadWindow.SetActive(true); 
        mManager.PauseGame();
    }
    public void CloseLoadWindow(){
        LoadWindow.SetActive(false); 
        mManager.UnPauseGame();
    }

    public void BuildLoadWindow(){
        Transform content = LSContent.transform;
        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).name == "CarHolder")
                Destroy(content.GetChild(i).gameObject);
        }
        string[] slots = PlayerPrefs.GetString("AllTheSlots").Split(' ');

        foreach (string element in slots)
        {
            if (element == "") continue;
            GameObject newElement = Instantiate(LSTemplate, content);
            
            Text newElementNameText = newElement.transform.Find("SlotName").GetComponent<Text>();
            //Text newElementGenNumText = newElement.transform.Find("GenerationsNum").GetComponent<Text>();
            Button newElementDeleteButton = newElement.transform.Find("DeleteButton").GetComponent<Button>();
            Button newElementLoadButton = newElement.transform.Find("LoadButton").GetComponent<Button>();
            Button newElementSaveButton = newElement.transform.Find("SaveButton").GetComponent<Button>();

            newElement.name = "CarHolder";

            newElementNameText.text = element;

            newElementDeleteButton.onClick.AddListener(() => DeleteSlot(element));

            newElementLoadButton.onClick.AddListener(() => trManag.LoadAndMutateNN(element));
            newElementLoadButton.onClick.AddListener(() => CloseLoadWindow());
           
            newElementSaveButton.onClick.AddListener(() => trManag.SaveInSlot(element));

            newElement.SetActive(true);
        }
    }

    public void SaveFromButton(){
        
        InputField saveText = SaveWindow.transform.Find("InputField").GetComponent<InputField>();
    
        if(saveText.text == "" || saveText.text.Length > 30) return;

        string slot = saveText.text.Replace(' ', '_');
        saveText.text = "";
        trManag.SaveInSlot(slot);

        string str = PlayerPrefs.GetString("AllTheSlots");

        if (str == ""){
            PlayerPrefs.SetString("AllTheSlots", slot + " ");
        }
        else{
            if (str.Contains(slot + " ")) return;

            PlayerPrefs.SetString("AllTheSlots", str + slot + " ");
        }
        CloseSaveWindow();
        BuildLoadWindow();
        OpenLoadWindow();
    }    
    public void DeleteSlot(string element){

        PlayerPrefs.DeleteKey(element);

        string slots = PlayerPrefs.GetString("AllTheSlots");
        

        PlayerPrefs.SetString("AllTheSlots", slots.Remove(slots.IndexOf(element), element.Length + 1));
        
        BuildLoadWindow();
    }

}
