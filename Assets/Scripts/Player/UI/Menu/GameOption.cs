using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class GameOption : MonoBehaviour
{
    [Header("조작 설정")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Text mouseSensitivityText;
    [Header("화면 설정")]
    //[SerializeField] private 
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Text brightnessText;
    private PostProcessVolume ppv;
    private ColorGrading colorGrading;
    [Header("오디오 설정")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text volumeText;
    [SerializeField] private AudioMixer audioMixer;

    // Start is called before the first frame update
    void Start()
    {
        // 초기화
        InitControlSettings();
        InitVideoSettings();
        InitAudioSettings();
    }

    // 조작 설정
    private void InitControlSettings()
    {
        if (!PlayerPrefs.HasKey("MouseSensitivity"))
        {
            PlayerPrefs.SetFloat("MouseSensitivity", 1.0f);
        }

        PlayerCameraMovement.mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        mouseSensitivitySlider.value = PlayerCameraMovement.mouseSensitivity;
        mouseSensitivityText.text = PlayerCameraMovement.mouseSensitivity.ToString("0.0");
    }

    public void ChangeControlValue(Text valueText)
    {
        // UI 변경
        valueText.text = mouseSensitivitySlider.value.ToString("0.0");

        // 감도 변경
        PlayerCameraMovement.mouseSensitivity = mouseSensitivitySlider.value;

        // PlayerPrefs 저장
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivitySlider.value);
        PlayerPrefs.Save();
    }


    // 화면 설정
    private void InitVideoSettings()
    {
        ppv = Camera.main.GetComponent<PostProcessVolume>();
        ppv.profile.TryGetSettings(out colorGrading); // 포스트 프로세싱 프로필 값 가져오기

        if (!PlayerPrefs.HasKey("Brightness"))
        {
            PlayerPrefs.SetFloat("Brightness", 0.3f);
        }

        colorGrading.gamma.value.w = PlayerPrefs.GetFloat("Brightness");
        brightnessSlider.value = colorGrading.gamma.value.w; // w가 감마
        brightnessText.text = colorGrading.gamma.value.w.ToString("0.0");
    }

    public void ChangeVideoValue(Text valueText)
    {
        // 값 변경
        valueText.text = brightnessSlider.value.ToString("0.0");

        // Contrast 변경
        colorGrading.gamma.value.w = brightnessSlider.value;

        // PlayerPrefs 저장
        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);
        PlayerPrefs.Save();
    }

    // 오디오 설정
    private void InitAudioSettings()
    {
        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", 70.0f);
        }

        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume") - 80f);
        float volume;
        audioMixer.GetFloat("MasterVolume", out volume);
        volumeSlider.value = volume + 80f;
        volumeText.text = (volume + 80f).ToString("0.0");
    }

    public void ChangeAudioValue(Text valueText)
    {
        // 값 변경
        valueText.text = volumeSlider.value.ToString("0.0");

        // 볼륨 변경
        audioMixer.SetFloat("MasterVolume", volumeSlider.value - 80f);

        // PlayerPrefs 저장
        PlayerPrefs.SetFloat("MasterVolume", volumeSlider.value);
        PlayerPrefs.Save();
    }
}