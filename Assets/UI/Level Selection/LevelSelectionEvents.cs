using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelSelectionEvents : MonoBehaviour
{
    public static float enemyHealthMulti;
    public static float enemySpeedMulti;
    public static float sellMulti;

    private SaveManager saveManager;
    private PlayerData playerData;

    private UIDocument doc;
    private VisualElement DifficultyMenu;
    private VisualElement transition;
    private VisualElement LoreView;
    private string selectedLevel;

    private void Awake()
    {
        // Initialize SaveManager and load player data
        saveManager = new SaveManager();
        playerData = saveManager.LoadProgress();

        // Initialize UIDocument and UI elements
        doc = GetComponent<UIDocument>();
        DifficultyMenu = doc.rootVisualElement.Q<VisualElement>("DifficultyMenu");
        transition = doc.rootVisualElement.Q<VisualElement>("Transition");
        LoreView = doc.rootVisualElement.Q<VisualElement>("LoreView");

        if (transition == null)
        {
            Debug.LogError("Transition element not found!");
            return;
        }

        // Initially hide the DifficultyMenu
        DifficultyMenu.style.display = DisplayStyle.None;
        transition.style.display = DisplayStyle.Flex;

        // Map buttons to actions
        AssignButton("CloseButton", () => ToggleDifficultyMenu(false));
        AssignButton("BackButton", () => StartCoroutine(PerformWithDelay(() => SceneManager.LoadScene("Main Menu"), 2f)));
        AssignButton("closeLoreButton", () => LoreView.style.display = DisplayStyle.None);
        AssignButton("LoreButton", () => LoreView.style.display = DisplayStyle.Flex);

        // Map level buttons
        AssignLevelButton("Level_1", "Level 1");
        AssignLevelButton("Level_2", "Level 2");
        AssignLevelButton("Level_3", "Level 3");
        AssignLevelButton("Level_4", "Level 4");
        AssignLevelButton("Level_5", "Level 5");
        AssignLevelButton("Level_6", "Level 6");


        // Map difficulty buttons
        AssignButton("EasyButton", () => LoadLevelWithDifficulty("Easy"));
        AssignButton("MediumButton", () => LoadLevelWithDifficulty("Medium"));
        AssignButton("HardButton", () => LoadLevelWithDifficulty("Hard"));

        // Set up level buttons
        SetupLevelButtons();
    }

    private void Start()
    {
        StartSceneFadeIn();
    }

    private void SetupLevelButtons()
    {
        int totalLevels = 6; // Set this to the number of available levels
        for (int i = 1; i <= totalLevels; i++)
        {
            string buttonName = $"Level_{i}";
            // Lock visual feedback
            var button = doc.rootVisualElement.Q<Button>(buttonName);
            if (button != null)
            {
                // Disable the button if the level is locked
                button.SetEnabled(i <= playerData.highestUnlockedLevel);
            }
        }
    }

    private void AssignButton(string buttonName, System.Action action)
    {
        var button = doc.rootVisualElement.Q<Button>(buttonName);
        if (button != null)
            button.clicked += action;
        else
            Debug.LogError($"Button '{buttonName}' not found!");
    }

    private void AssignLevelButton(string buttonName, string levelName)
    {
        AssignButton(buttonName, () => SelectLevel(levelName));
    }

    private void SelectLevel(string levelName)
    {
        selectedLevel = levelName;
        ToggleDifficultyMenu(true); // Show the Difficulty Menu after selecting level
    }

    private void LoadLevelWithDifficulty(string difficulty)
    {
        PlayerPrefs.SetString("SelectedLevel", selectedLevel);
        PlayerPrefs.SetString("Difficulty", difficulty);

        switch (difficulty) {
            case "Easy":
                enemyHealthMulti = 1;
                enemySpeedMulti = 1;
                sellMulti = 0.8f;
                break;
            case "Medium":
                enemyHealthMulti = 1.15f;
                enemySpeedMulti = 1.15f;
                sellMulti = 0.7f;
                break;
            case "Hard":
                enemyHealthMulti = 1.3f;
                enemySpeedMulti = 1.3f;
                sellMulti = 0.6f;
                break;
        }
        StartCoroutine(PerformWithDelay(() => SceneManager.LoadScene(selectedLevel), 1f));
    }

    private void ToggleDifficultyMenu(bool show)
    {
        DifficultyMenu.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private IEnumerator PerformWithDelay(System.Action action, float delay)
    {
        ApplyTransitionEffect("transition-In", delay, action);
        yield return null;
    }

    private void StartSceneFadeIn()
    {
        ApplyTransitionEffect("transition-In", 1f, null);
    }

    private void ApplyTransitionEffect(string className, float delay, System.Action onComplete)
    {
        transition.AddToClassList(className);
        StartCoroutine(TransitionCoroutine(className, delay, onComplete));
    }

    private IEnumerator TransitionCoroutine(string className, float delay, System.Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        transition.RemoveFromClassList(className);
        onComplete?.Invoke();
    }
}
