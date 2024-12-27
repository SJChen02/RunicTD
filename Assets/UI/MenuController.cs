using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    private UIDocument doc;
    private VisualElement optionsMenu;

    private Button play;
    private Button exit;
    private Button options;

    private Button closeOptionsMenu;
    private Button cancelOptionMenu;
    private Button Apply;

    private DropdownField DisplayRes;
    private DropdownField Quality;
    private Slider VolumeSlider;
    private SliderInt MouseSensitivitySlider;

    private int originalQualityIndex;
    private int originalResolutionIndex;
    private float originalVolume;
    private int originalMouseSensitivity;

    private int currentMouseSensitivity = 10; // Default sensitivity value

    private void Awake()
    {
        // Get the UIDocument component
        doc = GetComponent<UIDocument>();

        // Query buttons and options menu
        play = doc.rootVisualElement.Q<Button>("play");
        if (play != null)
            play.clicked += PlayClicked;

        exit = doc.rootVisualElement.Q<Button>("exit");
        if (exit != null)
            exit.clicked += ExitClicked;

        options = doc.rootVisualElement.Q<Button>("options");
        if (options != null)
            options.clicked += OptionsClicked;

        closeOptionsMenu = doc.rootVisualElement.Q<Button>("closeButton");
        if (closeOptionsMenu != null)
            closeOptionsMenu.clicked += CloseOptionsMenuClicked;

        optionsMenu = doc.rootVisualElement.Q<VisualElement>("optionsMenu");

        cancelOptionMenu = doc.rootVisualElement.Q<Button>("Cancel");
        if (cancelOptionMenu != null)
            cancelOptionMenu.clicked += CloseOptionsMenuClicked;

        Apply = doc.rootVisualElement.Q<Button>("Apply");
        if (Apply != null)
            Apply.clicked += ApplyClicked;

        // Initialize UI elements
        InitDisplayRes();
        InitQuality();
        InitVolumeSlider();
        InitMouseSensitivitySlider();
    }

    private void Start()
    {
        // Hide the options menu at the start
        if (optionsMenu != null)
            optionsMenu.style.display = DisplayStyle.None;
    }

    private void PlayClicked()
    {
        SceneManager.LoadScene("Level 1");
    }

    private void ExitClicked()
    {
        Application.Quit();
    }

    private void OptionsClicked()
    {
        if (optionsMenu != null)
        {
            optionsMenu.style.display = DisplayStyle.Flex;

            // Store original settings
            originalResolutionIndex = DisplayRes.index;
            originalQualityIndex = Quality.index;
            originalVolume = VolumeSlider.value;
            originalMouseSensitivity = currentMouseSensitivity;
        }
    }

    private void CloseOptionsMenuClicked()
    {
        if (optionsMenu != null)
        {
            optionsMenu.style.display = DisplayStyle.None;

            // Revert changes if not applied
            DisplayRes.index = originalResolutionIndex;
            Quality.index = originalQualityIndex;
            VolumeSlider.value = originalVolume;
            AudioListener.volume = originalVolume / 100f; // Convert back to 0-1 range
            MouseSensitivitySlider.value = originalMouseSensitivity;
            currentMouseSensitivity = originalMouseSensitivity;
        }
    }

    private void ApplyClicked()
    {
        // Apply changes
        var resolution = Screen.resolutions[DisplayRes.index];
        Screen.SetResolution(resolution.width, resolution.height, true);
        QualitySettings.SetQualityLevel(Quality.index, true);

        currentMouseSensitivity = MouseSensitivitySlider.value;

        // Update stored settings
        originalResolutionIndex = DisplayRes.index;
        originalQualityIndex = Quality.index;
        originalVolume = VolumeSlider.value;
        originalMouseSensitivity = currentMouseSensitivity;
    }

    private void InitDisplayRes()
    {
        DisplayRes = doc.rootVisualElement.Q<DropdownField>("DisplayRes");
        if (DisplayRes == null)
            return;

        DisplayRes.choices = Screen.resolutions.Select(resolution => $"{resolution.width}x{resolution.height}").ToList();
        DisplayRes.index = Screen.resolutions
            .Select((resolution, index) => (resolution, index))
            .First(value => value.resolution.width == Screen.currentResolution.width && value.resolution.height == Screen.currentResolution.height)
            .index;

        originalResolutionIndex = DisplayRes.index;
    }

    private void InitQuality()
    {
        Quality = doc.rootVisualElement.Q<DropdownField>("Quality");
        if (Quality == null)
            return;

        Quality.choices = QualitySettings.names.ToList();
        Quality.index = QualitySettings.GetQualityLevel();

        originalQualityIndex = Quality.index;
    }

    private void InitVolumeSlider()
    {
        VolumeSlider = doc.rootVisualElement.Q<Slider>("VolumeSlider");
        if (VolumeSlider == null)
        {
            Debug.LogError("VolumeSlider not found!");
            return;
        }

        VolumeSlider.lowValue = 0;
        VolumeSlider.highValue = 100;
        VolumeSlider.value = AudioListener.volume * 100; // Convert from 0-1 to 0-100 range

        VolumeSlider.RegisterValueChangedCallback(evt =>
        {
            AudioListener.volume = evt.newValue / 100f; // Convert back to 0-1 range
        });

        originalVolume = VolumeSlider.value;
    }

    private void InitMouseSensitivitySlider()
    {
        MouseSensitivitySlider = doc.rootVisualElement.Q<SliderInt>("MouseSensitivitySlider");
        if (MouseSensitivitySlider == null)
        {
            Debug.LogError("MouseSensitivitySlider not found!");
            return;
        }

        MouseSensitivitySlider.lowValue = 1;
        MouseSensitivitySlider.highValue = 20;
        MouseSensitivitySlider.value = currentMouseSensitivity;

        MouseSensitivitySlider.RegisterValueChangedCallback(evt =>
        {
            Debug.Log($"Mouse sensitivity changed to: {evt.newValue}");
            currentMouseSensitivity = evt.newValue;
        });

        originalMouseSensitivity = currentMouseSensitivity;
    }
}
