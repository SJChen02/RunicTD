using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HUD_Events : MonoBehaviour
{
    private UIDocument doc;
    private VisualElement PauseMenu;
    private VisualElement transition;
    private Button PauseButton;
    private Button exit;
    private Button options;
    private Button retry;
    private Button exitGame;
    private Button exitOptions;
    private VisualElement OptionsMenu;

    private DropdownField displayRes;
    private DropdownField quality;
    private Slider volumeSlider;
    private SliderInt mouseSensitivitySlider;
    private Button ApplyButton;
    private Button CancelButton;

    private int currentMouseSensitivity = 10; // Default sensitivity value

    private void Awake()
    {
        doc = GetComponent<UIDocument>();

        PauseMenu = doc.rootVisualElement.Q<VisualElement>("PauseMenu");
        transition = doc.rootVisualElement.Q<VisualElement>("Transition");
        OptionsMenu = doc.rootVisualElement.Q<VisualElement>("OptionsMenu");

        PauseButton = doc.rootVisualElement.Q<Button>("PauseButton");
        PauseButton.clicked += PauseButtonClicked;

        exit = doc.rootVisualElement.Q<Button>("exit");
        exit.clicked += CloseClicked;

        options = doc.rootVisualElement.Q<Button>("options");
        options.clicked += OptionsClicked;

        retry = doc.rootVisualElement.Q<Button>("retry");
        retry.clicked += RetryClicked;

        exitGame = doc.rootVisualElement.Q<Button>("exitGame");
        exitGame.clicked += ExitGameClicked;

        exitOptions = doc.rootVisualElement.Q<Button>("exitOptions");
        exitOptions.clicked += CloseClicked;

        ApplyButton = doc.rootVisualElement.Q<Button>("ApplyButton");
        ApplyButton.clicked += ApplyClicked;

        CancelButton = doc.rootVisualElement.Q<Button>("CancelButton");
        CancelButton.clicked += CloseClicked;

        // Initialize UI elements
        InitDisplayRes();
        InitQuality();
        InitVolumeSlider();
        InitMouseSensitivitySlider();
    }

    private void Start()
    {
        StartSceneFadeIn();
        PauseMenu.style.display = DisplayStyle.None;
        OptionsMenu.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    private void PauseButtonClicked()
    {
        PauseMenu.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
    }

    private void CloseClicked()
    {
        PauseMenu.style.display = DisplayStyle.None;
        OptionsMenu.style.display = DisplayStyle.None;
        Time.timeScale = 1f;

        // Reload preferences to revert any unsaved changes
        LoadPreferences();
    }

    private void OptionsClicked()
    {
        OptionsMenu.style.display = DisplayStyle.Flex;
        PauseMenu.style.display = DisplayStyle.None;
    }

    private void RetryClicked()
    {
        Time.timeScale = 1f;
        StartCoroutine(PerformWithDelay(ReloadScene, 2f));
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ExitGameClicked()
    {
        Time.timeScale = 1f;
        StartCoroutine(PerformWithDelay(ExitScene, 2f));
    }

    private void ExitScene()
    {
        SceneManager.LoadScene("Level Selection");
    }

    private void ApplyClicked()
    {
        // Apply resolution
        var resolution = Screen.resolutions[displayRes.index];
        Screen.SetResolution(resolution.width, resolution.height, true);

        // Apply quality settings
        QualitySettings.SetQualityLevel(quality.index, true);

        // Apply mouse sensitivity
        currentMouseSensitivity = mouseSensitivitySlider.value;

        // Save preferences
        SavePreferences();
    }

    private void InitDisplayRes()
    {
        displayRes = doc.rootVisualElement.Q<DropdownField>("displayRes");
        displayRes.choices = Screen.resolutions.Select(res => $"{res.width}x{res.height}").ToList();

        // Load saved resolution index or use default
        int savedResIndex = PlayerPrefs.GetInt("ResolutionIndex", GetDefaultResolutionIndex());
        displayRes.index = savedResIndex;
    }

    private void InitQuality()
    {
        quality = doc.rootVisualElement.Q<DropdownField>("quality");
        quality.choices = QualitySettings.names.ToList();

        // Load saved quality level or use current
        quality.index = PlayerPrefs.GetInt("QualityIndex", QualitySettings.GetQualityLevel());
    }

    private void InitVolumeSlider()
    {
        volumeSlider = doc.rootVisualElement.Q<Slider>("volumeSlider");
        volumeSlider.lowValue = 0;
        volumeSlider.highValue = 100;

        // Load saved volume or use current
        float savedVolume = PlayerPrefs.GetFloat("Volume", AudioListener.volume * 100);
        volumeSlider.value = savedVolume;

        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            AudioListener.volume = evt.newValue / 100f; // Convert to 0-1 range
        });
    }

    private void InitMouseSensitivitySlider()
    {
        mouseSensitivitySlider = doc.rootVisualElement.Q<SliderInt>("mouseSensitivitySlider");
        mouseSensitivitySlider.lowValue = 1;
        mouseSensitivitySlider.highValue = 20;

        // Load saved sensitivity or use default
        currentMouseSensitivity = PlayerPrefs.GetInt("MouseSensitivity", 10);
        mouseSensitivitySlider.value = currentMouseSensitivity;

        mouseSensitivitySlider.RegisterValueChangedCallback(evt =>
        {
            currentMouseSensitivity = evt.newValue;
        });
    }

    private void SavePreferences()
    {
        PlayerPrefs.SetInt("ResolutionIndex", displayRes.index);
        PlayerPrefs.SetInt("QualityIndex", quality.index);
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
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

        // Reload volume
        float volume = PlayerPrefs.GetFloat("Volume", AudioListener.volume * 100);
        volumeSlider.value = volume;
        AudioListener.volume = volume / 100f;

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

    private void StartSceneFadeIn()
    {
        transition.style.display = DisplayStyle.Flex;
        transition.AddToClassList("transition-In");
        StartCoroutine(RemoveClassAfterDelay("transition-In", 0f));
    }

    private IEnumerator PerformWithDelay(Action action, float delay)
    {
        transition.AddToClassList("transition-In");
        yield return new WaitForSecondsRealtime(delay);
        action?.Invoke();
    }

    private IEnumerator RemoveClassAfterDelay(string className, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        transition.RemoveFromClassList(className);
    }
}
