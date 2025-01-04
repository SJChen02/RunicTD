using System.Collections;
using TMPro;
using UnityEngine;

public class Fortress : MonoBehaviour
{
    [Header("Health")]
    public static int health = 100;
    public static int gold = 100;
    public TextMeshProUGUI goldAmount;
    public TextMeshProUGUI healthAmount;
    public TextMeshProUGUI waveCount;

    [Header("Upgrade UI")]
    public GameObject FortressSkillTree; // Reference to the upgrade UI panel

    private FortressRunes fortressRunes; // Reference to the SkillTree component

    [Header("Mana Generation")]
    public static float manaGenerationInterval = 10f; // Interval for gold generation in seconds
    private float manaGenerationTimer = 0f;

    private void Start()
    {
        // Use the new method to find the SkillTree component in the scene
        fortressRunes = Object.FindFirstObjectByType<FortressRunes>(); // Updated from FindObjectOfType

        if (fortressRunes == null)
        {
            Debug.LogError("SkillTree component not found in the scene!");
        }
    }

    private void Update()
    {
        SetText();
    }

    private void SetText()
    {
        goldAmount.text = "Gold: " + gold;
        healthAmount.text = "Health: " + health;
        waveCount.text = "Wave: " + (WaveTracker.currentWave + 1);
    }

    public static void DestroyFortress(GameObject fortress)
    {
        Destroy(fortress);
    }

    public static void TakeHit()
    {
        // Considers the enemy gone, take away health from the fortress
        WaveTracker.totalEnemiesLeft -= 1;
        health -= 20;

        // Ensures that health doesn't display as negative
        if (health < 0)
        {
            health = 0;
        }

        if (health <= 0)
        {
            // 'Destroys' the fortress by making it invisible
            // The component is still active, so gold and health are still able to be updated
            GameObject.Find("Fortress").GetComponent<Renderer>().enabled = false;
        }
    }
}


    /* Legacy Fortress Runes
    private void OnMouseDown()
    {
        Debug.Log("Fortress clicked!"); // Check if the fortress is being clicked

        if (FortressSkillTree != null)
        {
            FortressSkillTree.SetActive(true); // Enable the UI panel
        }
    }


    public void CloseSkillTreePanel()
    {
        if (FortressSkillTree != null)
        {
            FortressSkillTree.SetActive(false); // This hides the panel
            Debug.Log("Upgrade panel closed!"); // For debugging
        }
    }


    public void BuyFireRateUpgrade()
    {
        int upgradeCost = 200;

        if (gold >= upgradeCost)
        {
            gold -= upgradeCost;
            Debug.Log("Attack speed upgraded!");

            // Apply the upgrade logic if skillTree exists
            if (fortressRunes != null)
            {
                fortressRunes.Buffs = FortressRunes.Buff.FireRateUp;
            }
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    public void BuyRangeUpgrade()
    {
        int upgradeCost = 100;

        if (gold >= upgradeCost)
        {
            gold -= upgradeCost;
            Debug.Log("Range upgraded!");

            // Apply the upgrade logic if skillTree exists
            if (fortressRunes != null)
            {
                fortressRunes.Buffs = FortressRunes.Buff.RangeUp;
            }
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    public void BuyUltimateUpgrade()
    {
        int upgradeCost = 100;

        if (gold >= upgradeCost)
        {
            gold -= upgradeCost;
            Debug.Log("Ultimate upgrade activated!");

            // Apply the upgrade logic if skillTree exists
            if (fortressRunes != null)
            {
                fortressRunes.Buffs = FortressRunes.Buff.UltimateMode;
            }
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }
    */


    //for level unlocks
    //void UnlockNewLevel()
    //{
    //    if(SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
    //    {
    //        PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
    //        PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
    //        PlayerPrefs.Save();
    //    }
    //}
//}

