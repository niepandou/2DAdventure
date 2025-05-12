using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    public UnityAction<Character> onEventRaised;

    public void RaiseEvent(Character character)
    {
        //onEventRaised作为中转站
        //接受到血量变化的快递,将快递运输到指定位置
        onEventRaised?.Invoke(character);
    }
}
