using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeCanvas : MonoBehaviour
{
    [Header("事件监听")] 
    public FadeEventSO fadeEvent;
    
    public Image fadeImage;

    private void OnEnable()
    {
        fadeEvent.onEventRaised += OnFadeEvent;
    }

    private void OnDisable()
    {
        fadeEvent.onEventRaised -= OnFadeEvent;
    }

    private void OnFadeEvent(Color target,float duration,bool fadeIn)
    {
        fadeImage.DOBlendableColor(target, duration);
    }
}
