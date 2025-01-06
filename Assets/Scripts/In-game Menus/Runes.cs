using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Runes : MonoBehaviour {

    // color values obtained from the default button disabledColor in the inspector
    private static Color dullGrey = new Color(0.78431f, 0.78431f, 0.78431f, 0.5019608f);
    private static Button button;
    private static GameObject tablet;

    // rune costs - for easy editing
    private static int waterPath1Rune1 = 80;
    private static int waterPath1Rune2 = 120;
    private static int waterPath1Rune3 = 200;
    private static int waterPath2Rune1 = 80;
    private static int waterPath2Rune2 = 120;
    private static int waterPath2Rune3 = 200;

    private static int firePath1Rune1 = 80;
    private static int firePath1Rune2 = 120;
    private static int firePath1Rune3 = 200;
    private static int firePath2Rune1 = 80;
    private static int firePath2Rune2 = 120;
    private static int firePath2Rune3 = 200;

    private static int earthPath1Rune1 = 80;
    private static int earthPath1Rune2 = 120;
    private static int earthPath1Rune3 = 200;
    private static int earthPath2Rune1 = 80;
    private static int earthPath2Rune2 = 120;
    private static int earthPath2Rune3 = 200;

    private static int windPath1Rune1 = 80;
    private static int windPath1Rune2 = 120;
    private static int windPath1Rune3 = 200;
    private static int windPath2Rune1 = 80;
    private static int windPath2Rune2 = 120;
    private static int windPath2Rune3 = 200;

    public static void MarkRuneAsPurchased(GameObject parent, string runeName) {
        // sound effect
        SoundManager.PlaySound(SoundType.TowerUpgrade);

        // get button and disable it
        button = parent.transform.Find(runeName).GetComponent<Button>();
        button.interactable = false;

        // assign its colour
        ColorBlock colorBlock = button.colors;
        colorBlock.colorMultiplier = 2.0f;
        colorBlock.disabledColor = Color.green;
        button.colors = colorBlock;
    }

    public static void ClearRunes(GameObject tablet, int rank) {
        // rank 0 means no runes are purchased so no need to reset
        if (rank == 0) {
            return;
        }

        // otherwise, wipe the tablet back to default
        for (int i = 1; i <= 3; i++) {
            // get button and ensure it is disabled
            button = tablet.transform.Find("Path 1 Rune " + i).GetComponent<Button>();
            button.interactable = false;
            // reset colour back to default
            ColorBlock colorBlock = button.colors;
            colorBlock.disabledColor = dullGrey;
            colorBlock.colorMultiplier = 1.0f;
            button.colors = colorBlock;
            // if it's the first rune of a path, ensure the button is enabled
            if (i == 1) {
                button.interactable = true;
            }
        }
        for (int i = 1; i <= 3; i++) {
            // get button and ensure it is disabled
            button = tablet.transform.Find("Path 2 Rune " + i).GetComponent<Button>();
            button.interactable = false;
            // reset colour back to default
            ColorBlock colorBlock = button.colors;
            colorBlock.disabledColor = dullGrey;
            colorBlock.colorMultiplier = 1.0f;
            button.colors = colorBlock;
            // if it's the first rune of a path, enable the button
            if (i == 1) {
                button.interactable = true;
            }
        }
    }

    public static void FillRunes(GameObject tablet, int path, int rank) {
        // rank 0 means no runes are purchased so no need to reset
        if (rank == 0) {
            return;
        }

        // make the other path inaccessible
        tablet.transform.Find("Path " + (3 - path) + " Rune 1").GetComponent<Button>().interactable = false;

        // mark runes as purchased based on the tower rank
        for (int i = 1; i <= rank; i++) {
            MarkRuneAsPurchased(tablet, "Path " + path + " Rune " + i);
        }

        // if rank was below 3, then the next rune needs to be made accessible for purchase
        if (rank < 3) {
            button = tablet.transform.Find("Path " + path + " Rune " + (1 + rank)).GetComponent<Button>();
            button.interactable = true;
        }
    }

    // returns the tablet based on the tower name inputted
    private static GameObject GetTablet(string towerName) {
        switch (towerName) {
            case "Earth Wizard":
                return Controller.earthRunicTablet;
            case "Fire Wizard":
                return Controller.fireRunicTablet;
            case "Wind Wizard":
                return Controller.windRunicTablet;
            case "Water Wizard":
                return Controller.waterRunicTablet;
        }
        return null;
    }

    private static bool PurchaseEarthRune(int rank, int path, Tower towerScript) {
        // Boulder - Path of Devotion
        if (path == 1) {
            switch (rank) {
                case 0:
                    if (Fortress.mana >= earthPath1Rune1) {
                        towerScript.range *= 0.75f;
                        towerScript.damage *= 1.25f;

                        Controller.UpdateRangeIndicator();
                        Fortress.mana -= earthPath1Rune1;
                        Controller.UpdateSellValue(earthPath1Rune1);
                        return true;
                    }
                    return false;
                case 1:
                    if (Fortress.mana >= earthPath1Rune2) {
                        towerScript.fireRate -= 0.15f;
                        towerScript.stunDuration *= 1.25f;

                        Fortress.mana -= earthPath1Rune2;
                        Controller.UpdateSellValue(earthPath1Rune2);
                        return true;
                    }
                    return false;
                case 2:
                    if (Fortress.mana >= earthPath1Rune3) {
                        towerScript.damage *= 1.5f;
                        towerScript.stunDuration *= 1.5f;

                        Fortress.mana -= earthPath1Rune3;
                        Controller.UpdateSellValue(earthPath1Rune3);
                        return true;
                    }
                    return false;
            }
        }
        // Earthen Spikes - Path of Sacrifice
        else if (path == 2) {
            switch (rank) {
                case 0:
                    if (Fortress.mana >= earthPath2Rune1) {
                        towerScript.stunDuration *= 0.5f;
                        towerScript.damage *= 1.25f;

                        Fortress.mana -= earthPath2Rune1;
                        Controller.UpdateSellValue(earthPath2Rune1);
                        return true;
                    }
                    return false;
                case 1:
                    if (Fortress.mana >= earthPath2Rune2) {
                        towerScript.range *= 1.25f;
                        towerScript.stunDuration = 0.0f;

                        Controller.UpdateRangeIndicator();
                        Fortress.mana -= earthPath2Rune2;
                        Controller.UpdateSellValue(earthPath2Rune2);
                        return true;
                    }
                    return false;
                case 2:
                    if (Fortress.mana >= earthPath2Rune3) {
                        towerScript.damage *= 1.5f;
                        towerScript.fireRate += 0.3f;

                        Fortress.mana -= earthPath2Rune3;
                        Controller.UpdateSellValue(earthPath2Rune3);
                        return true;
                    }
                    return false;
            }
        }
        return false;
    }

    private static bool PurchaseFireRune(int rank, int path, Tower towerScript) {
        // Lava - Path of Devotion
        if (path == 1) {
            switch (rank) {
                case 0:
                    if (Fortress.mana >= firePath1Rune1) {
                        towerScript.fireRate -= 0.2f;
                        towerScript.damage *= 1.25f;

                        Fortress.mana -= firePath1Rune1;
                        Controller.UpdateSellValue(firePath1Rune1);
                        return true;
                    }
                    return false;
                case 1:
                    if (Fortress.mana >= firePath1Rune2) {
                        towerScript.range *= 0.7f;
                        towerScript.burnDamage *= 1.25f;

                        Controller.UpdateRangeIndicator();
                        Fortress.mana -= firePath1Rune2;
                        Controller.UpdateSellValue(firePath1Rune2);
                        return true;
                    }
                    return false;
                case 2:
                    if (Fortress.mana >= firePath1Rune3) {
                        towerScript.burnDamage *= 1.5f;
                        towerScript.burnDuration *= 2.0f;

                        Fortress.mana -= firePath1Rune3;
                        Controller.UpdateSellValue(firePath1Rune3);
                        return true;
                    }
                    return false;
            }
        }
        // Combustion - Path of Sacrifice
        else if (path == 2) {
            switch (rank) {
                case 0:
                    if (Fortress.mana >= firePath2Rune1) {
                        towerScript.damage *= 1.25f;
                        towerScript.burnDamage *= 0.4f;

                        Fortress.mana -= firePath2Rune1;
                        Controller.UpdateSellValue(firePath2Rune1);
                        return true;
                    }
                    return false;
                case 1:
                    if (Fortress.mana >= firePath2Rune2) {
                        towerScript.burnDamage *= 0f;
                        towerScript.range *= 1.25f;

                        Controller.UpdateRangeIndicator();
                        Fortress.mana -= firePath2Rune2;
                        Controller.UpdateSellValue(firePath2Rune2);
                        return true;
                    }
                    return false;
                case 2:
                    if (Fortress.mana >= firePath2Rune3) {
                        towerScript.fireRate += 0.4f;
                        towerScript.damage *= 1.25f;

                        Fortress.mana -= firePath2Rune3;
                        Controller.UpdateSellValue(firePath2Rune3);
                        return true;
                    }
                    return false;
            }
        }
        return false;
    }

    private static bool PurchaseWindRune(int rank, int path, Tower towerScript) {
        // Air Scythe - Path of Devotion
        if (path == 1) {
            switch (rank) {
                case 0:
                    if (Fortress.mana >= windPath1Rune1) {
                        towerScript.range *= 1.25f;
                        towerScript.fireRate -= 0.2f;

                        Controller.UpdateRangeIndicator();
                        Fortress.mana -= windPath1Rune1;
                        Controller.UpdateSellValue(windPath1Rune1);
                        return true;
                    }
                    return false;
                case 1:
                    if (Fortress.mana >= windPath1Rune2) {
                        towerScript.critChance *= 2.5f;
                        towerScript.fireRate -= 0.2f;

                        Fortress.mana -= windPath1Rune2;
                        Controller.UpdateSellValue(windPath1Rune2);
                        return true;
                    }
                    return false;
                case 2:
                    if (Fortress.mana >= windPath1Rune3) {
                        towerScript.critDamage *= 1.75f;
                        towerScript.damage *= 1.5f;

                        Fortress.mana -= windPath1Rune3;
                        Controller.UpdateSellValue(windPath1Rune3);
                        return true;
                    }
                    return false;
            }
        }
        // Gale Force - Path of Sacrifice
        else if (path == 2) {
            switch (rank) {
                case 0:
                    if (Fortress.mana >= windPath2Rune1) {
                        towerScript.critChance *= 0.5f;
                        towerScript.damage *= 1.5f;

                        Fortress.mana -= windPath2Rune1;
                        Controller.UpdateSellValue(windPath2Rune1);
                        return true;
                    }
                    return false;
                case 1:
                    if (Fortress.mana >= windPath2Rune2) {
                        towerScript.critChance *= 0f;
                        towerScript.fireRate += 0.3f;

                        Fortress.mana -= windPath2Rune2;
                        Controller.UpdateSellValue(windPath2Rune2);
                        return true;
                    }
                    return false;
                case 2:
                    if (Fortress.mana >= windPath2Rune3) {
                        towerScript.fireRate += 0.45f;
                        towerScript.range *= 1.5f;

                        Controller.UpdateRangeIndicator();
                        Fortress.mana -= windPath2Rune3;
                        Controller.UpdateSellValue(windPath2Rune3);
                        return true;
                    }
                    return false;
            }
        }
        return false;
    }

    private static bool PurchaseWaterRune(int rank, int path, Tower towerScript) {
        // Tsunami - Path of Devotion
        if (path == 1) {
            switch (rank) {
                case 0:
                    if (Fortress.mana >= waterPath1Rune1) {
                        towerScript.splashRadius *= 1.25f;
                        towerScript.fireRate -= 0.25f;

                        Fortress.mana -= waterPath1Rune1;
                        Controller.UpdateSellValue(waterPath1Rune1);
                        return true;
                    }
                    return false;
                case 1:
                    if (Fortress.mana >= waterPath1Rune2) {
                        towerScript.damage *= 1.25f;
                        towerScript.range *= 0.75f;

                        Controller.UpdateRangeIndicator();
                        Fortress.mana -= waterPath1Rune2;
                        Controller.UpdateSellValue(waterPath1Rune2);
                        return true;
                    }
                    return false;

                case 2:
                    if (Fortress.mana >= waterPath1Rune3) {
                        towerScript.splashRadius *= 1.35f;
                        towerScript.damage *= 1.5f;

                        Fortress.mana -= waterPath1Rune3;
                        Controller.UpdateSellValue(waterPath1Rune3);
                        return true;
                    }
                    return false;
            }
        }
        // Marine Orb - Path of Sacrifice
        else if (path == 2) {
            switch (rank) {
                case 0:
                    if (Fortress.mana >= waterPath2Rune1) {
                        towerScript.splashRadius *= 0.7f;
                        towerScript.range *= 1.25f;

                        Controller.UpdateRangeIndicator();
                        Fortress.mana -= waterPath2Rune1;
                        Controller.UpdateSellValue(waterPath2Rune1);
                        return true;
                    }
                    return false;
                case 1:
                    if (Fortress.mana >= waterPath2Rune2) {
                        towerScript.splashRadius *= 0f;
                        towerScript.damage *= 1.3f;

                        Fortress.mana -= waterPath2Rune2;
                        Controller.UpdateSellValue(waterPath2Rune2);
                        return true;
                    }
                    return false;
                case 2:
                    if (Fortress.mana >= waterPath2Rune3) {
                        towerScript.range *= 1.35f;
                        towerScript.damage *= 1.5f;

                        Controller.UpdateRangeIndicator();
                        Fortress.mana -= waterPath2Rune3;
                        Controller.UpdateSellValue(waterPath2Rune3);
                        return true;
                    }
                    return false;
            }
        }
        return false;
    }

    // returns true if the purchase of a specified rune was successful
    private static bool PurchaseRune(string towerName, int rank, int path, Tower towerScript) {
        switch (towerName) {
            case "Earth Wizard":
                return PurchaseEarthRune(rank, path, towerScript);
            case "Fire Wizard":
                return PurchaseFireRune(rank, path, towerScript);
            case "Wind Wizard":
                return PurchaseWindRune(rank, path, towerScript);
            case "Water Wizard":
                return PurchaseWaterRune(rank, path, towerScript);
        }
        return false;
    }

    public static void SetPathTo1() {
        Tower towerScript = Controller.currentTower.GetComponent<Tower>();
        towerScript.path = 1;
    }

    public static void SetPathTo2() {
        Tower towerScript = Controller.currentTower.GetComponent<Tower>();
        towerScript.path = 2;
    }

    public static void ProgressPath() {
        Tower towerScript = Controller.currentTower.GetComponent<Tower>();

        // check if the rune is successfully purchased
        if (PurchaseRune(towerScript.towerName, towerScript.rank, towerScript.path, towerScript)) {

            // obtain the relevant tablet
            tablet = GetTablet(towerScript.towerName);

            // update the tower rank
            towerScript.rank += 1;
            Controller.rankText.text = "Rank: " + towerScript.rank;

            // mark the rune as purchased
            MarkRuneAsPurchased(tablet, "Path " + towerScript.path + " Rune " + towerScript.rank);

            // lock off the other path
            if (towerScript.rank == 1) {
                GetTablet(towerScript.towerName).transform.Find("Path " + (3 - towerScript.path) + " Rune 1").GetComponent<Button>().interactable = false;
            }

            // enable the next button
            if (towerScript.rank != 3) {
                tablet.transform.Find("Path " + towerScript.path + " Rune " + (towerScript.rank + 1)).GetComponent<Button>().interactable = true;
            }
        }
        else {
            // if the first ruin failed to be purchased, reset the path to 0
            if (towerScript.rank == 0) {
                // restore the path to the default state
                towerScript.path = 0;
            }
        }
    }
}