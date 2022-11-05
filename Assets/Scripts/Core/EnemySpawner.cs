using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyWave[] enemyWaves_;
    public Transform[] spawnPoints_;

    public float spawnDelay_;

    private int currentWave_;
    private int aliveEnemies_;

    void Start()
    {
        currentWave_ = 0;
        aliveEnemies_ = 0;
        StartCoroutine(SpawnWaveCoroutine(currentWave_));
    }

    private void EnemyDeath()
    {
        aliveEnemies_--;
        if (aliveEnemies_ <= 0)
        {
            currentWave_++;
            if(currentWave_ < enemyWaves_.Length)
            {
                SpawnWave(currentWave_);
            }
            else
            {
                SpawnWave(Random.Range(0, enemyWaves_.Length));
            }
        }
    }

    public IEnumerator SpawnWaveCoroutine(int wave)
    {
        Debug.Assert(enemyWaves_[wave].enemies_.Length <= spawnPoints_.Length);
        yield return new WaitForSeconds(spawnDelay_);
        for(int i = 0; i < enemyWaves_[wave].enemies_.Length; ++i)
        {
            Enemy e = Instantiate(enemyWaves_[wave].enemies_[i], spawnPoints_[i].position, Quaternion.identity).GetComponent<Enemy>();
            e.GetComponent<HealthComponent>().onDeath_ += () => EnemyDeath();
            aliveEnemies_++;
        }
        yield return null;
    }

    public void SpawnWave(int wave)
    {
        Debug.Assert(enemyWaves_[wave].enemies_.Length <= spawnPoints_.Length);
        for (int i = 0; i < enemyWaves_[wave].enemies_.Length; ++i)
        {
            Enemy e = Instantiate(enemyWaves_[wave].enemies_[i], spawnPoints_[i].position, Quaternion.identity).GetComponent<Enemy>();
            e.GetComponent<HealthComponent>().onDeath_ += () => EnemyDeath();
            aliveEnemies_++;
        }
    }
}
