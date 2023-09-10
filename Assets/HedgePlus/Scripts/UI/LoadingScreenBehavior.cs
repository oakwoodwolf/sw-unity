using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LoadingScreenBehavior : MonoBehaviour
{
    public TextMeshProUGUI NameTop, NameBottom;
    public Image loadingProgress;
    public Animator loadingAnimator;
    float loadingAmount;
    public LevelData[] Levels;
    public float LoadingDelay = 1f;
    private static AsyncOperationHandle<SceneInstance> loadScene;


    private void Awake()
    {
        ///This object is meant to be persistent. So what we need to do is not destroy it when loading new scenes, and
        ///check if there is already a loading screen in the scene. If there is already one, destroy this one.
        DontDestroyOnLoad(gameObject);
        LoadingScreenBehavior checkForDuplicate = FindObjectOfType<LoadingScreenBehavior>();
        if (checkForDuplicate != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        loadingProgress.fillAmount = loadingAmount;
    }

    public void LoadLevel (int index)
    {
        ///This is pretty self explanatory. We set the loading screen text based on which level we are loading,
        ///play the screen transition, then begin loading the level.
        NameTop.text = Levels[index].NameTop;
        NameBottom.text = Levels[index].NameBottom;
        loadingAnimator.SetBool("Loading", true);
        StartCoroutine(LoadScene(Levels[index].levelScene));
    }

    IEnumerator LoadScene(AssetReference scene)
    {
        ///To actually have seamless loading as well as loading progress, we will be using an AsyncOperation.
        ///What this does is not only load the scene in the background, but also provide us with its loading
        ///progress, which we will then use for setting the progress bar.
        ///First we set a brief delay so it's not an instant scene transition. Afterwards, we start the async operation.
        ///Using a while loop for when it is not done loading, we simply set the loading progress value to the
        ///operation's progress, and return null to wait 1 frame before looping.
        ///Then, once the scene is fully loaded, we simply play the loading screen exit animation.
        yield return new WaitForSeconds(LoadingDelay);
        if (scene.RuntimeKeyIsValid())
        {
            // Load the scene asynchronously
            loadScene = Addressables.LoadSceneAsync(scene, activateOnLoad: true);
            while (!loadScene.IsDone)
            {

                loadingAmount = loadScene.PercentComplete / 0.9f;
                yield return null;
            }
            
            if (loadScene.IsDone)
            {
                loadingAnimator.SetBool("Loading", false);
                
            }
            if (loadScene.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Addressables initialized successfully.");
            }
            else if (loadScene.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Addressables initialization failed: " + loadScene.OperationException);
            }
        }
        else
        {
            Debug.LogError("AssetReference is not valid. " + scene);
            yield return null;
        }
        


    }
}
