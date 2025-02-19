using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWind : MonoBehaviour {
    // wind elemental damage
    public static void HitTarget(Transform target, float damage, float critChance, float critDamage) {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null) {
            if (critChance == 0) {
                enemy.TakeDamage(damage);
                return;
            }

            // Determine if the shot is a critical hit
            float roll = Random.Range(0f, 1f);
            float finalDamage = damage;

            if (roll <= critChance) // If roll is within crit chance
            {
                finalDamage = Mathf.CeilToInt(damage * critDamage);
            }

            enemy.TakeDamage(finalDamage, "Wind");
        }
    }
}
