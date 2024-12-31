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

    private int originalQualityIndex;
    private int originalResolutionIndex;
    private float originalVolume;
    private int originalMouseSensitivity;

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

        // Store original settings
        originalResolutionIndex = displayRes.index;
        originalQualityIndex = quality.index;
        originalVolume = volumeSlider.value;
        originalMouseSensitivity = currentMouseSensitivity;
    }

    private void CloseOptionsMenuClicked()
    {
        optionsMenu.style.display = DisplayStyle.None;

        // Revert changes if not applied
        displayRes.index = originalResolutionIndex;
        quality.index = originalQualityIndex;
        volumeSlider.value = originalVolume;
        AudioListener.volume = originalVolume / 100f; // Convert back to 0-1 range
        mouseSensitivitySlider.value = originalMouseSensitivity;
        currentMouseSensitivity = originalMouseSensitivity;
    }

    private void ApplyClicked()
    {
        // Apply changes
        var resolution = Screen.resolutions[displayRes.index];
        Screen.SetResolution(resolution.width, resolution.height, true);
        QualitySettings.SetQualityLevel(quality.index, true);

        currentMouseSensitivity = mouseSensitivitySlider.value;

        // Update stored settings
        originalResolutionIndex = displayRes.index;
        originalQualityIndex = quality.index;
        originalVolume = volumeSlider.value;
        originalMouseSensitivity = currentMouseSensitivity;
    }

    private void InitDisplayRes()
    {
        displayRes = doc.rootVisualElement.Q<DropdownField>("DisplayRes");
        displayRes.choices = Screen.resolutions.Select(res => $"{res.width}x{res.height}").ToList();
        displayRes.index = Screen.resolutions
            .Select((res, index) => (res, index))
            .First(value => value.res.width == Screen.currentResolution.width && value.res.height == Screen.currentResolution.height)
            .index;

        originalResolutionIndex = displayRes.index;
    }

    private void InitQuality()
    {
        quality = doc.rootVisualElement.Q<DropdownField>("Quality");
        quality.choices = QualitySettings.names.ToList();
        quality.index = QualitySettings.GetQualityLevel();

        originalQualityIndex = quality.index;
    }

    private void InitVolumeSlider()
    {
        volumeSlider = doc.rootVisualElement.Q<Slider>("VolumeSlider");
        volumeSlider.lowValue = 0;
        volumeSlider.highValue = 100;
        volumeSlider.value = AudioListener.volume * 100; // Convert from 0-1 to 0-100 range

        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            AudioListener.volume = evt.newValue / 100f; // Convert back to 0-1 range
        });

        originalVolume = volumeSlider.value;
    }

    private void InitMouseSensitivitySlider()
    {
        mouseSensitivitySlider = doc.rootVisualElement.Q<SliderInt>("MouseSensitivitySlider");
        mouseSensitivitySlider.lowValue = 1;
        mouseSensitivitySlider.highValue = 20;
        mouseSensitivitySlider.value = currentMouseSensitivity;

        mouseSensitivitySlider.RegisterValueChangedCallback(evt =>
        {
            currentMouseSensitivity = evt.newValue;
        });

        originalMouseSensitivity = currentMouseSensitivity;
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
