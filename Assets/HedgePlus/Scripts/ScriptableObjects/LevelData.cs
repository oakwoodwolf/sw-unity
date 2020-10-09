using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Level", menuName = "Hedge+/Level", order = 0)]
public class LevelData : ScriptableObject
{
    public int SceneIndex;
    public string NameTop;
    public string NameBottom;
}
