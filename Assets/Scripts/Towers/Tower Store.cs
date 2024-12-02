using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerStore : MonoBehaviour {

    private Tower towerScript;
    private GameObject towerSelected;

    private Vector3 offsetVector = new Vector3(0, 5, 0);

    public void PurchaseTower() {

        towerSelected = TowerIdentifier.GetTower();
        towerScript = towerSelected.GetComponent<Tower>();

        if (Fortress.gold >= towerScript.cost) {

            Destroy(Controller.currentTile);

            Instantiate(
                towerSelected,
                Controller.currentTile.transform.position + offsetVector,
                Quaternion.identity,
                GameObject.Find("Towers").transform
            );

            Fortress.gold -= towerScript.cost;
            Controller.currentTile = null;
            Controller.towerSelection.SetActive(false);

        }

    }

}

