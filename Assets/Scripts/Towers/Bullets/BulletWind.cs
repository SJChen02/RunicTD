using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWind : Bullet
{
    public float critChance = 0.2f;  // 20% chance to land a critical hit
    public float critDamage = 1.5f;  // 50% extra damage on crit (1.5 means 150%)

    //public float slowDuration = 2f;
    //public float slowAmount = 0.5f;

    protected override void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Determine if the shot is a critical hit
            float roll = Random.Range(0f, 1f);
            int finalDamage = damage;

            if (roll <= critChance) // If roll is within crit chance
            {
                finalDamage = Mathf.CeilToInt(damage * critDamage);
                // Debug.Log("Critical Hit");
            }

            enemy.TakeDamage(finalDamage, "Wind");
            //enemy.TakeDamage(damage, "Ice");
            //enemy.ApplySlowDebuff(slowAmount, slowDuration);
        }

        Destroy(gameObject);
    }
}
