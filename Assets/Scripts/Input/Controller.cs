using System;
using System.Data;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// this script is solely attached to the UI canvas
public class Controller : MonoBehaviour {

    // setting up for input
    private static PlayerInput playerInput;
    private static InputAction placeTowerAction;
    private static Keybindings keybindings;
    private static Camera mainCamera;
    private static GameObject objectClicked;

    // transforms for finding scene object references
    public static Transform parentUI;
    private static Transform targetingParent;
    private static Transform sellParent;
    private static Transform rankParent;
    private static Transform towerMenuParent;
    private static Transform runicTabletsParent;
    private static Transform towersParent;

    // tile and store
    private static Material selectedTileMaterial;
    public static GameObject store;
    public static GameObject currentTile;
    public static Material tileMaterial;

    // tower menu
    private bool mouseOverUI;
    private static GameObject tablet;
    private static GameObject rangeIndicator;
    public static GameObject towerMenu;
    public static GameObject currentTower;
    public static GameObject earthRunicTablet;
    public static GameObject fireRunicTablet;
    public static GameObject windRunicTablet;
    public static GameObject waterRunicTablet;

    // fortress menu
    public static GameObject fortressMenu;
    public static bool fortressOpened;

    // setting up text references
    public TextMeshProUGUI nameText;
    public static TextMeshProUGUI rankText;
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

        // setting the static texts via script as they cannot be set via inspector
        towerMenuParent = parentUI.Find("Tower Menu").transform;

        targetingParent = towerMenuParent.Find("Targeting").gameObject.transform;
        targetingText = targetingParent.Find("Targeting Text").GetComponent<TextMeshProUGUI>();

        sellParent = towerMenuParent.Find("Sell").gameObject.transform;
        sellText = sellParent.Find("Sell Text").GetComponent<TextMeshProUGUI>();

        rankParent = towerMenuParent.Find("Rank").gameObject.transform;
        rankText = rankParent.Find("Rank Text").GetComponent<TextMeshProUGUI>();

        // obtaining references for Runic Tablets
        runicTabletsParent = towerMenuParent.Find("Runic Tablets").gameObject.transform;
        earthRunicTablet = runicTabletsParent.Find("Earth Runic Tablet").gameObject;
        fireRunicTablet = runicTabletsParent.Find("Fire Runic Tablet").gameObject;
        windRunicTablet = runicTabletsParent.Find("Wind Runic Tablet").gameObject;
        waterRunicTablet = runicTabletsParent.Find("Water Runic Tablet").gameObject;

        // obtaining reference to the range indicator disk
        towersParent = GameObject.Find("Towers").transform;
        rangeIndicator = towersParent.Find("Range Indicator").gameObject;

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

    private static void ToggleRangeIndicator() {
        if (rangeIndicator.activeInHierarchy) {
            rangeIndicator.SetActive(false);
            return;
        }

        // scale, move and activate the range indicator
        Tower towerScript = currentTower.GetComponent<Tower>();
        rangeIndicator.transform.localScale = new Vector3(2 * towerScript.range, 2 * towerScript.range, 2 * towerScript.range);
        rangeIndicator.transform.position = currentTower.transform.position + new Vector3(0f, 0.01f, 0f);
        rangeIndicator.SetActive(true);
    }

    // for updating the range indicator while it's active, e.g. when purchasing a range tower rune
    public static void UpdateRangeIndicator() {
        Tower towerScript = currentTower.GetComponent<Tower>();
        rangeIndicator.transform.localScale = new Vector3(2 * towerScript.range, 2 * towerScript.range, 2 * towerScript.range);
    }

    public static void UpdateSellValue(int value) {
        Tower towerScript = currentTower.GetComponent<Tower>();
        towerScript.sellValue += (int)(LevelSelectionEvents.sellMulti * value);
        sellText.text = "Sell: " + towerScript.sellValue;
    }

    public static void CloseStore() {
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
            ToggleRunicTablet();
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

    public static void ToggleRunicTablet() {
        Tower towerScript = currentTower.GetComponent<Tower>();

        // obtain the relevant tablet
        switch (towerScript.towerName) {
            case "Earth Wizard":
                tablet = earthRunicTablet;
                break;
            case "Fire Wizard":
                tablet = fireRunicTablet;
                break;
            case "Wind Wizard":
                tablet = windRunicTablet;
                break;
            case "Water Wizard":
                tablet = waterRunicTablet;
                break;
        }

        if (tablet.activeInHierarchy) {
            // close and reset the runes
            tablet.SetActive(false);
            Runes.ClearRunes(tablet, towerScript.rank);
        }
        else {
            // fill in the runes and open
            Runes.FillRunes(tablet, towerScript.path, towerScript.rank);
            tablet.SetActive(true);
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
    }

    public static void CloseTowerMenu() {
        currentTower = null;
        towerMenu.SetActive(false);
        rangeIndicator.SetActive(false);
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
            FillTowerMenu();
            ToggleRangeIndicator();
            ToggleRunicTablet();
        }
        // a tower is selected already
        else {
            // if it's the same tower, close the tower menu
            if (currentTower == objectClicked) {
                ToggleRunicTablet();
                CloseTowerMenu();
            }
            // if it's a different tower, reload it
            else {
                // unload the currently selected tower
                ToggleRunicTablet();
                ToggleRangeIndicator();

                // move to the other tower and load it
                currentTower = objectClicked;
                ToggleRunicTablet();
                ToggleRangeIndicator();
                FillTowerMenu();
            }
        }
    }

    public static void CloseFortress() {
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
            ToggleRunicTablet();
            CloseTowerMenu();
        }

        fortressOpened = true;
        fortressMenu.SetActive(true);
    }

    private void LateUpdate() {
        // after all updates, flag if the mouse is over the UI
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