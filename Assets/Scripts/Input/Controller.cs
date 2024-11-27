using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

// this script is solely attached to Input Manager
public class Controller : MonoBehaviour {

    private PlayerInput playerInput;
    private InputAction placeTowerAction;
    private Keybindings keybindings;
    private Camera mainCamera;
    private Transform parent;

    public static GameObject towerSelection;
    private GameObject objectClicked;
    public static GameObject currentTile;
    private Material selectedTileMaterial;
    private Material tileMaterial;

    private void Awake() {
        selectedTileMaterial = Resources.Load<Material>("Materials/Level/Selected Tile");
        tileMaterial = Resources.Load<Material>("Materials/Level/Tile");
        parent = GameObject.Find("UI").transform;
        towerSelection = parent.Find("Tower Store").gameObject;

        keybindings = new Keybindings();
        mainCamera = Camera.main;
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

    private GameObject DetectObject() { // returns the GameObject under the mouse

        Ray ray = mainCamera.ScreenPointToRay(keybindings.Mouse.Position.ReadValue<Vector2>());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider != null) {
                return hit.collider.gameObject;
            }
        }

        return null;
    }

    private void SelectTile() {

        // if there is no selected tile already, select this tile and open the menu
        if (currentTile == null) {
            currentTile = objectClicked;
            currentTile.GetComponent<Renderer>().material = selectedTileMaterial;
            towerSelection.SetActive(true);
        }
        // a tile is selected already
        else {
            // if the same selected tile was clicked again, deselect it, reset the selected tile and close the menu
            if (currentTile == objectClicked) {
                currentTile.GetComponent<Renderer>().material = tileMaterial;
                currentTile = null;
                towerSelection.SetActive(false);
            }
            // a different tile was clicked
            else {
                // deselect current tile; assign new current tile; select it
                currentTile.GetComponent<Renderer>().material = tileMaterial;
                currentTile = objectClicked;
                currentTile.GetComponent<Renderer>().material = selectedTileMaterial;
            }
        }

    }

    public void LeftClick(InputAction.CallbackContext context) {

        // get clicked object and store it
        objectClicked = DetectObject();

        // if no object clicked, exit the function
        if (objectClicked == null) {
            return;
        }

        // if a tile is clicked, select it
        if (objectClicked.CompareTag("Tile")) {
            SelectTile();
        }

        /*
        else if (objectClicked.CompareTag("Fortress")) {
            // where opening the Skill Tree by clicking the Fortress should go
        }
        */


    }

}



