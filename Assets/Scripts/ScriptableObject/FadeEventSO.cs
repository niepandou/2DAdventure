using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FadeEventSO")]
public class FadeEventSO : ScriptableObject
{
        
    public UnityAction<Color, float, bool> onEventRaised;
    /// <summary>
    /// 逐渐变黑
    /// </summary>
    /// <param name="duration">渐变时间</param>
    public void FadeIn(float duration)
    {
        RaiseEvent(Color.black,duration,true);
    }

    /// <summary>
    /// 逐渐变透明
    /// </summary>
    /// <param name="duration">渐变时间</param>
    public void FadeOut(float duration)
    {
        RaiseEvent(Color.clear,duration,false);
    }

    /// <summary>
    /// 事件执行方法
    /// </summary>
    /// <param name="target">目标颜色</param>
    /// <param name="duration">渐变时间</param>
    /// <param name="fadeIn">是否渐入</param>
    public void RaiseEvent(Color target,float duration,bool fadeIn)
    {
        onEventRaised?.Invoke(target,duration,fadeIn);
    }
}
