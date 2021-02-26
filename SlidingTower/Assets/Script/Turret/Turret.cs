﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header ("Turret Basic Stats")]
    public float range;
    public float fireRate;
    public int damage;
    public float rotationSpeed;

    [Header("Upgrade Stats")]

    public int lazerUpgrade;
    public int doubleShootUpgrade;
    public int explosionUpgrade;

    [Space]

    public int slowValueUpgrade;
    public int poisonValueUpgrade;

    [Header ("Unity Setup")]
    public Transform partToRotate;
    public GameObject bulletPrefab;
    public Transform shootPoint;


    private float fireCooldown;
    private Transform target;
    private BoostBlock boostScript;


    private void Update()
    {
        FindTarget();

        if (target == null)
        {
            return;
        }
        else
        {
            AimTarget();
        }

        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
        fireCooldown -= Time.deltaTime;


    }

    void FindTarget()
    {
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Transform enemy in WaveSpawner.enemyList)
        {
            if (enemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void AimTarget()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Shoot()
    {
        GameObject actualBullet =  Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        Bullet bulletScript = actualBullet.GetComponent<Bullet>();
        
        if (bulletScript != null)
        {
            bulletScript.GetTarget(target);
        }
    }

    void GetUpgrade()
    {
        lazerUpgrade += boostScript.lazer;
        doubleShootUpgrade += boostScript.doubleShoot;
        explosionUpgrade += boostScript.explosion;

        slowValueUpgrade += boostScript.slowValue;
        poisonValueUpgrade += boostScript.poisonValue;

        fireRate += boostScript.fireRateBoost;
        damage += boostScript.damageBoost;
        range += boostScript.rangeBoost;
    }
    void DelUpgrade()
    {
        lazerUpgrade -= boostScript.lazer;
        doubleShootUpgrade -= boostScript.doubleShoot;
        explosionUpgrade -= boostScript.explosion;

        slowValueUpgrade -= boostScript.slowValue;
        poisonValueUpgrade -= boostScript.poisonValue;

        fireRate -= boostScript.fireRateBoost;
        damage -= boostScript.damageBoost;
        range -= boostScript.rangeBoost;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Je rentre");
        if (other.gameObject.layer == 9)
        {
            boostScript = other.GetComponent<BoostBlock>();
            GetUpgrade();
            boostScript = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            boostScript = other.GetComponent<BoostBlock>();
            DelUpgrade();
            boostScript = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
