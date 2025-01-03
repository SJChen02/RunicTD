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
    private Transform runicTabletsParent;

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
    public static GameObject earthRunicTablet;
    public static GameObject fireRunicTablet;
    public static GameObject iceRunicTablet;
    public static GameObject waterRunicTablet;

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

        // obtaining references for scene menus
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

        // obtaining references for Runic Tablets
        runicTabletsParent = towerMenuParent.Find("Runic Tablets").gameObject.transform;
        earthRunicTablet = runicTabletsParent.Find("Earth Runic Tablet").gameObject;
        fireRunicTablet = runicTabletsParent.Find("Fire Runic Tablet").gameObject;
        iceRunicTablet = runicTabletsParent.Find("Ice Runic Tablet").gameObject;
        waterRunicTablet = runicTabletsParent.Find("Water Runic Tablet").gameObject;

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

    private void CloseStore() {
        currentTile.GetComponent<Renderer>().material = tileMaterial;
        currentTile = null;
        store.SetActive(false);
    }

    private void OpenStore() {
        // check if the clicked tile already has a tower on it
        Tile tileScript = objectClicked.GetComponent<Tile>();
        if (tileScript.towerPlaced) {
            tileScript = null;
            return;
        }

        // if a tower menu is currently open, close it
        if (currentTower != null) {
            RunicTablet("Close", currentTower);
            CloseTowerMenu();
        }
        // if the fortress menu is already open, close it
        else if (fortressOpened) {
            CloseFortress();
        }

        // if the store is closed, open it
        if (currentTile == null) {
            currentTile = objectClicked;
            currentTile.GetComponent<Renderer>().material = selectedTileMaterial;
            store.SetActive(true);
        }
        // if the store is open, confirm which tile was clicked
        else {
            // if the store is already open on the clicked tile, close the store
            if (currentTile == objectClicked) {
                CloseStore();
            }
            // if the store is open on a different tile, deselect it and select the clicked tile
            else {
                currentTile.GetComponent<Renderer>().material = tileMaterial;
                currentTile = objectClicked;
                currentTile.GetComponent<Renderer>().material = selectedTileMaterial;
            }
        }
    }

    private void RunicTablet(string openOrClose, GameObject tower) {
        Tower towerScript = tower.GetComponent<Tower>();

        if (openOrClose.Equals("Open")) {
            switch (towerScript.towerName) {
                case "Earth Wizard":
                    earthRunicTablet.SetActive(true);
                    break;
                case "Fire Wizard":
                    fireRunicTablet.SetActive(true);
                    break;
                case "Ice Wizard":
                    iceRunicTablet.SetActive(true);
                    break;
                case "Water Wizard":
                    waterRunicTablet.SetActive(true);
                    break;
            }
        }
        else if (openOrClose.Equals("Close")) {
            switch (towerScript.towerName) {
                case "Earth Wizard":
                    earthRunicTablet.SetActive(false);
                    break;
                case "Fire Wizard":
                    fireRunicTablet.SetActive(false);
                    break;
                case "Ice Wizard":
                    iceRunicTablet.SetActive(false);
                    break;
                case "Water Wizard":
                    waterRunicTablet.SetActive(false);
                    break;
            }
        }
    }

    // fills in the required information for the tower menu
    private void FillTowerMenu() {
        Tower towerScript = currentTower.GetComponent<Tower>();

        // fill in the tower info
        nameText.text = towerScript.towerName;
        rankText.text = "Rank: " + towerScript.rank;
        targetingText.text = towerScript.targeting;
        sellText.text = "Sell: " + towerScript.sellValue;

        // enable the tower's runic tablet
        RunicTablet("Open", currentTower);
    }

    private void CloseTowerMenu() {
        currentTower = null;
        towerMenu.SetActive(false);
    }

    private void OpenTowerMenu() {
        // if the store is already open, close it
        if (currentTile != null) {
            CloseStore();
        }
        // if the fortress menu is already open, close it
        else if (fortressOpened) {
            CloseFortress();
        }

        // if there is no selected tower already, open the menu with runic tablet and info
        if (currentTower == null) {
            currentTower = objectClicked;
            towerMenu.SetActive(true);
            RunicTablet("Open", currentTower);
            FillTowerMenu();
        }
        // a tower is selected already
        else {
            // if it's the same tower, close the tower menu
            if (currentTower == objectClicked) {
                RunicTablet("Close", currentTower);
                CloseTowerMenu();
            }
            // if it's a different tower, reload the menu with its information
            else {
                RunicTablet("Close", currentTower);
                currentTower = objectClicked;
                RunicTablet("Open", currentTower);
                FillTowerMenu();
            }
        }
    }

    private void CloseFortress() {
        fortressOpened = false;
        fortressMenu.SetActive(false);
    }

    private void OpenFortress() {
        // if the store is already open, close it
        if (currentTile != null) {
            CloseStore();
        }
        // if a tower menu is currently open, deselect the tower and close the menu
        else if (currentTower != null) {
            RunicTablet("Close", currentTower);
            CloseTowerMenu();
        }

        fortressOpened = true;
        fortressMenu.SetActive(true);
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
            OpenStore();
        }
        else if (objectClicked.CompareTag("Tower")) {
            OpenTowerMenu();
        }
        else if (objectClicked.CompareTag("Fortress")) {
            if (fortressOpened) {
                CloseFortress();
            }
            else {
                OpenFortress();
            }
        }
    }
}