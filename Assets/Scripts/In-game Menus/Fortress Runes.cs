using UnityEngine;
using UnityEngine.UI;

public class FortressRunes : MonoBehaviour {

    // global multipliers for runes
    public static float earthDmgMulti = 1;
    public static float stunDurationMulti = 1;
    public static float fireDmgMulti = 1;
    public static float burnDurationMulti = 1;
    public static float burnDmgMulti = 1;
    public static float windDmgMulti = 1;
    public static float critChanceMulti = 1;
    public static float critDmgMulti = 1;
    public static float waterDmgMulti = 1;
    public static float splashRadiusMulti = 1;

    // fire rate and range are handled differently
    // these variables keep track so that newly placed towers reflect fortress runes bought
    public static float earthFireRateIncrease = 0;
    public static float fireFireRateIncrease = 0;
    public static float windFireRateIncrease = 0;
    public static float waterFireRateIncrease = 0;

    public static float earthRangeIncrease = 0;
    public static float fireRangeIncrease = 0;
    public static float windRangeIncrease = 0;
    public static float waterRangeIncrease = 0;

    // setting up references for the paths to access the runes
    private static Transform fortressMenuParent;
    private static GameObject earthPath;
    private static GameObject firePath;
    private static GameObject windPath;
    private static GameObject waterPath;
    private static GameObject manaGainPath;
    private static GameObject manaRatePath;

    private static void IncreaseGlobalFireRate(float fireRate, string towerName) {
        // get all towers
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        // loop through them
        foreach (GameObject tower in towers) {

            // check if it's the requested tower type and add the requested fire rate
            Tower towerScript = tower.GetComponent<Tower>();
            if (towerScript.name.Equals(towerName)) {
                towerScript.fireRate += fireRate;
            }
        }

        // set the value for all future towers
        switch (towerName) {
            case "Earth Wizard":
                earthFireRateIncrease += fireRate;
                break;
            case "Fire Wizard":
                fireFireRateIncrease += fireRate;
                break;
            case "Wind Wizard":
                windFireRateIncrease += fireRate;
                break;
            case "Water Wizard":
                waterFireRateIncrease += fireRate;
                break;
        }
    }

    private static void IncreaseGlobalRange(float range, string towerName) {
        // get all towers
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        // loop through all present towers
        foreach (GameObject tower in towers) {

            // check if it's the requested tower type and add the requested range
            Tower towerScript = tower.GetComponent<Tower>();
            if (towerScript.towerName.Equals(towerName)) {
                towerScript.range += range;
            }
        }

        // set the value for all future towers
        switch (towerName) {
            case "Earth Wizard":
                earthRangeIncrease += range;
                break;
            case "Fire Wizard":
                fireRangeIncrease += range;
                break;
            case "Wind Wizard":
                windRangeIncrease += range;
                break;
            case "Water Wizard":
                waterRangeIncrease += range;
                break;
        }
    }

    private void Start() {
        fortressMenuParent = Controller.parentUI.Find("Fortress Menu").transform;
        earthPath = fortressMenuParent.Find("Earth Path").gameObject;
        firePath = fortressMenuParent.Find("Fire Path").gameObject;
        windPath = fortressMenuParent.Find("Wind Path").gameObject;
        waterPath = fortressMenuParent.Find("Water Path").gameObject;
        manaGainPath = fortressMenuParent.Find("Mana Gain Path").gameObject;
        manaRatePath = fortressMenuParent.Find("Mana Rate Path").gameObject;
    }

    //-----// earth runes //-----//

    public static void EarthRune1() {
        if (Fortress.mana >= 100) {
            Fortress.mana -= 100;
            IncreaseGlobalRange(2.5f, "Earth Wizard");
            Runes.MarkRuneAsPurchased(earthPath, "Rune 1");

            earthPath.transform.Find("Rune 2").GetComponent<Button>().interactable = true;
        }
    }
    public static void EarthRune2() {
        if (Fortress.mana >= 200) {
            Fortress.mana -= 200;
            IncreaseGlobalFireRate(0.1f, "Earth Wizard");
            Runes.MarkRuneAsPurchased(earthPath, "Rune 2");

            earthPath.transform.Find("Rune 3a").GetComponent<Button>().interactable = true;
            earthPath.transform.Find("Rune 3b").GetComponent<Button>().interactable = true;
        }
    }
    public static void EarthRune3a() {
        if (Fortress.mana >= 300) {
            Fortress.mana -= 300;
            stunDurationMulti += 0.15f;
            Runes.MarkRuneAsPurchased(earthPath, "Rune 3a");

            earthPath.transform.Find("Rune 3b").GetComponent<Button>().interactable = false;
        }
    }
    public static void EarthRune3b() {
        if (Fortress.mana >= 300) {
            Fortress.mana -= 300;
            earthDmgMulti += 0.2f;
            Runes.MarkRuneAsPurchased(earthPath, "Rune 3b");

            earthPath.transform.Find("Rune 3a").GetComponent<Button>().interactable = false;
        }
    }

    //-----// fire runes //-----//

    public static void FireRune1() {
        if (Fortress.mana >= 100) {
            Fortress.mana -= 100;
            fireDmgMulti += 0.1f;
            Runes.MarkRuneAsPurchased(firePath, "Rune 1");

            firePath.transform.Find("Rune 2").GetComponent<Button>().interactable = true;
        }
    }

