using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("广播")] 
    public FloatEventSO syncVolumeEvent;
    [Header("事件监听")] 
    public PlayAudioEventSO BGMEvent;
    public PlayAudioEventSO SFXEvent;
    public FloatEventSO volumeChangeEvent;
    public VoidEventSO pauseEvent;
    
    [Header("组件")] 
    public AudioSource BGM;
    public AudioSource SFX;
    public AudioMixer mixer;
    private void OnEnable()
    {
        SFXEvent.OnEventRaised += OnSFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
        volumeChangeEvent.OnEventRaised += OnVolumenChangeEvent;
        pauseEvent.OnEventRaised += OnPauseEvent;
    }
    
    private void OnDisable()
    {
        SFXEvent.OnEventRaised -= OnSFXEvent;       
        BGMEvent.OnEventRaised -= OnBGMEvent;
        volumeChangeEvent.OnEventRaised -= OnVolumenChangeEvent;
        pauseEvent.OnEventRaised -= OnPauseEvent;
    }

    private void OnPauseEvent()
    {
        float value;
        mixer.GetFloat("MasterVolume", out value);
        syncVolumeEvent.OnEventRaised(value);
    }

    private void OnVolumenChangeEvent(float value)
    {
        mixer.SetFloat("MasterVolume", value * 100 -80);
    }

    private void OnBGMEvent(AudioClip clip)
    {
        BGM.clip = clip;
        BGM.Play();
    }

    private void OnSFXEvent(AudioClip clip)
    {
        SFX.clip = clip;
        SFX.Play();
    }
}
