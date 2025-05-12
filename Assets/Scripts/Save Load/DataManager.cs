using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    private List<ISaveable> saveableList = new List<ISaveable>();
    public Data saveData;
    public string sceneGuid;
    private string jsonFolder;
    
    [Header("事件监听")] 
    public VoidEventSO saveDataEventSO;

    public VoidEventSO loadDataEventSO;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        saveData = new Data();
        jsonFolder = Application.persistentDataPath + "/Save Data/";
        ReadSavedData();
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
        saveDataEventSO.OnEventRaised += Save;
        loadDataEventSO.OnEventRaised += Load;
    }

    private void OnDisable()
    {
        saveDataEventSO.OnEventRaised -= Save;
        loadDataEventSO.OnEventRaised -= Load;

    }

    public void RegisterSaveData(ISaveable saveable)
    {
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }

    public void UnRegisterSaveData(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }

    public void Save()
    {
        foreach (var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }

        string resultPath = jsonFolder + "save.dat";

        var jsonData = JsonConvert.SerializeObject(saveData);

        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        
        File.WriteAllText(resultPath,jsonData);
        
    }

    public void Load()
    {
        StartCoroutine(LoadCoroutine());
    }

    IEnumerator LoadCoroutine()
    {
        //先加载场景后加载其余内容
        ISaveable sceneLoaderSaveable = null;
        foreach (var saveable in saveableList)
        {
            if (saveable.GetDataID().ID == saveData.sceneLoaderGuid)
            {
                sceneLoaderSaveable = saveable;
                break;
            }
        }
        
        if(sceneLoaderSaveable == null) yield break;
        
        sceneLoaderSaveable.LoadData(saveData);
        yield return new WaitForSeconds(2f);
        
        foreach (var saveable in saveableList)
        {
            if (saveable.GetDataID().ID != saveData.sceneLoaderGuid)
            {
                saveable.LoadData(saveData);
                yield return null;
            }
        }
    }

    private void ReadSavedData()
    {
        string resultPath = jsonFolder + "save.dat";
        if (File.Exists(resultPath))
        {
            string stringData = File.ReadAllText(resultPath);
            //序列化
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);
            saveData = jsonData;
        }
        
        
    }
}
