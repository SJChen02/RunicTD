using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletIce : Bullet
{
    public float slowDuration = 2f;
    public float slowAmount = 0.5f;

    public override void Upgrade()
    {
        slowDuration += 0.5f;
        slowAmount += 0.1f;
        damage += 10;
    }

    protected override void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, "Ice");
            enemy.ApplySlowDebuff(slowAmount, slowDuration);
        }

        Destroy(gameObject);
    }
}
