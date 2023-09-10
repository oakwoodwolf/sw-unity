using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Level", menuName = "Hedge+/Level", order = 0)]
public class LevelData : ScriptableObject
{
    public int SceneIndex;
    public AssetReference levelScene;
    public string NameTop;
    public string NameBottom;
}
