using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

<<<<<<< HEAD
<<<<<<< Updated upstream
<<<<<<< HEAD
    public GameObject towerPrefab;
=======
    public GameObject prefab;
>>>>>>> dev
=======
    public GameObject towerPrefab;
>>>>>>> Stashed changes
=======
    public GameObject prefab;
>>>>>>> dev
    private Vector3 offsetVector = new Vector3(0, 5, 0);

    private void OnMouseDown() {
        if (CompareTag("Tile")) {
<<<<<<< HEAD
<<<<<<< Updated upstream
<<<<<<< HEAD
            placeTower(towerPrefab);
        }
    }

    private void placeTower(GameObject towerType) {
        if (FortressGold.gold >= 10) {
            Destroy(gameObject);
            Instantiate(
                towerPrefab,
                gameObject.transform.position + offsetVector,
                Quaternion.identity,
                GameObject.Find("Towers").transform
            );
            FortressGold.gold -= 10;
        }

=======
=======
>>>>>>> dev
            Destroy(gameObject);
            Instantiate(
                prefab, 
                gameObject.transform.position + offsetVector, 
                Quaternion.identity, 
                GameObject.Find("Dummy Towers").transform
            );
<<<<<<< HEAD
=======
            placeTower(towerPrefab);
>>>>>>> Stashed changes
        }
>>>>>>> dev
    }

    private void placeTower(GameObject towerType) {
        if (FortressGold.gold >= 10) {
            Destroy(gameObject);
            Instantiate(
                towerPrefab,
                gameObject.transform.position + offsetVector,
                Quaternion.identity,
                GameObject.Find("Towers").transform
            );
            FortressGold.gold -= 10;
        }

=======
        }
>>>>>>> dev
    }

}
