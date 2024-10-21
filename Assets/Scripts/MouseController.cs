using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    public GameObject towerPrefab;
    private Vector3 offsetVector = new Vector3(0, 5, 0);

    private void OnMouseDown() {
        if (CompareTag("Tile")) {
            placeTower(towerPrefab);
        }
    }

    private void placeTower(GameObject towerType) {
        if (Fortress.gold >= 10) {
            Destroy(gameObject);
            Instantiate(
                towerPrefab,
                gameObject.transform.position + offsetVector,
                Quaternion.identity,
                GameObject.Find("Towers").transform
            );
            Fortress.gold -= 10;
        }
    }

}
