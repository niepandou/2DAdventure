using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Character : MonoBehaviour,ISaveable
{
    [FormerlySerializedAs("newGameEventSO")] [Header("事件监听")]
    public VoidEventSO afterSceneLoadedEventSO;
    [Header("属性")] 
    public float maxHealth;
    public float currentHealth;

    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    public float currentHurtRate;
    public float hurtRate;
    public float defaultHurtRate = 1;
    [Header("受伤无敌")] 
    public float wudiTime; //无敌时间

    public float wudiCounter;//无敌计时器
    public bool wudi;//是否无敌
    public bool isDead;
    
    public UnityEvent<Character> onHealthChange;
    public UnityEvent<Transform> onTakeDamage;
    public UnityEvent onDie;
    
    private void NewGame()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        currentHurtRate = defaultHurtRate;
        onHealthChange?.Invoke(this);
    }

    private void OnEnable()
    {
        afterSceneLoadedEventSO.OnEventRaised += NewGame;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        afterSceneLoadedEventSO.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void Update()
    {
        if (wudi)
        {
            wudiCounter -= Time.deltaTime;//计时器不断减小直至0
            if (wudiCounter <= 0)
            {
                wudi = false;
            }
        }

        if (currentPower < maxPower)
        {
            currentPower += powerRecoverSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            if (currentHealth >= 0)
            {
                //执行死亡,更新血量
                isDead = true;
                currentHealth = -100;
                onHealthChange?.Invoke(this);
                onDie?.Invoke();
            }
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if (wudi) return;
        if (currentHealth - attacker.damage * currentHurtRate > 0)//扣血,并触发受伤无敌
        {
            currentHealth -= attacker.damage*currentHurtRate;
            TriggerWudi(); 
            //执行受伤动画
            onTakeDamage?.Invoke(attacker.transform); 
        }
        else
        {
            if (currentHealth >= 0)
            {
                isDead = true;
                currentHealth = -100;
                //触发死亡
                onDie?.Invoke();
            }
            
            
        }

        //受伤血量发生变化,派送快递
        onHealthChange?.Invoke(this);
    }
    //受伤无敌
    private void TriggerWudi()
    {
        if (!wudi) wudi = true;
        wudiCounter = wudiTime; 
    }

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        onHealthChange?.Invoke(this);
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        if (data.characterPointDict.ContainsKey(GetDataID().ID))
        {
            data.characterPointDict[GetDataID().ID] = new SerializeVector3(transform.position);
            data.floatSavedData[GetDataID().ID + "health"] = this.currentHealth;
            data.floatSavedData[GetDataID().ID + "power"] = this.currentPower;
        }
        else
        {
            data.characterPointDict.Add(GetDataID().ID, new SerializeVector3(transform.position));
            data.floatSavedData.Add(GetDataID().ID + "health",this.currentHealth);
            data.floatSavedData.Add(GetDataID().ID + "power",this.currentPower);
        }
    }

    public void LoadData(Data data)
    {
        if (data.characterPointDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPointDict[GetDataID().ID].ToVector3();
            this.currentHealth = data.floatSavedData[GetDataID().ID + "health"];
            this.currentPower = data.floatSavedData[GetDataID().ID + "power"];
            
            //UI更新
            onHealthChange?.Invoke(this);
        }
    }
}
