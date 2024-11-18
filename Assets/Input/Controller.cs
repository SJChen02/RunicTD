using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class Controller : MonoBehaviour {

    private PlayerInput playerInput;
    private InputAction placeTowerAction;
    private Keybindings keybindings;
    private Camera mainCamera;

    [Header("Tower Placing")]
    public GameObject towerPrefab;
    private Vector3 offsetVector;
    private GameObject objectClicked;

    private void Awake() {
        keybindings = new Keybindings();
        mainCamera = Camera.main;
        offsetVector = new Vector3(0, 5, 0);
    }

    private void OnEnable() {
        keybindings.Enable();
        keybindings.Mouse.LeftClick.performed += LeftClick;
    }

    private void OnDisable() {
        // unsubscribing from event first to prevent memory leaks
        keybindings.Mouse.LeftClick.performed -= LeftClick;
        keybindings.Disable();
    }

    private GameObject DetectObject() { // returns the GameObject that is left clicked

        Ray ray = mainCamera.ScreenPointToRay(keybindings.Mouse.Position.ReadValue<Vector2>());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider != null) {
                return hit.collider.gameObject;
            }
        }

        return null;
    }

    public void LeftClick(InputAction.CallbackContext context) {
        
        // get clicked object and store it
        objectClicked = DetectObject();

        // if no object clicked, exit the function
        if (objectClicked == null) {
            return;
        }

        if (objectClicked.CompareTag("Tile")) {

            if (Fortress.gold >= 50) {

                Destroy(objectClicked);
                Debug.Log("Tile Destroyed");

                Instantiate(
                    towerPrefab,
                    objectClicked.transform.position + offsetVector,
                    Quaternion.identity,
                    GameObject.Find("Towers").transform
                );

                Fortress.gold -= 50;
            }

        }

        /*
        else if (objectClicked.CompareTag("Fortress")) {
            // where opening the Skill Tree by clicking the Fortress should go
        }
        */
    }
    
}

