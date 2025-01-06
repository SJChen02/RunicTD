using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTargeting : MonoBehaviour {

    // attached to targeting buttons in the tower menu
    public static void NextTargeting() {
        Tower towerScript = Controller.currentTower.GetComponent<Tower>();

        // linking the new targeting to the Tower script variable
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

        // updating the targeting text in the menu
        Controller.targetingText.text = towerScript.targeting;
    }

    // attached to targeting buttons in the tower menu
    public static void LastTargeting() {
        Tower towerScript = Controller.currentTower.GetComponent<Tower>();

        // linking the new targeting to the Tower script variable
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

        // updating the targeting text in the menu
        Controller.targetingText.text = towerScript.targeting;
    }
}
