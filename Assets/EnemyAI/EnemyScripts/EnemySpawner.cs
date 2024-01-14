using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EnemySpawner : MonoBehaviour
{
    private enum Enemytype
    {
        Normal = 0,
        Sniper = 1,
        Runner = 2
    }

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
    private int _numberOfEnemies = 20;
    private int _enemiesSpawned;

    [SerializeField]
    private bool SpawnEnemies = true;

    private float spawnBucket;

    private static EnemyPool enemyPool = new();


    private class EnemyPool
    {
        Stack<GameObject> _pool = new();

        public GameObject GetEnemy(GameObject enemyPrefab, Vector3 position, GameObject exitTrigger)
        {
            if (_pool.Count == 0)
            {
                GameObject enemy = Instantiate(enemyPrefab, position, enemyPrefab.transform.rotation);
                SetEnemyStats(ref enemy, (Enemytype)Random.Range(0, 3), exitTrigger);
                return enemy;
            }

            GameObject enemyFromPool = _pool.Pop();
            enemyFromPool.transform.position = position;
            SetEnemyStats(ref enemyFromPool, (Enemytype)Random.Range(0, 3), exitTrigger);
            enemyFromPool.SetActive(true);
            return enemyFromPool;
        }

        public void ReturnEnemy(GameObject enemy)
        {
            enemy.SetActive(false);
            _pool.Push(enemy);
        }

        public void PrepareEnemies(GameObject enemyPrefab, int count)
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
            enemyPool.PrepareEnemies(_enemyPrefab, 300);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (SpawnEnemies)
        {
            SpawnEnemyAtRate();
        }

        if (_enemiesSpawned >= _numberOfEnemies)
        {
            SpawnEnemies = false;
        }
    }

    void SpawnEnemyAtRate()
    {
        spawnBucket += (_spawnRatePerSecond * Time.deltaTime);

        if (spawnBucket >= 1f)
        {
            int enemiesToSpawn = (int)spawnBucket;

            spawnBucket -= enemiesToSpawn;
            _enemiesSpawned += enemiesToSpawn;
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
        GameObject enemy = enemyPool.GetEnemy(_enemyPrefab, spawnPosition, _exitTrigger);
    }

    static void SetEnemyStats(ref GameObject enemy, Enemytype enemytype, GameObject exitTrigger)
    {
        EnemyInfoManager enemyInfoManager = enemy.GetComponent<EnemyInfoManager>();
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        EnemyWeaponController enemyWeaponController = enemy.GetComponent<EnemyWeaponController>();
        enemyInfoManager.SetExitTrigger(exitTrigger);

        switch (enemytype)
        {
            case Enemytype.Normal:
                enemyInfoManager.Health = 100f;
                break;
            case Enemytype.Sniper:
                enemyInfoManager.Health = 50f;
                enemyWeaponController._fireRate = 0.5f;
                enemyWeaponController._damage = 50f;
                enemyWeaponController._magazineSize = 1;
                enemyWeaponController._reloadTime = 0.25f;
                enemyInfoManager._detectPlayerRadius = 20f;
                enemyInfoManager._attackPlayerRadius = 18f;
                break;
            case Enemytype.Runner:
                enemyInfoManager.Health = 75f;
                enemyController.maxVelocity = 10f;
                enemyController.acceleration = 10f;
                enemyWeaponController._bulletSpread = 50f;
                enemyWeaponController._fireRate = 5f;
                enemyWeaponController._damage = 25f;
                break;
            default:
                Debug.LogError("Invalid enemy type");
                break;
        }
    }

    public void Reset()
    {
        _enemiesSpawned = 0;
        SpawnEnemies = true;
    }

    public static void ReturnEnemy(GameObject enemy)
    {
        enemyPool.ReturnEnemy(enemy);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}
