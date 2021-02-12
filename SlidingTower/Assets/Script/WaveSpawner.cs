﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public Transform enemyPrefab;
    public Transform spawnPoint;

    public float timeBfStart;
    public float timeBtwWave;

    public Text uiCounter;

    [HideInInspector]
    public float timeCounter;

    private int waveIndex;


    private void Start()
    {
        timeCounter = timeBfStart;
    }

    private void Update()
    {

        if (timeCounter <= 0f)
        {
            timeCounter = timeBtwWave;
            StartCoroutine(SpawnWave());
        }

        timeCounter -= Time.deltaTime;

        uiCounter.text = Mathf.Round(timeCounter).ToString();
    }

    IEnumerator SpawnWave()
    {
        waveIndex++;

        for (int i = 0; i < waveIndex; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.3f);
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }
}
