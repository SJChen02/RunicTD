using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEarth : Bullet
{
    public float stunDuration = 1f;

    public override void Upgrade()
    {
        stunDuration += 0.5f;
        damage += 10;
    }

    protected override void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, "Earth");
            enemy.ApplyStunDebuff(stunDuration);
        }

        Destroy(gameObject);
    }
}
