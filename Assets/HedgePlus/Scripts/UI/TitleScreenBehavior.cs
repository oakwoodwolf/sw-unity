using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenBehavior : MonoBehaviour
{
    LoadingScreenBehavior loader;
    public AudioSource MusicSource;
    public int LevelToLoad = 0;
    private void Start()
    {
        loader = FindObjectOfType<LoadingScreenBehavior>();
    }
    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartCoroutine("FadeOutMusic");
            loader.LoadLevel(LevelToLoad);
        }
    }

    IEnumerator FadeOutMusic()
    {
        while (MusicSource.volume > 0)
        {
            MusicSource.volume -= Time.deltaTime;
            yield return null;
        }
    }
}
