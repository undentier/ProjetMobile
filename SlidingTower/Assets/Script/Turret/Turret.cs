﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    #region Variable
    [Header ("Basic stats")]
    public float startFireRate = 3;
    public float startDamage;
    public float startRange;
    public float rotationSpeed;
    public int numMaxTargets = 5;
    public int numOfCanon = 1;


    [Header ("Value of stat upgrade")]
    public int[] fireRateBonus;
    public int[] damageBonus;
    public int[] rangeBonus;

    private float actualFireRate;
    private float actualDamage;
    private float actualRange;

    [Header ("Value of negatif effect")]
    public float[] slowForceBonus;
    public float[] slowDurationBonus;
    private float actualSlowForce;
    private float actualSlowDuration;
    [Space]
    public float[] poisonDamageBonus;
    public float[] poisonDurationBonus;
    public float[] poisonTickBonus;
    private float actualpoisonDamage;
    private float actualPoisonDuration;
    private float actualPoisonTick;

    [Header("Value of shooting type")]
    public float[] explosionRadiusBonus;
    private float actualExplosionRadius;

    [Header ("Unity setup")]
    public Transform partToRotate;
    public GameObject basicBullet;
    public GameObject explosiveBullet;
    public Transform shootPoint;

    [HideInInspector]
    public Enemy[] targets;
    private GameObject bulletToShoot;
    private List<Enemy> copyList = new List<Enemy>();
    private float fireCoolDown;

    #region Upgrade variable
    [HideInInspector]
    public int laserUpgrade;
    [HideInInspector]
    public int explosionUpgrade;

    [HideInInspector]
    public int slowUpgrade;
    [HideInInspector]
    public int poisonUpgrade;

    [HideInInspector]
    public int fireRateUpgrade;
    [HideInInspector]
    public int damageUpgrade;
    [HideInInspector]
    public int rangeUpgrade;
    #endregion

    #endregion

    void Start()
    {
        bulletToShoot = basicBullet;
        targets = new Enemy[numMaxTargets];
    }

    void FixedUpdate()
    {
        FindTargets();
        MultiShoot();
    }

    void FindTargets()
    {
        copyList = new List<Enemy>(WaveSpawner.instance.enemyList);

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null)
            {
                if (copyList.Count >= i + 1)
                {
                    if (copyList[i] != null)
                    {
                        if (Vector3.Distance(transform.position, copyList[i].transform.position) < actualRange)
                        {
                            targets[i] = copyList[i];
                            copyList.Remove(copyList[i]);
                        }
                    }
                }
            }
            if (targets[i] != null)
            {
                if (Vector3.Distance(transform.position ,targets[i].transform.position) > actualRange)
                {
                    targets[i] = null;
                }
            }
        }
    }

    void MultiShoot()
    {
        if (targets[0] != null)
        {
            AimTarget();

            if (fireCoolDown <= 0f)
            {
                for (int i = 0; i < numOfCanon; i++)
                {
                    if (targets[i] != null)
                    {
                        Fire(targets[i]);
                    }
                }
                fireCoolDown = 1f / actualFireRate;
            }
            fireCoolDown -= Time.deltaTime;
        }
    }
    void AimTarget()
    {
        Vector3 dir = targets[0].transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
    void Fire(Enemy target)
    {
        GameObject actualBullet = Instantiate(bulletToShoot, shootPoint.position, shootPoint.rotation);
        Bullet bulletScript = actualBullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.GetTarget(target.transform);
            bulletScript.GetDamage(actualDamage);
            bulletScript.GetSlowInfo(actualSlowForce, actualSlowDuration);
            bulletScript.GetPoisonInfo(actualpoisonDamage, actualPoisonDuration, actualPoisonTick);
            bulletScript.GetExplosiveInfo(actualExplosionRadius);
        }
    }

    public void GetNodeUpgrade(NodeSysteme node)
    {
        laserUpgrade = node.laserUpgrade;
        explosionUpgrade = node.explosionUpgrade;

        slowUpgrade = node.slowUpgrade;
        poisonUpgrade = node.poisonUpgrade;

        fireRateUpgrade = node.fireRateUpgrade;
        damageUpgrade = node.damageUpgrade;
        rangeUpgrade = node.rangeUpgrade;

        ApplyUpgrade();
    }
    public void ResetUpgrade()
    {
        laserUpgrade = 0;
        explosionUpgrade = 0;

        slowUpgrade = 0;
        poisonUpgrade = 0;

        fireRateUpgrade = 0;
        damageUpgrade = 0;
        rangeUpgrade = 0;

        ApplyUpgrade();
    }

    void ApplyUpgrade()
    {
        #region Stats boost
        switch (fireRateUpgrade)
        {
            case 0:
                actualFireRate = startFireRate;
                break;
            case 1:
                actualFireRate = fireRateBonus[0];
                break;
            case 2:
                actualFireRate = fireRateBonus[1];
                break;
            case 3:
                actualFireRate = fireRateBonus[2];
                break;
            case 4:
                actualFireRate = fireRateBonus[3];
                break;
        }

        switch (damageUpgrade)
        {
            case 0:
                actualDamage = startDamage;
                break;
            case 1:
                actualDamage = damageBonus[0];
                break;
            case 2:
                actualDamage = damageBonus[1];
                break;
            case 3:
                actualDamage = damageBonus[2];
                break;
            case 4:
                actualDamage = damageBonus[3];
                break;
        }

        switch (rangeUpgrade)
        {
            case 0:
                actualRange = startRange;
                break;
            case 1:
                actualRange = rangeBonus[0];
                break;
            case 2:
                actualRange = rangeBonus[1];
                break;
            case 3:
                actualRange = rangeBonus[2];
                break;
            case 4:
                actualRange = rangeBonus[3];
                break;
        }
        #endregion

        #region Negatif effect boost
        switch (slowUpgrade)
        {
            case 0:
                actualSlowForce = 0;
                actualSlowDuration = 0;
                break;
            case 1:
                actualSlowForce = slowForceBonus[0];
                actualSlowDuration = slowDurationBonus[0];
                break;
            case 2:
                actualSlowForce = slowForceBonus[1];
                actualSlowDuration = slowDurationBonus[1];
                break;
            case 3:
                actualSlowForce = slowForceBonus[2];
                actualSlowDuration = slowDurationBonus[2];
                break;
            case 4:
                actualSlowForce = slowForceBonus[3];
                actualSlowDuration = slowDurationBonus[3];
                break;
        }

        switch (poisonUpgrade)
        {
            case 0:
                actualpoisonDamage = 0f;
                actualPoisonDuration = 0f;
                actualPoisonTick = 0f;
                break;
            case 1:
                actualpoisonDamage = poisonDamageBonus[0];
                actualPoisonDuration = poisonDurationBonus[0];
                actualPoisonTick = poisonTickBonus[0];
                break;
            case 2:
                actualpoisonDamage = poisonDamageBonus[1];
                actualPoisonDuration = poisonDurationBonus[1];
                actualPoisonTick = poisonTickBonus[1];
                break;
            case 3:
                actualpoisonDamage = poisonDamageBonus[2];
                actualPoisonDuration = poisonDurationBonus[2];
                actualPoisonTick = poisonTickBonus[2];
                break;
            case 4:
                actualpoisonDamage = poisonDamageBonus[3];
                actualPoisonDuration = poisonDurationBonus[3];
                actualPoisonTick = poisonTickBonus[3];
                break;
        }
        #endregion

        switch (explosionUpgrade)
        {
            case 0:
                bulletToShoot = basicBullet;
                actualExplosionRadius = 0f;
                break;
            case 1:
                bulletToShoot = explosiveBullet;
                actualExplosionRadius = explosionRadiusBonus[0];
                break;
            case 2:
                bulletToShoot = explosiveBullet;
                actualExplosionRadius = explosionRadiusBonus[1];
                break;
            case 3:
                bulletToShoot = explosiveBullet;
                actualExplosionRadius = explosionRadiusBonus[2];
                break;
            case 4:
                bulletToShoot = explosiveBullet;
                actualExplosionRadius = explosionRadiusBonus[3];
                break;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, actualRange);
    }
}
