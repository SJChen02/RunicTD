using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    public GameObject prefab;
    private Vector3 offsetVector = new Vector3(0, 5, 0);

    private void OnMouseDown() {
        if (CompareTag("Tile")) {
            Destroy(gameObject);
            Instantiate(
                prefab, 
                gameObject.transform.position + offsetVector, 
                Quaternion.identity, 
                GameObject.Find("Dummy Towers").transform
            );
        }
    }

}
