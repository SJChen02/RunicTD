using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEarth : Bullet
{
    public float stunDuration = 1f;

    protected override void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            enemy.ApplyStunDebuff(stunDuration);
        }

        Destroy(gameObject);
    }
}
