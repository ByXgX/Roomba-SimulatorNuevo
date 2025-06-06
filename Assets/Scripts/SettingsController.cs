using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;

    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string GRAPHICS_QUALITY_KEY = "GraphicsQuality";
    private const string FULLSCREEN_KEY = "Fullscreen";

    void Start()
    {

        SetupQualityDropdown();

        LoadSettings();

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        qualityDropdown.onValueChanged.AddListener(SetGraphicsQuality);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    void SetupQualityDropdown()
    {
        qualityDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (string levelName in QualitySettings.names)
        {
            options.Add(levelName);
        }
        qualityDropdown.AddOptions(options);
    }

    // --- Métodos para cambiar los ajustes ---

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume; 
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(GRAPHICS_QUALITY_KEY, qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Modo pantalla completa: " + isFullscreen);
    }

    // --- Cargar Ajustes al Inicio ---

    void LoadSettings()
    {
        // Cargar Volumen
        if (PlayerPrefs.HasKey(MASTER_VOLUME_KEY))
        {
            float savedVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY);
            masterVolumeSlider.value = savedVolume; 
            SetMasterVolume(savedVolume); 
        } else {
            SetMasterVolume(masterVolumeSlider.value);
        }

        if (PlayerPrefs.HasKey(GRAPHICS_QUALITY_KEY))
        {
            int savedQuality = PlayerPrefs.GetInt(GRAPHICS_QUALITY_KEY);
            qualityDropdown.value = savedQuality; 
            SetGraphicsQuality(savedQuality); 
        } else {
            SetGraphicsQuality(qualityDropdown.value); 
        }


        // Cargar Pantalla Completa
        if (PlayerPrefs.HasKey(FULLSCREEN_KEY))
        {
            bool savedFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY) == 1;
            fullscreenToggle.isOn = savedFullscreen; 
            SetFullscreen(savedFullscreen); 
        } else {
            SetFullscreen(fullscreenToggle.isOn); 
        }
    }
}
