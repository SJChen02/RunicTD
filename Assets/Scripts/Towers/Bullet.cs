using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public string towerName;

    [Header("Do Not Edit")]
    public Transform target;
    public float damage = 40;
    public static float speed = 70f;
    public float stunDuration;
    public float burnDuration;
    public float burnDamage;
    public float splashRadius;
    public float critChance;
    public float critDamage;

    // Set target functions and variables
    public void SeekEarth(Transform _target, float _damage, float _stunDuration) {
        target = _target;
        damage = _damage;
        stunDuration = _stunDuration;
    }
    public void SeekFire(Transform _target, float _damage, float _burnDuration, float _burnDamage) {
        target = _target;
        damage = _damage;
        burnDuration = _burnDuration;
        burnDamage = _burnDamage;
    }
    public void SeekWind(Transform _target, float _damage, float _critChance, float _critDamage) {
        target = _target;
        damage = _damage;
        critChance = _critChance;
        critDamage = _critDamage;
    }
    public void SeekWater(Transform _target, float _damage, float _splashRadius) {
        target = _target;
        damage = _damage;
        splashRadius = _splashRadius;
    }

    void Update() {
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        // register bullet hit
        if (dir.magnitude <= distanceThisFrame) { //dir.magnitude is distance to target
            switch (towerName) {
                case "Earth Wizard":
                    BulletEarth.HitTarget(target, damage, stunDuration);
                    Destroy(gameObject);
                    return;
                case "Fire Wizard":
                    BulletFire.HitTarget(target, damage, burnDuration, burnDamage);
                    Destroy(gameObject);
                    return;
                case "Wind Wizard":
                    BulletWind.HitTarget(target, damage, critChance, critDamage);
                    Destroy(gameObject);
                    return;
                case "Water Wizard":
                    BulletWater.HitTarget(target, damage, splashRadius, gameObject.transform.position);
                    Destroy(gameObject);
                    return;
            }
        }
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }
}
