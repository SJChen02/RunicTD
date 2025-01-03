using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthbarLock : MonoBehaviour
{
    float XLock = 60;
    float lockPos = 0;

    // Update is called once per frame
    // Locks the healthbar to a certain rotation no matter what the enemy's rotation is
    void Update()
    {
        transform.rotation = Quaternion.Euler(XLock, lockPos, lockPos);
    }
}
