using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fortress : MonoBehaviour {

    [Header("Health")]

    public static int health = 100;
    public static int gold = 100;
    public TextMeshProUGUI goldAmount;
    public TextMeshProUGUI healthAmount;

    [Header("Upgrade UI")]
    public GameObject FortressSkillTree; // Reference to the upgrade UI panel

    private SkillTree skillTree; // Reference to the SkillTree component

    private void Start()
    {
        // Use the new method to find the SkillTree component in the scene
        skillTree = Object.FindFirstObjectByType<SkillTree>(); // Updated from FindObjectOfType

        if (skillTree == null)
        {
            Debug.LogError("SkillTree component not found in the scene!");
        }
    }


    void Update() {

        SetText();

    }

    private void SetText() {

        goldAmount.text = "Gold: " + gold;
        healthAmount.text = "Health: " + health;

    }

    // cannot call Destroy() directly in TakeHit(), so this is a workaround
    public static void DestroyFortress(GameObject fortress) {

        Destroy(fortress);

    }

    public static void TakeHit() {
        
        // considers the enemy gone, take away health from the fortress
        WaveTracker.totalEnemiesLeft -= 1;
        health -= 60;

        // ensures that health doesn't display as negative
        if (health < 0) {

            health = 0;

        }

        if (health <= 0) {

            // notify the GameManager that the game is over
            if (GameManager.instance != null) {

                GameManager.instance.GameOver();

            }

            // 'destroys' the fortress by making it invisible
            // the component is still active, so gold and health are still able to be updated
            GameObject.Find("Fortress").GetComponent<Renderer>().enabled = false; 
            // at present, the waves still continue after the fortress dies
        }

    }

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


    public void BuyAttackSpeedUpgrade()
    {
        int upgradeCost = 200;

        if (gold >= upgradeCost)
        {
            gold -= upgradeCost;
            Debug.Log("Attack speed upgraded!");

            // Apply the upgrade logic if skillTree exists
            if (skillTree != null)
            {
                skillTree.Buffs = SkillTree.Buff.AttackSpdUp;
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
            if (skillTree != null)
            {
                skillTree.Buffs = SkillTree.Buff.RangeUP;
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
            if (skillTree != null)
            {
                skillTree.Buffs = SkillTree.Buff.UltimateMode;
            }
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }
}

