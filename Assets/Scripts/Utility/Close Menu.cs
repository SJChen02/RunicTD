using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMenu : MonoBehaviour {

    public static void Close() {
        if (Controller.currentTile != null) {
            Controller.currentTile.GetComponent<Renderer>().material = Controller.tileMaterial;
            Controller.currentTile = null;
            Controller.store.SetActive(false);
        }
        else if (Controller.currentTower != null) {
            Controller.currentTower = null;
            Controller.towerMenu.SetActive(false);
        }
        else if (Controller.fortressOpened) {
            Controller.fortressOpened = false;
            Controller.fortressMenu.SetActive(false);
        }
    }
}
