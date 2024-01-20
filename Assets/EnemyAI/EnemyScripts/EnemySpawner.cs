using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public enum Enemytype
    {
        Normal = 0,
        Sniper = 1,
        Runner = 2
    }

    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    [Tooltip("This will have to be added to aleast one EnemySpawner")]
    private EnemySpriteProvider _enemySpriteProvider;
    private static EnemySpriteProvider _enemySpriteProviderStatic;

    [SerializeField]
    private float _spawnRatePerSecond = 0.5f;

    [SerializeField]
    private float _spawnRadius = 5f;

    [SerializeField]
    private GameObject _exitTrigger;

    [SerializeField]
    private int _numberOfEnemies = 20;
    private int _enemiesSpawned;

    [SerializeField]
    private bool SpawnEnemies = true;

    private float spawnBucket;

    private static EnemyPool enemyPool;
    private static bool _enemiesArePrepared = false;


    private class EnemyPool
    {
        Stack<GameObject> _pool = new();

        public GameObject GetEnemy(GameObject enemyPrefab, Vector3 position, GameObject exitTrigger)
        {
            if (_pool.Count == 0)
            {
                GameObject enemy = Instantiate(enemyPrefab, position, enemyPrefab.transform.rotation);
                SetEnemyStats(ref enemy, (Enemytype)Random.Range(0, Enum.GetNames(typeof(Enemytype)).Length), exitTrigger);
                return enemy;
            }
            GameObject enemyFromPool = _pool.Pop();
            for (int i = 0; i < 10 && enemyFromPool == null; i++)
            {
                enemyFromPool = _pool.Pop();
            }
            enemyFromPool.SetActive(true);
            enemyFromPool.transform.position = position;
            SetEnemyStats(ref enemyFromPool, (Enemytype)Random.Range(0, Enum.GetNames(typeof(Enemytype)).Length), exitTrigger);
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
            EnemyInfoManager enemyInfoManager = enemy.GetComponentInChildren<EnemyInfoManager>();
            enemyInfoManager.SetExitTrigger(null);
            enemyInfoManager.SetMaxHealth(100);

            for (int i = 0; i < count; i++)
            {
                ReturnEnemy(Instantiate(enemy));
            }

            Destroy(enemy);
        }

        public int Count()
        {
            return _pool.Count;
        }

    }

    void Awake()
    {

        AStar_Handler aStar_Handler = _enemyPrefab.GetComponentInChildren<AStar_Handler>();
        if (aStar_Handler != null)
        {
            AStarOnNodes aStar = new(aStar_Handler.GetGridOffset(), aStar_Handler.GetNumerator(), aStar_Handler.GetDenominator(), aStar_Handler.GetCollisionBoxSize());
            aStar.BuildBlockedTilesHashMap(new(aStar_Handler.GetNumerator(), aStar_Handler.GetDenominator()), 3000);
        }

        if (!_enemiesArePrepared)
        {
            if (_enemySpriteProviderStatic == null)
            {
                _enemySpriteProviderStatic = _enemySpriteProvider;
            }
            else
            {
                _enemySpriteProvider = _enemySpriteProviderStatic;
            }
            _enemiesArePrepared = true;
            enemyPool = new();
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
        Vector3 spawnOffset = (Random.insideUnitSphere * _spawnRadius) /4;
        if (Vector3.Distance(spawnOffset, Vector3.zero) > (_spawnRadius / 4))
        {
            spawnOffset = spawnOffset.normalized * 1f;
            Debug.LogError("SpawnOffset was weird.");
        }
        Vector3 spawnPosition = transform.position + spawnOffset;
        spawnPosition.z = 0f;

        GameObject enemy = enemyPool.GetEnemy(_enemyPrefab, spawnPosition, _exitTrigger);
    }

    public void SetExitTrigger(GameObject exitTrigger)
    {
        _exitTrigger = exitTrigger;
    }

    static void SetEnemyStats(ref GameObject enemy, Enemytype enemytype, GameObject exitTrigger)
    {
        EnemyInfoManager enemyInfoManager = enemy.GetComponentInChildren<EnemyInfoManager>();
        EnemyController enemyController = enemy.GetComponentInChildren<EnemyController>();
        EnemyWeaponController enemyWeaponController = enemy.GetComponentInChildren<EnemyWeaponController>();
        SpriteRenderer spriteRenderer = enemy.GetComponentInChildren<SpriteRenderer>();
        enemyInfoManager.SetExitTrigger(exitTrigger);

        switch (enemytype)
        {
            case Enemytype.Normal:
                enemyInfoManager.SetMaxHealth(100f);
                enemyWeaponController._reloadTime = 0.25f;
                enemyWeaponController._damage = 11f;
                spriteRenderer.sprite = _enemySpriteProviderStatic.GetSprite(Enemytype.Normal);
                break;
            case Enemytype.Sniper:
                enemyInfoManager.SetMaxHealth(50f);
                enemyController.maxVelocity = 2f;
                enemyController.acceleration = 8f;
                enemyWeaponController._fireRate = 0.5f;
                enemyWeaponController._damage = 17f;
                enemyWeaponController._magazineSize = 1;
                enemyWeaponController._reloadTime = 0.5f;
                enemyInfoManager._detectPlayerRadius = 20f;
                enemyInfoManager._attackPlayerRadius = 18f;
                spriteRenderer.sprite = _enemySpriteProviderStatic.GetSprite(Enemytype.Sniper);
                break;
            case Enemytype.Runner:
                enemyInfoManager.SetMaxHealth(75f);
                enemyController.maxVelocity = 6f;
                enemyController.acceleration = 7f;
                enemyWeaponController._reloadTime = 0.3f;
                enemyWeaponController._bulletSpread = 40f;
                enemyWeaponController._fireRate = 5f;
                enemyWeaponController._damage = 7f;
                spriteRenderer.sprite = _enemySpriteProviderStatic.GetSprite(Enemytype.Runner);
                break;
            default:
                Debug.LogError("Invalid enemy type");
                break;
        }
    }

    public void Reset()
    {
        _enemiesSpawned = 0;
    }

    public void SetStatusOfSpawner(bool spawnEnemies)
    {
        SpawnEnemies = spawnEnemies;
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
