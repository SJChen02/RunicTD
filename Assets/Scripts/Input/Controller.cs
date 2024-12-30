using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

// this script is solely attached to Input Manager
public class Controller : MonoBehaviour {

    // setting up for input
    private PlayerInput playerInput;
    private InputAction placeTowerAction;
    private Keybindings keybindings;
    private Camera mainCamera;
    private GameObject objectClicked;

    // transforms for finding scene object references
    private Transform parentUI;
    private Transform targetingParent;
    private Transform sellParent;
    private Transform towerMenuParent;

    // tile and store
    public static GameObject store;
    public static GameObject currentTile;
    private Material selectedTileMaterial;
    public static Material tileMaterial;
    private bool eventConsumed;

    // tower menu
    public static GameObject towerMenu;
    public static GameObject currentTower;
    private bool mouseOverUI;

    // fortress menu
    public static GameObject fortressMenu;
    public static bool fortressOpened;

    // these can be static, but don't need to be because the script is attached to one object
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI rankText;

    // these need to be static to be accessed from other scripts when the values are updated
    public static TextMeshProUGUI targetingText;
    public static TextMeshProUGUI sellText;

    //---------------------------------------------------------//

    private void Awake() {
        // setting values
        fortressOpened = false;

        // tile materials
        selectedTileMaterial = Resources.Load<Material>("Materials/Level/Selected Tile");
        tileMaterial = Resources.Load<Material>("Materials/Level/Tile");

        // obtaining references for scene objects
        parentUI = GameObject.Find("UI").transform;
        store = parentUI.Find("Store").gameObject;
        towerMenu = parentUI.Find("Tower Menu").gameObject;
        fortressMenu = parentUI.Find("Fortress Menu").gameObject;

        // targetingText and sellText are static, so they must be set via script, not via inspector
        towerMenuParent = parentUI.Find("Tower Menu").transform;
        targetingParent = towerMenuParent.Find("Targeting").gameObject.transform;
        targetingText = targetingParent.Find("Targeting Text").GetComponent<TextMeshProUGUI>();
        sellParent = towerMenuParent.Find("Sell").gameObject.transform;
        sellText = sellParent.Find("Sell Text").GetComponent<TextMeshProUGUI>();

        keybindings = new Keybindings();
        mainCamera = Camera.main;
    }

    // enabling keybindings and subscribing to the event
    private void OnEnable() {
        keybindings.Enable();
        keybindings.Mouse.LeftClick.canceled += LeftClick;
    }

    // unsubscribing from the event first to prevent memory leaks, then disabling keybindings
    private void OnDisable() {
        keybindings.Mouse.LeftClick.canceled -= LeftClick;
        keybindings.Disable();
    }

    // casts a ray to detect and return the object under the mouse
    private GameObject DetectObject() {
        Ray ray = mainCamera.ScreenPointToRay(keybindings.Mouse.Point.ReadValue<Vector2>());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider != null) {
                return hit.collider.gameObject;
            }
        }

        return null;
    }

    //---------------------------------------------------------//

    private void SelectTile() {
        // if a tower menu is currently open, deselect the tower and close the menu
        if (currentTower != null) {
            currentTower = null;
            towerMenu.SetActive(false);
        }
        // if the fortress menu is already open, close it
        if (fortressOpened) {
            fortressOpened = false;
            fortressMenu.SetActive(false);
        }

        // if there is no selected tile already, select clicked tile
        if (currentTile == null) {
            currentTile = objectClicked;
            // exit if a tower is placed on the tile
            Tile tileScript = currentTile.GetComponent<Tile>();
            if (tileScript.towerPlaced) {
                currentTile = null;
                tileScript = null;
                return;
            }
            // otherwise, highlight tile and open store
            currentTile.GetComponent<Renderer>().material = selectedTileMaterial;
            store.SetActive(true);
        }
        // a tile is selected already
        else {
            // if it is the tile clicked, unhighlight it, deselect it, and close the store
            if (currentTile == objectClicked) {
                currentTile.GetComponent<Renderer>().material = tileMaterial;
                currentTile = null;
                store.SetActive(false);
            }
            // a different tile was clicked
            else {
                // exit if a tower is placed on the clicked tile, without deselecting the previous tile
                Tile tileScript = objectClicked.GetComponent<Tile>();
                if (tileScript.towerPlaced) {
                    tileScript = null;
                    return;
                }
                // otherwise, unhighlight previous tile, select the new tile and highlight it
                currentTile.GetComponent<Renderer>().material = tileMaterial;
                currentTile = objectClicked;
                currentTile.GetComponent<Renderer>().material = selectedTileMaterial;
            }
        }
    }

    // fills in the required information for the tower menu
    private void SetTowerInfo() {
        Tower towerScript = currentTower.GetComponent<Tower>();

        string nameValue = towerScript.towerName;
        int rankValue = towerScript.rank;
        string targeting = towerScript.targeting;
        int sellValue = towerScript.sellValue;

        nameText.text = nameValue;
        rankText.text = "Rank: " + rankValue;
        targetingText.text = targeting;
        sellText.text = "Sell: " + sellValue;
    }


    private void SelectTower() {
        // if a tile is already selected, deselect it
        if (currentTile != null) {
            currentTile.GetComponent<Renderer>().material = tileMaterial;
            currentTile = null;
            store.SetActive(false);
        }
        // if the fortress menu is already open, close it
        if (fortressOpened) {
            fortressOpened = false;
            fortressMenu.SetActive(false);
        }

        // if there is no selected tower already, select this tower and open the tower menu
        if (currentTower == null) {
            currentTower = objectClicked;
            towerMenu.SetActive(true);
            SetTowerInfo();
        }
        // a tower is selected already
        else {
            // if the same selected tower was clicked again, deselect it, reset the selected tower and close the tower menu
            if (currentTower == objectClicked) {
                currentTower = null;
                towerMenu.SetActive(false);
            }
            // a different tower was clicked
            else {
                // deselect current tower; assign new current tower; select it
                currentTower = objectClicked;
                SetTowerInfo();
            }
        }
    }

    private void SelectFortress() {
        // close the fortress menu if it is already open
        if (fortressOpened) {
            fortressOpened = false;
            fortressMenu.SetActive(false);
        }
        // open the fortress menu
        else {
            // if a tile is already selected, deselect it
            if (currentTile != null) {
                currentTile.GetComponent<Renderer>().material = tileMaterial;
                currentTile = null;
                store.SetActive(false);
            }
            // if a tower menu is currently open, deselect the tower and close the menu
            if (currentTower != null) {
                currentTower = null;
                towerMenu.SetActive(false);
            }
            fortressOpened = true;
            fortressMenu.SetActive(true);
        }
    }

    private void LateUpdate() { // after all updates, flag if the mouse is over the UI
        mouseOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    //---------------------------------------------------------//

    public void LeftClick(InputAction.CallbackContext context) {
        // skip execution if the mouse is over the UI
        if (mouseOverUI) {
            return;
        }

        // get clicked object and store it
        objectClicked = DetectObject();

        // if no object clicked, exit the function
        if (objectClicked == null) {
            return;
        }

        // check what was clicked and select accordingly
        if (objectClicked.CompareTag("Tile")) {
            SelectTile();
        }
        else if (objectClicked.CompareTag("Tower")) {
            SelectTower();
        }
        else if (objectClicked.CompareTag("Fortress")) {
            SelectFortress();
        }
    }
}