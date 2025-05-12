using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour,ISaveable
{
    public PlayerController player;
    public Vector3 firstPos;
    public Vector3 menuPos;
    public AudioManager audioManager;
    public GameObject pausePanel;
    public Button settingsBtn;
    
    [FormerlySerializedAs("loadEventSo")] 
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEventSO;
    public VoidEventSO BackToMenuEventSO;
    
    [Header("广播")] 
    public VoidEventSO AfterSceneLoadedEventSo;
    public SceneLoadEventSO sceneUnloadedEventSO;

    [Header("场景")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;
    private GameSceneSO currentLoadedScene;
    private GameSceneSO SceneToLoad;
    
    public FadeEventSO fadeEvent;
    
    private Vector3 positionToGo;
    private bool fadeScreen;
    private bool isLoading;
    public float fadeDuration;
    
     private void Start()
     {
         loadEventSO.RaiseLoadRequestEvent(menuScene,menuPos,true);

         // NewGame();
     }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEventSO.OnEventRaised += NewGame;
        BackToMenuEventSO.OnEventRaised += OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }
    
    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEventSO.OnEventRaised -= NewGame;
        BackToMenuEventSO.OnEventRaised -= OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void NewGame()
    {
        SceneToLoad = firstLoadScene;
        //OnLoadRequestEvent(SceneToLoad,firstPos,true);
        loadEventSO.RaiseLoadRequestEvent(SceneToLoad,firstPos,true);
    }
    //返回菜单执行操作
    private void OnBackToMenuEvent()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        SceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(SceneToLoad,menuPos,true);
    }
    
/// <summary>
/// 场景加载事件请求
/// </summary>
/// <param name="locationToLoad">去往的场景</param>
/// <param name="posToGo">玩家传送地点</param>
/// <param name="fadeScreen">是否渐入渐出</param>
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading) return;
        
        isLoading = true;
        this.SceneToLoad = locationToLoad;
        this.positionToGo = posToGo;
        this.fadeScreen = fadeScreen;
        if(currentLoadedScene != null)
            StartCoroutine(UnLoadPreviousScene());
        else LoadNewScene();
    }
/// <summary>
/// 卸载之前的场景 
/// </summary>
/// <returns></returns>
    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            fadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);

        //广播调整ui显示
        sceneUnloadedEventSO.LoadRequestEvent(SceneToLoad, positionToGo, true);
        
        //修改玩家z轴保证暂时不与任何物体交互
        player.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
        //卸载场景
        currentLoadedScene.sceneReference.UnLoadScene();
        
        //加载新场景
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        var loadingOption = SceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        loadingOption.Completed += OnLoadCompleted;
    }

    /// <summary>
    /// 场景加载完成
    /// </summary>
    /// <param name="obj"></param>
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadedScene = SceneToLoad;
        //无需再改z轴,这里自己就改完了
        player.transform.position = positionToGo;
     
        //player.gameObject.SetActive(true);
        if (fadeScreen)
        {
            fadeEvent.FadeOut(fadeDuration);
        }

        isLoading = false;
        audioManager.gameObject.SetActive(true);
        settingsBtn.gameObject.SetActive(true);
        //发现加载的是菜单场景,角色不应该进行操作
        //不进行之后的传递信号操作,使玩家不会监听到加载完成事件的信号从而无法操作
        if (currentLoadedScene.SceneType == SceneType.Menu)
        {
            audioManager.gameObject.SetActive(false);
            settingsBtn.gameObject.SetActive(false);
            return;
        }
        //场景加载完成,事件广播传递信号
        AfterSceneLoadedEventSo.OnEventRaised();
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        data.sceneLoaderGuid = GetDataID().ID;
        data.SaveGameScene(currentLoadedScene);
    }

    public void LoadData(Data data)
    {
        var playerID = player.GetComponent<DataDefinition>().ID;

        if (data.characterPointDict.ContainsKey(playerID))
        {
            positionToGo = data.characterPointDict[playerID].ToVector3();
            SceneToLoad = data.GetSavedGameScene();
            
            OnLoadRequestEvent(SceneToLoad,positionToGo,true);
        }
    }
}
