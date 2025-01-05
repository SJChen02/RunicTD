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
    private ProgressBar ManaBar;
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

    private VisualElement VictoryScreen;
    private VisualElement LoseScreen;
    private Button MenuButton;
    private Button RetryButton;
    private Button NextLevelButton;
    private Button MenuButton2;
    private Button RetryButton2;

    private int currentMouseSensitivity = 10; // Default sensitivity value
    private SaveManager saveManager;
    private PlayerData playerData;
    private bool victoryProcessed = false; // Flag to prevent multiple increments
    private bool defeatProcessed = false;  // Flag for defeat condition
    private int levelNumber;
    private float manaBarTimer = 0f; // Timer for the ManaBar fill
    public static float manaFillDuration = 10f; // Duration for the progress bar to fill
    public static int manaGain = 20;

    private void Awake()
    {
        doc = GetComponent<UIDocument>();
        saveManager = new SaveManager();
        playerData = saveManager.LoadProgress();

        PauseMenu = doc.rootVisualElement.Q<VisualElement>("PauseMenu");
        transition = doc.rootVisualElement.Q<VisualElement>("Transition");
        OptionsMenu = doc.rootVisualElement.Q<VisualElement>("OptionsMenu");
        VictoryScreen = doc.rootVisualElement.Q<VisualElement>("VictoryScreen");
        LoseScreen = doc.rootVisualElement.Q<VisualElement>("LoseScreen");
        ManaBar = doc.rootVisualElement.Q<ProgressBar>("ManaBar");

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

        MenuButton = doc.rootVisualElement.Q<Button>("MenuButton");
        MenuButton.clicked += ExitGameClicked;

        RetryButton = doc.rootVisualElement.Q<Button>("RetryButton");
        RetryButton.clicked += RetryClicked;

        MenuButton2 = doc.rootVisualElement.Q<Button>("MenuButton2");
        MenuButton2.clicked += ExitGameClicked;

        RetryButton2 = doc.rootVisualElement.Q<Button>("RetryButton2");
        RetryButton2.clicked += RetryClicked;

        NextLevelButton = doc.rootVisualElement.Q<Button>("NextLevelButton");
        NextLevelButton.clicked += NextLevelButtonClicked;

        Scene currentScene = SceneManager.GetActiveScene();
        levelNumber = int.Parse(currentScene.name.Split(' ')[1]);

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
        VictoryScreen.style.display = DisplayStyle.None;
        LoseScreen.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        // Handle Victory Condition
        if (WaveTracker.gameWon && !victoryProcessed)
        {
            VictoryScreen.style.display = DisplayStyle.Flex;
            VictoryScreen.RemoveFromClassList("fade-out");
            if (playerData.highestUnlockedLevel == levelNumber)
            {
                playerData.highestUnlockedLevel++;
                saveManager.SaveProgress(playerData);
            }
            victoryProcessed = true; // Prevent further increments

            // Optionally, enable NextLevelButton if there are more levels
            NextLevelButton.style.display = DisplayStyle.Flex;
        }

        // Handle Defeat Condition
        if (Fortress.health <= 0 && !defeatProcessed)
        {
            LoseScreen.style.display = DisplayStyle.Flex;
            LoseScreen.RemoveFromClassList("fade-out");
            defeatProcessed = true; // Prevent further processing
        }

        // Update Mana Progress Bar
        UpdateManaBar();
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
        VictoryScreen.style.display = DisplayStyle.None;
        LoseScreen.style.display = DisplayStyle.None;
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
        // This reloads the scene completely
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
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

    private void NextLevelButtonClicked()
    {
        Time.timeScale = 1f;
        StartCoroutine(PerformWithDelay(NextScene, 2f));
    }

    private void NextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene is within the build settings range
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more levels to load. This is the last level.");
            // Optionally, you can disable the NextLevelButton or redirect to a main menu
            NextLevelButton.style.display = DisplayStyle.None;
        }
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
    private void UpdateManaBar()
    {
        if (ManaBar != null)
        {
            // Increment timer by delta time
            manaBarTimer += Time.deltaTime;

            // Calculate progress as a percentage of the fill duration
            float progress = manaBarTimer / manaFillDuration;

            // Clamp progress between 0 and 1
            progress = Mathf.Clamp01(progress);

            // Update progress bar value
            ManaBar.value = progress * 100; // ProgressBar's value ranges from 0 to 100

            // Check if progress is complete
            if (manaBarTimer >= manaFillDuration)
            {
                // Reset timer and perform mana creation action
                manaBarTimer = 0f;
                HandleManaCreation();
            }
        }
    }

    private void HandleManaCreation()
    {
        Fortress.mana += manaGain; // add mana
        Debug.Log($"Mana created! Now up to: {Fortress.mana}");
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
            .FirstOrDefault(value => value.res.width == Screen.currentResolution.width && value.res.height == Screen.currentResolution.height)
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