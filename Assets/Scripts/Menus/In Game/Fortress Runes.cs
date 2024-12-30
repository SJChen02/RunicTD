using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class FortressRunes : MonoBehaviour {

    List<UnityAction> ListOfActivatedBuff = new List<UnityAction>();
    private bool FireRateBuff = false;
    private bool RangeUpBuff = false;

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
            towerScript.fireRate = towerScript.DefaultfireRate;
            towerScript.range = towerScript.Defaultrange;

            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging

        }

        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    private void FireRateUp() {

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers) {

            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.fireRate = towerScript.DefaultfireRate * 1.1;

            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging

        }

        if (FireRateBuff == false) {
            GameObject.Find("Fire Rate Button").SetActive(false); // deactivate button
            FireRateBuff = true;
            ListOfActivatedBuff.Add(FireRateUp);
        }


        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    private void RangeUp() {

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers) {

            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.range = towerScript.Defaultrange * 1.25;

            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging

        }

        if (RangeUpBuff == false) {
            GameObject.Find("Range Button").SetActive(false); // deactivate button
            RangeUpBuff = true;
            ListOfActivatedBuff.Add(RangeUp);
        }

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

        /*
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

        
        foreach (UnityAction action in ListOfActivatedBuff)
        {
            action.Invoke();
        }
        */
        //Debug.Log("CurrentBuffActivated: " + Buffs);
    }

}