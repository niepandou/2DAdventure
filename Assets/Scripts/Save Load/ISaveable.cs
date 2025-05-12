using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    DataDefinition GetDataID();
    //现在接口可以实现默认方法
    void RegisterSaveData()
    {
        DataManager.instance.RegisterSaveData(this);
    }
    //语法糖简写
    void UnRegisterSaveData() => DataManager.instance.UnRegisterSaveData(this);

    void GetSaveData(Data data);
    void LoadData(Data data);
}
