using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFire : Bullet
{
    public float burnDuration = 3f;
    public float burnDamage = 5f;

    public override void Upgrade()
    {
        burnDuration += 0.5f;
        burnDamage += 1f;
        damage += 10;
    }
    
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
