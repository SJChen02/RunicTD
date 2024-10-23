using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class SkillTree : MonoBehaviour {

<<<<<<< Updated upstream
=======

    List<UnityAction> ListOfActivatedBuff = new List<UnityAction>();
    private bool BroughtFireRateBuff = false;
    private bool BroughtRangeUpBuff = false;

>>>>>>> Stashed changes
    public enum Buff {

        None,
        FireRateUp,
        RangeUp,
        UltimateMode,

    }

    public Buff Buffs;

    private void None() {

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers) {

            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.fireRate = 1.25f;
            towerScript.range = 20f;

            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging

        }

        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    private void FireRateUp() {

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers) {

            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.fireRate = 1.375f; 
            
            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging

        }
        
<<<<<<< Updated upstream
        GameObject.Find("Fire Rate Button").SetActive(false); // deactivate button
=======
        if (BroughtFireRateBuff == false)
        {
            GameObject.Find("Fire Rate Button").SetActive(false); // deactivate button
            BroughtFireRateBuff = true;
        }
        
>>>>>>> Stashed changes
        
        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    private void RangeUp() {

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers) {

            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.range = 25; 
            
            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging

        }

<<<<<<< Updated upstream
        GameObject.Find("Range Button").SetActive(false); // deactivate button
=======
        if (BroughtRangeUpBuff == false)
        {
            GameObject.Find("Range Button").SetActive(false); // deactivate button
            BroughtRangeUpBuff = true;
        }
>>>>>>> Stashed changes

        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    private void UltimateMode() {

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers) {

            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.fireRate = 10f;
            towerScript.range = 1000f;
            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging

        }

        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    // Update is called once per frame
    void Update() {

        switch (Buffs) {

            case Buff.None:
                None();
                break;

            case Buff.FireRateUp:
                FireRateUp();
                ListOfActivatedBuff.Add(FireRateUp);
                break;

            case Buff.RangeUp:
                RangeUp();
                ListOfActivatedBuff.Add(RangeUp);
                break;

            case Buff.UltimateMode:
                UltimateMode();
                break;
        }

        foreach (UnityAction action in ListOfActivatedBuff)
        {
            action.Invoke();
        }

        //Debug.Log("CurrentBuffActivated: " + Buffs);
    }

}
