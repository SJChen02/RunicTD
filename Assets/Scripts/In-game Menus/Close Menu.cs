using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMenu : MonoBehaviour {

    // check what is open currently and close it
    public static void Close() {
        if (Controller.currentTile != null) {
            Controller.CloseStore();
        }
        else if (Controller.currentTower != null) {
            Controller.ToggleRunicTablet();
            Controller.CloseTowerMenu();
        }
        else if (Controller.fortressOpened) {
            Controller.CloseFortress();
        }
    }
}
