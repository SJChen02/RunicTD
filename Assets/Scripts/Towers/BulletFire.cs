using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFire : Bullet
{
    public float burnDuration = 3f;
    public float burnDamage = 5f;

    protected override void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, "Fire");
            enemy.ApplyBurnDebuff(burnDamage, burnDuration);
        }

        Destroy(gameObject);
    }
}
