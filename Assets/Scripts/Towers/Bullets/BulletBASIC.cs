using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBASIC : Bullet
{
    public override void Upgrade()
    {
        damage += 10;
    }
    
    protected override void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
