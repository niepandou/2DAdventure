using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    [Header("事件监听")] 
    public VoidEventSO afterSceneLoadedEventSO;
    
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impluseSource;
    public VoidEventSO cameraShakeEvent;
    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    // private void Start()
    // {
    //     Application.targetFrameRate = 60;
    // }

    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += OnCameraShake;
        afterSceneLoadedEventSO.OnEventRaised += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= OnCameraShake;
        afterSceneLoadedEventSO.OnEventRaised -= OnAfterSceneLoadedEvent;

    }

    #region 事件

    private void OnAfterSceneLoadedEvent()
    {
        GetNewCameraBounds();
    }

    private void OnCameraShake()
    {
        impluseSource.GenerateImpulse();
    }

    #endregion
    
    
    // private void Start()
    // {
    //     GetNewCameraBounds();
    // }

    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindWithTag("Bounds");

        if (obj == null)
        {
            return;
        }

        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();//清理先前场景的缓存
    }
}
