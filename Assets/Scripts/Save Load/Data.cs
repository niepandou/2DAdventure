using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    //保存场景
    public string sceneLoaderGuid;
    public string sceneToSave;
    
    //保存角色的坐标
    public Dictionary<string, SerializeVector3> characterPointDict = new Dictionary<string, SerializeVector3>();
    //保存角色的属性
    public Dictionary<string, float> floatSavedData = new Dictionary<string, float>();
    //保存宝箱,存档点的开关
    public Dictionary<string, bool> boolSavedData = new Dictionary<string, bool>();
    
    public void SaveGameScene(GameSceneSO savedScene)
    {
        sceneToSave = JsonUtility.ToJson(savedScene);
    }

    public GameSceneSO GetSavedGameScene()
    {
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();
        JsonUtility.FromJsonOverwrite(sceneToSave,newScene);

        return newScene;
    }
    
}

public class SerializeVector3
{
    public float x, y, z;

    public SerializeVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
