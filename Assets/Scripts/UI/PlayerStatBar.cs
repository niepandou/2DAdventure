using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatBar : MonoBehaviour
{
    private Character character;
    public Image healthImage;
    public Image healthDelayImage;//红血显示,延迟血量变化展示
    public Image powerImage;
    private bool isRecovering;
    
    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= (Time.deltaTime * 0.1f);
        }

        if (isRecovering)
        {
            float percentage = character.currentPower / character.maxPower;
            powerImage.fillAmount = percentage;

            if (percentage >= 1)
            {
                isRecovering = false;
                return;
            }
        }
    }

    //接受血量变化百分比
    //percentage: current/Max
    public void OnHealthChange(float percentage)
    { 
        healthImage.fillAmount = percentage;
    }
    public void OnPowerChange(Character character)
    {
        this.character = character;
        isRecovering = true;
    }
}
