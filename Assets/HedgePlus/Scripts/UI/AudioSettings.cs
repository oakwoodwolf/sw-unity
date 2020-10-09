using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class AudioSettings : MonoBehaviour
{
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SoundVolumeSlider;
    public AudioMixerGroup MusicMixer;
    public AudioMixerGroup SoundMixer;
    float MasterVolume = 1f;
    float MusicVolume = 1f;
    float SoundVolume = 1f;
    private void OnEnable()
    {
        MasterVolumeSlider.value = MasterVolume;
        MusicVolumeSlider.value = MusicVolume;
        SoundVolumeSlider.value = SoundVolume;
    }

    private void Update()
    {
        MasterVolume = MasterVolumeSlider.value;
        MusicVolume = MusicVolumeSlider.value;
        SoundVolume = SoundVolumeSlider.value;

        AudioListener.volume = MasterVolume;
        MusicMixer.audioMixer.SetFloat("Volume", Mathf.Lerp(-40f, 0f, MusicVolume));
        SoundMixer.audioMixer.SetFloat("Volume", Mathf.Lerp(-40f, 0f, SoundVolume));
    }
}
