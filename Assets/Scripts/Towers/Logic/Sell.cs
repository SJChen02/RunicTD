using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sell : MonoBehaviour {

    public static void SellTower() {

        // adding the value of the tower to the gold pool
        Tower towerScript = Controller.currentTower.GetComponent<Tower>();
        Fortress.gold += towerScript.sellValue;

        // accessing the tile the tower is placed on and reactivating it
        Tile tileScript = towerScript.tilePlacedOn.GetComponent<Tile>();
        tileScript.towerPlaced = false;
        Controller.currentTile = null;

        // close menu and clear runes
        Controller.ToggleRunicTablet();

        // deleting the tower
        Destroy(Controller.currentTower);

        // close the menu
        Controller.CloseTowerMenu();
    }
}
