using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIManager : MonoBehaviour
{
    public PlayerStatBar playerStatBar;
    [Header("广播")] 
    public VoidEventSO pauseEvent;    
    [Header("事件监听")] 
    public CharacterEventSO healthEvent;
    public SceneLoadEventSO unloadedSceneEventSO;
    public VoidEventSO loadDataEventSO;
    public VoidEventSO gameOverEventSO;
    public VoidEventSO backToMenuEventSO;
    public FloatEventSO syncVolumeEvent;
    
    [Header("组件")]
    public GameObject gameOverPanel;
    public GameObject restart;
    public GameObject moblieTouch;
    public Button settings;
    public GameObject pausePanel;
    public Slider volumeSlider;
    private void Awake()
    {
        //pc模式
        #if UNITY_STANDALONE
        moblieTouch.SetActive(false);
        #endif 
        
        settings.onClick.AddListener(TogglePausePanel);
    }

    private void OnEnable()
    {
        //告诉中转站添加一个指定地址
        healthEvent.onEventRaised += OnHealthEvent;
        unloadedSceneEventSO.LoadRequestEvent += OnUnloadedSceneEvent;
        loadDataEventSO.OnEventRaised += OnLoadDataEvent;
        gameOverEventSO.OnEventRaised += OnGameOverEvent;
        //借用一下loadDataEvent来关闭死亡面板
        backToMenuEventSO.OnEventRaised += OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;

    }

    private void OnDisable()
    {
        healthEvent.onEventRaised -= OnHealthEvent;
        unloadedSceneEventSO.LoadRequestEvent -= OnUnloadedSceneEvent;
        loadDataEventSO.OnEventRaised -= OnLoadDataEvent;
        gameOverEventSO.OnEventRaised -= OnGameOverEvent;
        backToMenuEventSO.OnEventRaised -= OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;

    }

    private void OnSyncVolumeEvent(float value)
    {
        volumeSlider.value = (value + 80) / 100; 
    }

    //开关暂停面板
    public void TogglePausePanel()
    {
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseEvent.OnEventRaised();
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    private void OnLoadDataEvent()
    {
        //关闭死亡面板
        gameOverPanel.SetActive(false);
    }

    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restart);
    }
    private void OnUnloadedSceneEvent(GameSceneSO sceneToLoad, Vector3 arg1, bool arg2)
    {
        if (sceneToLoad.SceneType == SceneType.Menu)
        {
            playerStatBar.gameObject.SetActive(false);
        }
        else
        {
            playerStatBar.gameObject.SetActive(true);
            }
    }

    //到达指定地址,对快递进行处理
    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChange(percentage);
        playerStatBar.OnPowerChange(character);
    }
}
