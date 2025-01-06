using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    private UIDocument doc;
    private VisualElement optionsMenu, transition;
    private Button play, exit, options, closeOptionsMenu, cancelOptionMenu, apply;
    private DropdownField displayRes, quality;
    private Slider MusicVolumeSlider;
    private Slider MasterVolumeSlider;
    private Slider SoundEffectVolumeSlider;
    private SliderInt mouseSensitivitySlider;

    private int currentMouseSensitivity = 10;

    private void Awake()
    {
        // Initialize UI elements
        doc = GetComponent<UIDocument>();
        play = doc.rootVisualElement.Q<Button>("play");
        exit = doc.rootVisualElement.Q<Button>("exit");
        options = doc.rootVisualElement.Q<Button>("options");
        closeOptionsMenu = doc.rootVisualElement.Q<Button>("closeButton");
        cancelOptionMenu = doc.rootVisualElement.Q<Button>("Cancel");
        apply = doc.rootVisualElement.Q<Button>("Apply");

        optionsMenu = doc.rootVisualElement.Q<VisualElement>("optionsMenu");
        transition = doc.rootVisualElement.Q<VisualElement>("Transition");

        // Button click events
        play.clicked += PlayClicked;
        exit.clicked += ExitClicked;
        options.clicked += OptionsClicked;
        closeOptionsMenu.clicked += CloseOptionsMenuClicked;
        cancelOptionMenu.clicked += CloseOptionsMenuClicked;
        apply.clicked += ApplyClicked;

        // Initialize UI components
        InitDisplayRes();
        InitQuality();
        InitMasterVolumeSlider();
        InitMusicVolumeSlider();
        InitSoundEffectVolumeSlider();
        InitMouseSensitivitySlider();

        transition.style.display = DisplayStyle.Flex;
    }

    private void Start()
    {
        // Fade-in effect when scene loads
        StartSceneFadeIn();
        optionsMenu.style.display = DisplayStyle.None;
    }

    private void PlayClicked() => StartCoroutine(PerformWithDelay(ChangeScene, 2f));
    private void ChangeScene() => SceneManager.LoadScene("Level Selection");
    private void ExitClicked() => Application.Quit();
    private void OptionsClicked() => optionsMenu.style.display = DisplayStyle.Flex;
    private void CloseOptionsMenuClicked() { optionsMenu.style.display = DisplayStyle.None; LoadPreferences(); }

    private void ApplyClicked()
    {
        // Apply and save changes
        var resolution = Screen.resolutions[displayRes.index];
        Screen.SetResolution(resolution.width, resolution.height, true);
        QualitySettings.SetQualityLevel(quality.index, true);
        currentMouseSensitivity = mouseSensitivitySlider.value;
        SavePreferences();
    }

    private void InitDisplayRes()
    {
        displayRes = doc.rootVisualElement.Q<DropdownField>("DisplayRes");
        displayRes.choices = Screen.resolutions.Select(res => $"{res.width}x{res.height}").ToList();
        displayRes.index = PlayerPrefs.GetInt("ResolutionIndex", GetDefaultResolutionIndex());
    }

    private void InitQuality()
    {
        quality = doc.rootVisualElement.Q<DropdownField>("Quality");
        quality.choices = QualitySettings.names.ToList();
        quality.index = PlayerPrefs.GetInt("QualityIndex", QualitySettings.GetQualityLevel());
    }

    private void InitMasterVolumeSlider()
    {
        MasterVolumeSlider = doc.rootVisualElement.Q<Slider>("MasterVolumeSlider");
        MasterVolumeSlider.lowValue = 0;
        MasterVolumeSlider.highValue = 100;

        // Load saved master volume or use default
        float savedMasterVolume = PlayerPrefs.GetFloat("MasterVolume", SoundManager.MasterVolume * 100);
        MasterVolumeSlider.value = savedMasterVolume;

        MasterVolumeSlider.RegisterValueChangedCallback(evt =>
        {
            SoundManager.MasterVolume = evt.newValue / 100f; // Convert to 0-1 range
            SoundManager.UpdateMusicVolume(); // Ensure all audio sources are updated
        });
    }

    private void InitMusicVolumeSlider()
    {
        MusicVolumeSlider = doc.rootVisualElement.Q<Slider>("MusicVolumeSlider");
        MusicVolumeSlider.lowValue = 0;
        MusicVolumeSlider.highValue = 100;

        // Load saved music volume or use default
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", SoundManager.MusicVolume * 100);
        MusicVolumeSlider.value = savedMusicVolume;

        MusicVolumeSlider.RegisterValueChangedCallback(evt =>
        {
            SoundManager.MusicVolume = evt.newValue / 100f; // Convert to 0-1 range
            SoundManager.UpdateMusicVolume(); // Update the background music source
        });
    }

    private void InitSoundEffectVolumeSlider()
    {
        SoundEffectVolumeSlider = doc.rootVisualElement.Q<Slider>("SoundEffectVolumeSlider");
        SoundEffectVolumeSlider.lowValue = 0;
        SoundEffectVolumeSlider.highValue = 100;

        // Load saved sound effect volume or use default
        float savedSoundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume", SoundManager.SoundEffectVolume * 100);
        SoundEffectVolumeSlider.value = savedSoundEffectVolume;

        SoundEffectVolumeSlider.RegisterValueChangedCallback(evt =>
        {
            SoundManager.SoundEffectVolume = evt.newValue / 100f; // Convert to 0-1 range
        });
    }

    private void InitMouseSensitivitySlider()
    {
        mouseSensitivitySlider = doc.rootVisualElement.Q<SliderInt>("MouseSensitivitySlider");
        mouseSensitivitySlider.lowValue = 1;
        mouseSensitivitySlider.highValue = 20;
        mouseSensitivitySlider.value = PlayerPrefs.GetInt("MouseSensitivity", 10);
        mouseSensitivitySlider.RegisterValueChangedCallback(evt => currentMouseSensitivity = evt.newValue);
    }

    private void SavePreferences()
    {
        PlayerPrefs.SetInt("ResolutionIndex", displayRes.index);
        PlayerPrefs.SetInt("QualityIndex", quality.index);
        PlayerPrefs.SetFloat("Volume", MasterVolumeSlider.value); // Overall volume
        PlayerPrefs.SetFloat("MasterVolume", MasterVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolumeSlider.value);
        PlayerPrefs.SetFloat("SoundEffectVolume", SoundEffectVolumeSlider.value);
        PlayerPrefs.SetInt("MouseSensitivity", currentMouseSensitivity);

        PlayerPrefs.Save();
        Debug.Log("Preferences saved.");
    }

    private void LoadPreferences()
    {
        // Reload resolution
        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", GetDefaultResolutionIndex());
        displayRes.index = resolutionIndex;

        // Reload quality
        int qualityIndex = PlayerPrefs.GetInt("QualityIndex", QualitySettings.GetQualityLevel());
        quality.index = qualityIndex;

        // Reload volumes
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", SoundManager.MasterVolume * 100);
        MasterVolumeSlider.value = masterVolume;
        SoundManager.MasterVolume = masterVolume / 100f;

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", SoundManager.MusicVolume * 100);
        MusicVolumeSlider.value = musicVolume;
        SoundManager.MusicVolume = musicVolume / 100f;

        float soundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume", SoundManager.SoundEffectVolume * 100);
        SoundEffectVolumeSlider.value = soundEffectVolume;
        SoundManager.SoundEffectVolume = soundEffectVolume / 100f;

        // Reload mouse sensitivity
        currentMouseSensitivity = PlayerPrefs.GetInt("MouseSensitivity", 10);
        mouseSensitivitySlider.value = currentMouseSensitivity;

        Debug.Log("Preferences loaded.");
    }

    private int GetDefaultResolutionIndex()
    {
        return Screen.resolutions
            .Select((res, index) => (res, index))
            .First(value => value.res.width == Screen.currentResolution.width && value.res.height == Screen.currentResolution.height)
            .index;
    }

    private IEnumerator PerformWithDelay(Action action, float delay)
    {
        transition.AddToClassList("transition-In");
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    private void StartSceneFadeIn() => StartCoroutine(RemoveClassAfterDelay("transition-In", 0f));

    private IEnumerator RemoveClassAfterDelay(string className, float delay)
    {
        yield return new WaitForSeconds(delay);
        transition.RemoveFromClassList(className);
    }
}