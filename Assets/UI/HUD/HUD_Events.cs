using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HUD_Events : MonoBehaviour
{
    private UIDocument doc;
    private SaveManager saveManager;
    private PlayerData playerData;
    private int levelNumber;
    private bool victoryProcessed = false;
    private bool defeatProcessed = false;

    private VisualElement PauseMenu, HUD_Element, transition, OptionsMenu, VictoryScreen, TutorialBackground, TutorialPage1, TutorialPage2, Lore;
    private Label Level1, Level2, Level3, Level4, Level5, Level6, ending, credits;

    private Button PauseButton, exit, options, retry, exitGame, exitOptions, TutorialButton, TutorialEndButton, ApplyButton, CancelButton, MenuButton, RetryButton, NextLevelButton;

    private ProgressBar ManaBar;

    private DropdownField displayRes, quality;
    private Slider MusicVolumeSlider, MasterVolumeSlider, SoundEffectVolumeSlider;
    private SliderInt mouseSensitivitySlider;

    private int currentMouseSensitivity = 10; // Default sensitivity value
    private float manaBarTimer = 0f;
    public static float manaFillDuration = 10f; // Duration for the progress bar to fill
    public static int manaGain = 50;
    private void Start()
    {
        StartSceneFadeIn();

        PauseMenu.style.display = DisplayStyle.None;
        OptionsMenu.style.display = DisplayStyle.None;
        VictoryScreen.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
        StartCoroutine(ShowLevelIntro());

        if (!playerData.Tutorial)
        {
            TutorialBackground.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
        }
    }

    private void Update()
    {
        // Handle Victory Condition
        if (WaveTracker.gameWon && !victoryProcessed)
        {
            if (levelNumber == 6) // Check if it's Level 6
            {
                TriggerEnding();
            }
            else
            {
                VictoryScreen.style.display = DisplayStyle.Flex;
                VictoryScreen.RemoveFromClassList("fade-out");
                VictoryScreen.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Visuals/VICTORY"));
                if (playerData.highestUnlockedLevel == levelNumber)
                {
                    playerData.highestUnlockedLevel++;
                    saveManager.SaveProgress(playerData);
                }
                victoryProcessed = true; // Prevent further increments

                // Optionally, enable NextLevelButton if there are more levels
                NextLevelButton.style.display = DisplayStyle.Flex;
            }
        }

        // Handle Defeat Condition
        if (Fortress.health <= 0 && !defeatProcessed)
        {
            NextLevelButton.style.display = DisplayStyle.None;
            VictoryScreen.style.display = DisplayStyle.Flex;
            VictoryScreen.RemoveFromClassList("fade-out");
            VictoryScreen.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Visuals/game over"));
            defeatProcessed = true; // Prevent further processing
        }

        // Update Mana Progress Bar
        UpdateManaBar();
    }

    private void PauseButtonClicked()
    {
        PauseMenu.style.display = DisplayStyle.Flex;
        HUD_Element.pickingMode = PickingMode.Position;
        Time.timeScale = 0f;
    }

    private void CloseClicked()
    {
        PauseMenu.style.display = DisplayStyle.None;
        OptionsMenu.style.display = DisplayStyle.None;
        VictoryScreen.style.display = DisplayStyle.None;
        HUD_Element.pickingMode = PickingMode.Ignore;
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
        Fortress.mana += manaGain; // Add 20 mana
        Debug.Log($"Mana created! Mana up to: {Fortress.mana}");
    }

    private void TutorialButtonClicked()
    {
        TutorialPage1.style.display = DisplayStyle.None;
        TutorialPage2.style.display = DisplayStyle.Flex;
    }

    private void TutorialEndButtonClicked()
    {
        TutorialBackground.style.display = DisplayStyle.None;
        playerData.Tutorial = true;
        saveManager.SaveProgress(playerData);
        Time.timeScale = 1f;
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

    private IEnumerator ShowLevelIntro()
    {
        // Debug: Confirm level number
        Debug.Log($"Starting level intro for level {levelNumber}");

        // Find the label corresponding to the current level
        Label levelLabel = GetLevelLabel(levelNumber);
        if (levelLabel != null)
        {
            levelLabel.style.display = DisplayStyle.Flex; // Show level label
            Debug.Log($"Level label '{levelLabel.text}' is now visible.");
        }
        else
        {
            Debug.LogError($"No label found for level {levelNumber}");
        }

        // Show the lore visual element
        Lore.style.display = DisplayStyle.Flex; // Show lore element
        Lore.RemoveFromClassList("fade-out");
        Debug.Log("Lore element is now visible.");

        // Pause the game
        Time.timeScale = 0f;

        // Wait for 10 seconds
        yield return new WaitForSecondsRealtime(11f);

        // Hide the level label and lore
        if (levelLabel != null)
        {
            levelLabel.style.display = DisplayStyle.None; // Hide level label
            Debug.Log($"Level label '{levelLabel.text}' is now hidden.");
        }
        Lore.AddToClassList("fade-out");
        Lore.style.display = DisplayStyle.None; // Hide lore element
        Debug.Log("Lore element is now hidden.");

        // Resume the game
        Time.timeScale = 1f;
        if (!playerData.Tutorial)
        {
            Time.timeScale = 0f;
        }
        Debug.Log("Game resumed.");
    }

    private Label GetLevelLabel(int level)
    {
        // Return the label corresponding to the level number
        switch (level)
        {
            case 1: return Level1;
            case 2: return Level2;
            case 3: return Level3;
            case 4: return Level4;
            case 5: return Level5;
            case 6: return Level6;
            default: return null;
        }
    }

    private void TriggerEnding()
    {
        Debug.Log("Level 6 completed. Triggering ending sequence.");

        // Start the ending sequence coroutine
        StartCoroutine(ShowEnding());

        victoryProcessed = true; // Prevent further increments
    }

    private IEnumerator ShowEnding()
    {
        // Display the lore and ending elements
        Lore.style.display = DisplayStyle.Flex;
        Lore.RemoveFromClassList("fade-out");
        ending.style.display = DisplayStyle.Flex;

        // Pause the game
        Time.timeScale = 0f;

        // Show the ending label for 15 seconds
        yield return new WaitForSecondsRealtime(15f);

        // Hide the ending label
        ending.style.display = DisplayStyle.None;

        // Show the credits label
        credits.style.display = DisplayStyle.Flex;
        Debug.Log("Credits are now displayed.");

        // Show the credits for 8 seconds
        yield return new WaitForSecondsRealtime(8f);

        // Hide the credits label and the lore
        credits.style.display = DisplayStyle.None;
        Lore.AddToClassList("fade-out");
        Lore.style.display = DisplayStyle.None;

        Debug.Log("Credits and lore elements are now hidden.");

        // Resume the game and redirect to the main menu
        Time.timeScale = 1f;
        GoToMainMenu();
    }

    private void GoToMainMenu()
    {
        Debug.Log("Redirecting to Main Menu.");
        SceneManager.LoadScene("Main Menu"); // Replace with your main menu scene name
    }

    private void Awake()
    {
        // Initialize UI elements
        doc = GetComponent<UIDocument>();
        saveManager = new SaveManager();
        playerData = saveManager.LoadProgress();

        PauseMenu = doc.rootVisualElement.Q<VisualElement>("PauseMenu");
        HUD_Element = doc.rootVisualElement.Q<VisualElement>("HUD_Element");
        transition = doc.rootVisualElement.Q<VisualElement>("Transition");
        OptionsMenu = doc.rootVisualElement.Q<VisualElement>("OptionsMenu");
        VictoryScreen = doc.rootVisualElement.Q<VisualElement>("VictoryScreen");
        ManaBar = doc.rootVisualElement.Q<ProgressBar>("ManaBar");
        TutorialBackground = doc.rootVisualElement.Q<VisualElement>("Tutorial");
        TutorialPage1 = doc.rootVisualElement.Q<VisualElement>("TutorialPage1");
        TutorialPage2 = doc.rootVisualElement.Q<VisualElement>("TutorialPage2");
        TutorialButton = doc.rootVisualElement.Q<Button>("TutorialButton");
        TutorialButton.clicked += TutorialButtonClicked;
        Lore = doc.rootVisualElement.Q<VisualElement>("Lore");
        Level1 = doc.rootVisualElement.Q<Label>("Level1");
        Level2 = doc.rootVisualElement.Q<Label>("Level2");
        Level3 = doc.rootVisualElement.Q<Label>("Level3");
        Level4 = doc.rootVisualElement.Q<Label>("Level4");
        Level5 = doc.rootVisualElement.Q<Label>("Level5");
        Level6 = doc.rootVisualElement.Q<Label>("Level6");
        ending = doc.rootVisualElement.Q<Label>("ending");
        credits = doc.rootVisualElement.Q<Label>("credits");

        TutorialEndButton = doc.rootVisualElement.Q<Button>("TutorialEndButton");
        TutorialEndButton.clicked += TutorialEndButtonClicked;

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

        NextLevelButton = doc.rootVisualElement.Q<Button>("NextLevelButton");
        NextLevelButton.clicked += NextLevelButtonClicked;

        Scene currentScene = SceneManager.GetActiveScene();
        levelNumber = int.Parse(currentScene.name.Split(' ')[1]);

        
        InitDisplayRes();
        InitQuality();
        InitMasterVolumeSlider();
        InitMusicVolumeSlider();
        InitSoundEffectVolumeSlider();
        InitMouseSensitivitySlider();
    }
}