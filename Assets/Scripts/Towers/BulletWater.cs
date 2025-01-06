using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWater : MonoBehaviour {
    // water elemental damage
    public static void HitTarget(Transform target, float damage, float splashRadius, Vector3 bulletPosition) {

        // deal normal damage if area of effect is 0
        if (splashRadius == 0f) {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(damage);
            }
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(bulletPosition, splashRadius);
        foreach (Collider collider in colliders) {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, "Water");
            }
        }
    }
}
