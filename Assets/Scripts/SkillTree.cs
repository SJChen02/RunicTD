using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SkillTree : MonoBehaviour {

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
            towerScript.fireRate = (float)1.375; 
            
            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging

        }
        
        GameObject.Find("Fire Rate Button").SetActive(false); // deactivate button
        
        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    private void RangeUp() {

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers) {

            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.range = 25; 
            
            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging

        }

        GameObject.Find("Range Button").SetActive(false); // deactivate button

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
                break;

            case Buff.RangeUp:
                RangeUp();
                break;

            case Buff.UltimateMode:
                UltimateMode();
                break;
        }

        //Debug.Log("CurrentBuffActivated: " + Buffs);
    }

}
