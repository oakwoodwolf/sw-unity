using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
public class GraphicsSettings : MonoBehaviour
{
    [System.Serializable] public class Resolution
    {
        public int Width;
        public int Height;
    }
    public List<Resolution> resolutions;

    [Header("UI Elements")]
    public TMP_Dropdown Quality;
    public TMP_Dropdown _resolution;
    public TMP_Dropdown BloomToggle;
    public TMP_Dropdown GradingToggle;
    public TMP_Dropdown OcclusionToggle;
    public TMP_Dropdown WindowToggle;

    //Post Processing
    Bloom bloomLayer = null;
    ColorGrading gradingLayer = null;
    AmbientOcclusion occlusionLayer = null;

    PostProcessVolume postProcessing;

    void OnEnable()
    {
        GetSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetSettings()
    {
        postProcessing = FindObjectOfType<PostProcessVolume>();
        Quality.value = QualitySettings.GetQualityLevel();

        for(int i = 0; i < resolutions.Count; i++)
        {
            string Label = resolutions[i].Width.ToString() + "x" + resolutions[i].Height.ToString();
            _resolution.options.Add(new TMP_Dropdown.OptionData(Label));
        }

        postProcessing.profile.TryGetSettings(out bloomLayer);
        postProcessing.profile.TryGetSettings(out gradingLayer);
        postProcessing.profile.TryGetSettings(out occlusionLayer);

        BloomToggle.value = bloomLayer.enabled ? 1 : 0;
        GradingToggle.value = gradingLayer.enabled ? 1 : 0;
        OcclusionToggle.value = occlusionLayer.enabled ? 1 : 0;
    }

    public void ApplyGraphicsSettings()
    {
        QualitySettings.SetQualityLevel(Quality.value);

        //Set post processing
        bloomLayer.enabled.value = BloomToggle.value != 0;
        gradingLayer.enabled.value = GradingToggle.value != 0;
        occlusionLayer.enabled.value = OcclusionToggle.value != 0;
    }
}
