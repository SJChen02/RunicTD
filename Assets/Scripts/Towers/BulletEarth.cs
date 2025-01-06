using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEarth : MonoBehaviour {
    // earth elemental damage
    public static void HitTarget(Transform target, float damage, float stunDuration) {
        Enemy enemy = target.GetComponent<Enemy>();

        if (enemy != null) {
            enemy.TakeDamage(damage, "Earth");
            enemy.ApplyStunDebuff(stunDuration);
        }
    }
}
