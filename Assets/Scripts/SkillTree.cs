using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SkillTree : MonoBehaviour
{
    public enum Buff
    {
        None,
        AttackSpdUp,
        RangeUP,
        UltimateMode,
    }
    public Buff Buffs;


    private void None()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers)
        {
            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.fireRate = 1f;
            towerScript.range = 20f;

            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging
        }
        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    private void AttackSpdUp()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers)
        {
            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.fireRate = 10f;
            towerScript.range = 15f;
            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging
        }
        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    private void RangeUP()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers)
        {
            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.fireRate = 1f;
            towerScript.range = 1000f;
            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging
        }
        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    private void UltimateMode()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        foreach (GameObject tower in towers)
        {
            Tower towerScript = tower.GetComponent<Tower>(); // Grabs the Tower script
            towerScript.fireRate = 10f;
            towerScript.range = 1000f;
            //Debug.Log("CurrentBuffActivated: " + towerScript.fireRate); //for debugging
        }
        //Debug.Log("CurrentBuffActivated: " + towers); //for debugging

    }

    // Update is called once per frame
    void Update()
    {

        switch (Buffs)
        {
            case Buff.None:
                None();
                break;

            case Buff.AttackSpdUp:
                AttackSpdUp();
                break;

            case Buff.RangeUP:
                RangeUP();
                break;

            case Buff.UltimateMode:
                UltimateMode();
                break;
        }

        //Debug.Log("CurrentBuffActivated: " + Buffs);
    }
}
