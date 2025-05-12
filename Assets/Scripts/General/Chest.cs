using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Chest : MonoBehaviour,IInteractable,ISaveable
{   
    private static PlayerController CachedPlayer; // 静态缓存所有宝箱共享
    private Character cachedCharacter;
    private Animator anim;
    public bool isDone;
    
    public UnityEvent<Character> onHealthChange;
    private void OnEnable()
    {
        
        anim = GetComponent<Animator>();
        CachedPlayer = FindObjectOfType<PlayerController>();
        // 如果已有缓存直接使用
        if (CachedPlayer != null) {
            CacheCharacter(CachedPlayer);
        }
        // 否则开始监听玩家生成事件
        else {
            PlayerController.OnPlayerSpawned += CacheCharacter;
        }
        
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable() {
        PlayerController.OnPlayerSpawned -= CacheCharacter;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }
    private void CacheCharacter(PlayerController player) {
        CachedPlayer = player; // 更新静态缓存
        cachedCharacter = player.GetComponent<Character>();
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            openChest();
            isDone = true;
            gameObject.tag = "Untagged";
        }
    }

    void openChest()
    {
        cachedCharacter.currentHealth = cachedCharacter.maxHealth;
        cachedCharacter.currentPower = cachedCharacter.maxPower;
        onHealthChange?.Invoke(cachedCharacter);
        anim.SetBool("open",true);
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        if (data.boolSavedData.ContainsKey(GetDataID().ID + "chestState"))
        {
            data.boolSavedData[GetDataID().ID + "chestState"] = isDone;
        }
        else
        {
            data.boolSavedData.Add(GetDataID().ID + "chestState",isDone);
        }
    }

    public void LoadData(Data data)
    {
        if (data.boolSavedData.ContainsKey(GetDataID().ID + "chestState"))
        {
            this.isDone = data.boolSavedData[GetDataID().ID + "chestState"];
            
            //执行开箱操作但没有开箱的指定生效技能
            if (isDone)
            {
                anim.SetBool("open",true);
                gameObject.tag = "Untagged";
            }
            
        }
    }


}