    public static void FireRune2() {
        if (Fortress.mana >= 200) {
            Fortress.mana -= 200;
            IncreaseGlobalRange(3.5f, "Fire Wizard");
            Runes.MarkRuneAsPurchased(firePath, "Rune 2");

            firePath.transform.Find("Rune 3a").GetComponent<Button>().interactable = true;
            firePath.transform.Find("Rune 3b").GetComponent<Button>().interactable = true;
        }
    }

    public static void FireRune3a() {
        if (Fortress.mana >= 300) {
            Fortress.mana -= 300;
            burnDmgMulti += 0.15f;
            burnDurationMulti += 0.1f;
            Runes.MarkRuneAsPurchased(firePath, "Rune 3a");

            firePath.transform.Find("Rune 3b").GetComponent<Button>().interactable = false;
        }
    }

    public static void FireRune3b() {
        if (Fortress.mana >= 300) {
            Fortress.mana -= 300;
            IncreaseGlobalRange(5f, "Fire Wizard");
            Runes.MarkRuneAsPurchased(firePath, "Rune 3b");

            firePath.transform.Find("Rune 3a").GetComponent<Button>().interactable = false;
        }
    }

    //-----// wind runes //-----//

    public static void WindRune1() {
        if (Fortress.mana >= 100) {
            Fortress.mana -= 100;
            IncreaseGlobalFireRate(0.1f, "Wind Wizard");
            Runes.MarkRuneAsPurchased(windPath, "Rune 1");

            windPath.transform.Find("Rune 2").GetComponent<Button>().interactable = true;
        }
    }

    public static void WindRune2() {
        if (Fortress.mana >= 200) {
            Fortress.mana -= 200;
            IncreaseGlobalRange(2f, "Wind Wizard");
            Runes.MarkRuneAsPurchased(windPath, "Rune 2");

            windPath.transform.Find("Rune 3a").GetComponent<Button>().interactable = true;
            windPath.transform.Find("Rune 3b").GetComponent<Button>().interactable = true;
        }
    }

    public static void WindRune3a() {
        if (Fortress.mana >= 300) {
            Fortress.mana -= 300;
            critDmgMulti += 0.15f;
            Runes.MarkRuneAsPurchased(windPath, "Rune 3a");

            windPath.transform.Find("Rune 3b").GetComponent<Button>().interactable = false;
        }
    }

    public static void WindRune3b() {
        if (Fortress.mana >= 300) {
            Fortress.mana -= 300;
            windDmgMulti += 0.2f;
            Runes.MarkRuneAsPurchased(windPath, "Rune 3b");

            windPath.transform.Find("Rune 3a").GetComponent<Button>().interactable = false;
        }
    }

    //-----// water runes //-----//

    public static void WaterRune1() {
        if (Fortress.mana >= 100) {
            Fortress.mana -= 100;
            IncreaseGlobalRange(3f, "Water Wizard");
            Runes.MarkRuneAsPurchased(waterPath, "Rune 1");

            waterPath.transform.Find("Rune 2").GetComponent<Button>().interactable = true;
        }
    }

    public static void WaterRune2() {
        if (Fortress.mana >= 200) {
            Fortress.mana -= 200;
            waterDmgMulti += 0.1f;
            Runes.MarkRuneAsPurchased(waterPath, "Rune 2");

            waterPath.transform.Find("Rune 3a").GetComponent<Button>().interactable = true;
            waterPath.transform.Find("Rune 3b").GetComponent<Button>().interactable = true;
        }
    }

    public static void WaterRune3a() {
        if (Fortress.mana >= 300) {
            Fortress.mana -= 300;
            splashRadiusMulti += 0.1f;
            Runes.MarkRuneAsPurchased(waterPath, "Rune 3a");

            waterPath.transform.Find("Rune 3b").GetComponent<Button>().interactable = false;
        }
    }

    public static void WaterRune3b() {
        if (Fortress.mana >= 300) {
            Fortress.mana -= 300;
            IncreaseGlobalFireRate(0.15f, "Water Wizard");
            Runes.MarkRuneAsPurchased(waterPath, "Rune 3b");

            waterPath.transform.Find("Rune 3a").GetComponent<Button>().interactable = false;
        }
    }

    //-----// mana runes //-----//

    public static void ManaGainRune1() {
        if (Fortress.mana >= 250) {
            Fortress.mana -= 250;
            HUD_Events.manaGain += 10;
            Runes.MarkRuneAsPurchased(manaGainPath, "Rune 1");

            manaGainPath.transform.Find("Rune 2").GetComponent<Button>().interactable = true;
        }
    }

    public static void ManaGainRune2() {
        if (Fortress.mana >= 400) {
            Fortress.mana -= 400;
            HUD_Events.manaGain += 20;
            Runes.MarkRuneAsPurchased(manaGainPath, "Rune 2");
        }
    }

    public static void ManaRateRune1() {
        if (Fortress.mana >= 300) {
            Fortress.mana -= 300;
            HUD_Events.manaFillDuration -= 1;
            Runes.MarkRuneAsPurchased(manaRatePath, "Rune 1");

            manaRatePath.transform.Find("Rune 2").GetComponent<Button>().interactable = true;
        }
    }

    public static void ManaRateRune2() {
        if (Fortress.mana >= 500) {
            Fortress.mana -= 500;
            HUD_Events.manaFillDuration -= 2;
            Runes.MarkRuneAsPurchased(manaRatePath, "Rune 2");
        }
    }

}