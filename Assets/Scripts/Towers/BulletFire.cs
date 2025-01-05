using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFire : MonoBehaviour {
    public static void HitTarget(Transform target, float damage, float burnDuration, float burnDamage) {
        Enemy enemy = target.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage, "Fire");
            enemy.ApplyBurnDebuff(burnDamage, burnDuration);
        }
    }
}
