using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour,IInteractable
{
    public GameSceneSO sceneToGo;
    public Vector3 postionToGo;
    public SceneLoadEventSO LoadEventSO;
    public void TriggerAction() 
    {
        LoadEventSO.RaiseLoadRequestEvent(sceneToGo,postionToGo,true);
    }
}
