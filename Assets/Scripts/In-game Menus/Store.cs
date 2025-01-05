using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour {

    public void PurchaseTower() {

        // referencing the script to access the tower cost in the if statement
        GameObject towerSelected = TowerIdentifier.GetTower();
        Tower towerScript = towerSelected.GetComponent<Tower>();

        if (Fortress.mana >= towerScript.cost) {

            // flagging the tile as deactivated
            Tile tileScript = Controller.currentTile.GetComponent<Tile>();
            tileScript.towerPlaced = true;

            // saving which tile the tower is placed on, for selling
            towerScript.tilePlacedOn = Controller.currentTile;

            // unhighlighting the tile
            Controller.currentTile.GetComponent<Renderer>().material = Controller.tileMaterial;

            // placing the tower
            Instantiate(
                towerSelected,
                Controller.currentTile.transform.position,
                Quaternion.identity,
                GameObject.Find("Towers").transform
            );

            // paying for the tower, unselecting the tile, closing the store
            Fortress.mana -= towerScript.cost;
            Controller.currentTile = null;
            Controller.store.SetActive(false);
        }
    }
}