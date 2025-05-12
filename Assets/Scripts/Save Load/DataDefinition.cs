using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//创建并获取物体的唯一GUID
public class DataDefinition : MonoBehaviour
{
    public PersistentType persistentType;
    public string ID;


    private void OnValidate()
    {
        if(persistentType == PersistentType.ReadWrite)
        {
            if (ID == string.Empty)
            {
                ID = System.Guid.NewGuid().ToString();
            }
        }
        else
        {
            ID = string.Empty;
        }   
    }
}
