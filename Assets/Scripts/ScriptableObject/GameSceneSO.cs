using UnityEngine;
using UnityEngine.AddressableAssets;
[CreateAssetMenu(menuName = "Game Scene/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    public SceneType SceneType;
    public AssetReference sceneReference; 
}
