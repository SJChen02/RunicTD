using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTargeting : MonoBehaviour {

    public static void NextTargeting() {
        Tower towerScript = Controller.currentTower.GetComponent<Tower>();

        switch (towerScript.targeting) {
            case "First":
                towerScript.targeting = "Close";
                break;

            case "Close":
                towerScript.targeting = "Last";
                break;

            case "Last":
                towerScript.targeting = "First";
                break;
        }

        Controller.targetingText.text = towerScript.targeting;
    }

    public static void LastTargeting() {
        Tower towerScript = Controller.currentTower.GetComponent<Tower>();

        switch (towerScript.targeting) {
            case "First":
                towerScript.targeting = "Last";
                break;

            case "Last":
                towerScript.targeting = "Close";
                break;

            case "Close":
                towerScript.targeting = "First";
                break;
        }

        Controller.targetingText.text = towerScript.targeting;
    }
}
