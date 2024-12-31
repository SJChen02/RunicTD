using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWater : Bullet
{
    public float splashRadius = 20f;

    public override void Upgrade()
    {
        splashRadius += 1f;
        damage += 10;
    }

    protected override void HitTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, splashRadius);

        foreach (Collider collider in colliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, "Water");
            }
        }

        Destroy(gameObject);
    }
}
