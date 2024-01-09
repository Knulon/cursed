using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private float _spawnRatePerSecond = 0.5f;

    [SerializeField]
    private float _spawnRadius = 5f;

    [SerializeField]
    private GameObject _exitTrigger;

    [SerializeField]
    private GameObject _key;

    [SerializeField]
    private bool SpawnEnemies = true;

    private float spawnBucket;

    private static EnemyPool enemyPool = new EnemyPool();


    private class EnemyPool
    {
        Stack<GameObject> _pool = new();

        public GameObject GetEnemy(GameObject enemyPrefab,Vector3 position, GameObject exitTrigger)
        {
            if (_pool.Count == 0)
            {
                GameObject enemy = Instantiate(enemyPrefab, position, enemyPrefab.transform.rotation);
                EnemyInfoManager enemyInfoManager = enemy.GetComponent<EnemyInfoManager>();
                enemyInfoManager.SetExitTrigger(exitTrigger);
                enemyInfoManager.Health = 100f;
                return enemy;
            }

            GameObject enemyFromPool = _pool.Pop();
            enemyFromPool.transform.position = position;
            EnemyInfoManager enemyInfoManagerFromPool = enemyFromPool.GetComponent<EnemyInfoManager>();
            enemyInfoManagerFromPool.SetExitTrigger(exitTrigger);
            enemyInfoManagerFromPool.Health = 100f;
            enemyFromPool.SetActive(true);
            return enemyFromPool;
        }

        public void ReturnEnemy(GameObject enemy)
        {
            enemy.SetActive(false);
            _pool.Push(enemy);
        }

        public void PrepareEnemies(GameObject enemyPrefab,int count)
        {
            GameObject enemy = Instantiate(enemyPrefab, Vector3.zero, enemyPrefab.transform.rotation);
            EnemyInfoManager enemyInfoManager = enemy.GetComponent<EnemyInfoManager>();
            enemyInfoManager.SetExitTrigger(null);
            enemyInfoManager.Health = 100f;

            for (int i = 0; i < count; i++)
            {
                ReturnEnemy(Instantiate(enemy));
            }
        }

        public int Count()
        {
            return _pool.Count;
        }

    }

    void Start()
    {
        if (enemyPool.Count() == 0)
        {
            enemyPool.PrepareEnemies(_enemyPrefab,300);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (SpawnEnemies)
        {
            SpawnEnemyAtRate();
            Debug.Log(enemyPool.Count());
        }
    }

    void SpawnEnemyAtRate()
    {
        spawnBucket += (_spawnRatePerSecond * Time.deltaTime) * (1 - Random.Range(-0.3f, 0.3f));

        if (spawnBucket >= 1f)
        {
            int enemiesToSpawn = (int)spawnBucket;

            spawnBucket -= enemiesToSpawn;
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * _spawnRadius;
        spawnPosition.z = 0f;
        GameObject enemy = enemyPool.GetEnemy(_enemyPrefab,spawnPosition,_exitTrigger);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}
