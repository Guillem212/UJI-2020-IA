using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class PPEffects : MonoBehaviour
{

    StateControlWaluigi waluigiState;

    PostProcessVolume volume;
    ChromaticAberration chromaticAberrationLayer = null;
    LensDistortion lensDistortionLayer = null;
    MotionBlur motionBlurLayer = null;
    DepthOfField depthOfFieldLayer = null;
    Color detectedColor = new Color(0.9150943f, 0.3f, 0.3f, 1f);
    //PostProcessProfile profile;

    //Bloom bloomLayer = null;
    //ColorGrading colorGradingLayer = null;
    AmbientOcclusion ambientOcclusionLayer = null;
    Vignette vignetteLayer = null;
    ColorGrading colorGradingLayer = null;
    Bloom bloomLayer = null;        

    // Start is called before the first frame update
    void Awake()
    {
        volume = GetComponent<PostProcessVolume>();        

        volume.profile.TryGetSettings<AmbientOcclusion>(out ambientOcclusionLayer);
        volume.profile.TryGetSettings<Vignette>(out vignetteLayer);
        volume.profile.TryGetSettings<ChromaticAberration>(out chromaticAberrationLayer);
        volume.profile.TryGetSettings<LensDistortion>(out lensDistortionLayer);
        volume.profile.TryGetSettings<MotionBlur>(out motionBlurLayer);        
        volume.profile.TryGetSettings<DepthOfField>(out depthOfFieldLayer);
        volume.profile.TryGetSettings<ColorGrading>(out colorGradingLayer);
        volume.profile.TryGetSettings<Bloom>(out bloomLayer);

        //motionBlurLayer.enabled.value = FindObjectOfType<SettingsManager>().motionBlurSetting;
        //ambientOcclusionLayer.enabled.value = FindObjectOfType<SettingsManager>().ambientOclussionSetting;
        // loomLayer.enabled.value = FindObjectOfType<SettingsManager>().bloomSetting;

        //chromaticAberrationLayer.enabled.value = true;        
        lensDistortionLayer.enabled.value = true;
        lensDistortionLayer.intensity.value = -0.05f;
        colorGradingLayer.colorFilter.value = Color.white;

        waluigiState = FindObjectOfType<StateControlWaluigi>();
    }    

    // Update is called once per frame
    void Update()
    {
        SetOverDetection(waluigiState.m_detectionRatio);
    }        
    
    public void SetAlertFeedbackPP()
    {
        
    }

    public void SetOverDetection(float detectedAmount)
    {        
        //chromaticAberrationLayer.intensity.value = Mathf.Lerp(0f, 1f, detectedAmount);
        lensDistortionLayer.intensity.value = Mathf.Lerp(0f, -60f, detectedAmount);
        colorGradingLayer.saturation.value = Mathf.Lerp(0f, 60f, detectedAmount);        
    }    

    public void EndChangePP()
    {
        colorGradingLayer.colorFilter.value = detectedColor;
        colorGradingLayer.postExposure.value = 1.8f;
        colorGradingLayer.hueShift.value = 15f;
        colorGradingLayer.saturation.value = 100f;
        colorGradingLayer.contrast.value = 38.8f;

    }
}
