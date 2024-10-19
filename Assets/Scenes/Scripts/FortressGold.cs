using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FortressGold : MonoBehaviour {
    // Start is called before the first frame update

    public static int gold = 100;
    public TextMeshProUGUI goldAmount;

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        SetText();
    }

    private void SetText() {
        goldAmount.text = "Gold: " + gold;
    }
}
