using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    private UIDocument doc;
    private VisualElement optionsMenu;
    private VisualElement transition;

    private Button play;
    private Button exit;
    private Button options;
    private Button closeOptionsMenu;
    private Button cancelOptionMenu;
    private Button apply;

    private DropdownField displayRes;
    private DropdownField quality;
    private Slider volumeSlider;
    private SliderInt mouseSensitivitySlider;

    private int currentMouseSensitivity = 10; // Default sensitivity value

    private void Awake()
    {
        // Get the UIDocument component and query UI elements
        doc = GetComponent<UIDocument>();

        play = doc.rootVisualElement.Q<Button>("play");
        play.clicked += PlayClicked;

        exit = doc.rootVisualElement.Q<Button>("exit");
        exit.clicked += ExitClicked;

        options = doc.rootVisualElement.Q<Button>("options");
        options.clicked += OptionsClicked;

        closeOptionsMenu = doc.rootVisualElement.Q<Button>("closeButton");
        closeOptionsMenu.clicked += CloseOptionsMenuClicked;

        optionsMenu = doc.rootVisualElement.Q<VisualElement>("optionsMenu");
        transition = doc.rootVisualElement.Q<VisualElement>("Transition");

        cancelOptionMenu = doc.rootVisualElement.Q<Button>("Cancel");
        cancelOptionMenu.clicked += CloseOptionsMenuClicked;

        apply = doc.rootVisualElement.Q<Button>("Apply");
        apply.clicked += ApplyClicked;

        // Initialize UI elements
        InitDisplayRes();
        InitQuality();
        InitVolumeSlider();
        InitMouseSensitivitySlider();
        transition.style.display = DisplayStyle.Flex;
    }

    private void Start()
    {
        // Trigger the fade-in animation when entering the scene
        StartSceneFadeIn();

        // Hide the options menu at the start
        optionsMenu.style.display = DisplayStyle.None;
    }

    private void PlayClicked()
    {
        StartCoroutine(PerformWithDelay(ChangeScene, 2f));
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("Level Selection");
    }

    private void ExitClicked()
    {
        Application.Quit();
    }

    private void OptionsClicked()
    {
        optionsMenu.style.display = DisplayStyle.Flex;
    }

    private void CloseOptionsMenuClicked()
    {
        optionsMenu.style.display = DisplayStyle.None;

        // Reload stored settings
        LoadPreferences();
    }

    private void ApplyClicked()
    {
        // Apply changes
        var resolution = Screen.resolutions[displayRes.index];
        Screen.SetResolution(resolution.width, resolution.height, true);
        QualitySettings.SetQualityLevel(quality.index, true);

        currentMouseSensitivity = mouseSensitivitySlider.value;

        // Save preferences
        SavePreferences();
    }

    private void InitDisplayRes()
    {
        displayRes = doc.rootVisualElement.Q<DropdownField>("DisplayRes");
        displayRes.choices = Screen.resolutions.Select(res => $"{res.width}x{res.height}").ToList();

        // Load saved resolution index or use default
        int savedResIndex = PlayerPrefs.GetInt("ResolutionIndex", GetDefaultResolutionIndex());
        displayRes.index = savedResIndex;
    }

    private void InitQuality()
    {
        quality = doc.rootVisualElement.Q<DropdownField>("Quality");
        quality.choices = QualitySettings.names.ToList();

        // Load saved quality level or use current
        quality.index = PlayerPrefs.GetInt("QualityIndex", QualitySettings.GetQualityLevel());
    }

    private void InitVolumeSlider()
    {
        volumeSlider = doc.rootVisualElement.Q<Slider>("VolumeSlider");
        volumeSlider.lowValue = 0;
        volumeSlider.highValue = 100;

        // Load saved volume or use current
        float savedVolume = PlayerPrefs.GetFloat("Volume", AudioListener.volume * 100);
        volumeSlider.value = savedVolume;

        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            AudioListener.volume = evt.newValue / 100f; // Convert back to 0-1 range
        });
    }

    private void InitMouseSensitivitySlider()
    {
        mouseSensitivitySlider = doc.rootVisualElement.Q<SliderInt>("MouseSensitivitySlider");
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

    private IEnumerator PerformWithDelay(Action action, float delay)
    {
        transition.AddToClassList("transition-In");
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        action?.Invoke(); // Execute the action
    }

    private void StartSceneFadeIn()
    {
        transition.AddToClassList("transition-In");

        StartCoroutine(RemoveClassAfterDelay("transition-In", 0f));
    }

    private IEnumerator RemoveClassAfterDelay(string className, float delay)
    {
        yield return new WaitForSeconds(delay);

        transition.RemoveFromClassList(className);
    }
}
