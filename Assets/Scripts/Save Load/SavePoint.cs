using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class SavePoint : MonoBehaviour, IInteractable
{
    [FormerlySerializedAs("loadGameEventSO")] [Header("广播")] 
    public VoidEventSO saveGameEventSO;

    public SpriteRenderer spriteRenderer;
    public GameObject lightObj;
    public Sprite darkSprite;
    public Sprite lightSprite;
    
    private bool isDone;

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        lightObj.SetActive(isDone);
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            isDone = true; 
            spriteRenderer.sprite = lightSprite;
            lightObj.SetActive(true);
            this.gameObject.tag = "Untagged";
            
            //保存数据
            saveGameEventSO.RaiseEvent();
        }
    }
}