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
    private Slider volumeSlider;
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
        InitVolumeSlider();
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

    private void InitVolumeSlider()
    {
        volumeSlider = doc.rootVisualElement.Q<Slider>("VolumeSlider");
        volumeSlider.lowValue = 0;
        volumeSlider.highValue = 100;
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", AudioListener.volume * 100);
        volumeSlider.RegisterValueChangedCallback(evt => AudioListener.volume = evt.newValue / 100f);
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
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetInt("MouseSensitivity", currentMouseSensitivity);
        PlayerPrefs.Save();
        Debug.Log("Preferences saved.");
    }

    private void LoadPreferences()
    {
        displayRes.index = PlayerPrefs.GetInt("ResolutionIndex", GetDefaultResolutionIndex());
        quality.index = PlayerPrefs.GetInt("QualityIndex", QualitySettings.GetQualityLevel());
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", AudioListener.volume * 100);
        AudioListener.volume = volumeSlider.value / 100f;
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
