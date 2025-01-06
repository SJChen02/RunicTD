using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerIdentifier : MonoBehaviour {

    // for the buttons in the store
    private static GameObject towerSelected;

    public static void TowerSelected(GameObject tower) {
        towerSelected = tower;
    }

    public static GameObject GetTower() {
        return towerSelected;
    }

}