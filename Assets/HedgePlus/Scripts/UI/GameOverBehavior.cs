using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverBehavior : MonoBehaviour
{
    public string NextScene;
    void BeginLoadScene()
    {
        SceneManager.LoadSceneAsync(NextScene);
    }
}
