using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyWeaponController : MonoBehaviour
{
    [SerializeField]
    public float BulletRotationOffset = -90f;

    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private float _bulletSpeed = 5f;

    [SerializeField]
    private float _damage = 1f;

    [SerializeField]
    private float _fireRate = 1f; // Bullets per second

    [SerializeField]
    private int _magazineSize = 10;
    private int _bulletsInMagazine;

    [SerializeField]
    private float _reloadTime = 1f; // Seconds

    [SerializeField]
    private float _bulletSpread = 0.1f;

    private float _bulletsToFire = 1f;
    private float _reloadTimeLeft = 0f;

    private EnemyController _enemyController;
    private static BulletPool _bulletPool = new BulletPool();
    private Collider2D _myCurrenCollider2D;

    [SerializeField]
    public int BulletPoolItems = 0;

    class BulletPool
    {
        private Stack<GameObject> _pool = new Stack<GameObject>();

        public GameObject GetBullet(GameObject _bulletPrefab, Vector3 position, float Damage, Collider2D myCollider2D)
        {
            if (_pool.Count == 0)
            {
                GameObject bullet = Instantiate(_bulletPrefab, position, _bulletPrefab.transform.rotation);
                Bullet bulletScriptComponent = bullet.GetComponent<Bullet>();
                bulletScriptComponent.Damage = Damage;
                bulletScriptComponent.SetCollider(myCollider2D);
                bulletScriptComponent.ResetTimeToLive();
                return bullet;
            }

            // TODO: This fails if the bulletPrefab is not the same as the one in the pool
            GameObject popBullet = _pool.Pop();
            popBullet.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            popBullet.transform.position = position;
            popBullet.transform.rotation = _bulletPrefab.transform.rotation;
            Bullet popBulletScriptComponent = popBullet.GetComponent<Bullet>();
            popBulletScriptComponent.ResetTimeToLive();
            popBulletScriptComponent.Damage = Damage;
            popBulletScriptComponent.SetCollider(myCollider2D);
            popBullet.SetActive(true);
            return popBullet;
        }

        public void AddBullet(GameObject bullet)
        {
            bullet.SetActive(false);
            _pool.Push(bullet);
        }

        public int Count()
        {
            return _pool.Count;
        }

        public void PrepareBullets(int i, GameObject _bulletPrefab, Vector3 position, float Damage, Collider2D myCollider2D)
        {
            for (int j = 0; j < i; j++)
            {
                GameObject bullet = Instantiate(_bulletPrefab, Vector3.zero, _bulletPrefab.transform.rotation);
                bullet.SetActive(false);
                _pool.Push(bullet);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _bulletsInMagazine = _magazineSize;
        _enemyController = GetComponent<EnemyController>();
        _myCurrenCollider2D = GetComponent<Collider2D>();
        _bulletPool.PrepareBullets(_magazineSize, _bulletPrefab, Vector3.zero, _damage, _myCurrenCollider2D);
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemyController.HumanControlled)
        {
            HumanShoot();
        }
        if (_bulletsInMagazine == 0)
        {
            _reloadTimeLeft += Time.deltaTime;
            _bulletsToFire = 1;
        }
        if (_reloadTimeLeft >= _reloadTime)
        {
            _bulletsInMagazine = _magazineSize;
            _reloadTimeLeft = 0;
        }

        BulletPoolItems = _bulletPool.Count();
    }

    public void Shoot(Vector2 shootDirection)
    {
        _bulletsToFire += _fireRate * Time.deltaTime;

        if (_bulletsToFire >= 1 && _bulletsInMagazine != 0)
        {
            _bulletsToFire -= 1;
            _bulletsInMagazine--;

            shootDirection.Normalize();

            GameObject bullet = _bulletPool.GetBullet(_bulletPrefab, transform.position, _damage, _myCurrenCollider2D);

            Vector2 rotatedShootDirection = shootDirection;
            float randomAngle = Random.Range(-_bulletSpread, _bulletSpread);
            rotatedShootDirection.x = shootDirection.x * Mathf.Cos(randomAngle * Mathf.Deg2Rad) - shootDirection.y * Mathf.Sin(randomAngle * Mathf.Deg2Rad);
            rotatedShootDirection.y = shootDirection.x * Mathf.Sin(randomAngle * Mathf.Deg2Rad) + shootDirection.y * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
            rotatedShootDirection.Normalize();

            float angle = Mathf.Atan2(rotatedShootDirection.y, rotatedShootDirection.x) * Mathf.Rad2Deg + BulletRotationOffset;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            bullet.GetComponent<Rigidbody2D>().velocity = rotatedShootDirection * _bulletSpeed;
        }
    }

    public void HumanShoot()
    {
        if (Input.GetButton("Fire1"))
        {
            Shoot(_enemyController.Direction);
            Debug.Log("Shoot");
        }
    }

    public static void AddBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        _bulletPool.AddBullet(bullet);
    }


    void OnDrawGizmos()
    {
        if (_bulletsInMagazine == 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position + Vector3.up, new(2, .5f, (1 - _reloadTimeLeft / _reloadTime) * 5));
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position + Vector3.up, new(2, .5f, (_bulletsInMagazine / _magazineSize) * 5));
        }

        if (_enemyController == null)
        {
            return;
        }

        // Draw bullet spread with two lines from the center
        Vector2 shootDirection = _enemyController.Direction;

        Vector2 rightSpreadBoundary = shootDirection;
        rightSpreadBoundary.x = shootDirection.x * Mathf.Cos(_bulletSpread * Mathf.Deg2Rad) - shootDirection.y * Mathf.Sin(_bulletSpread * Mathf.Deg2Rad);
        rightSpreadBoundary.y = shootDirection.x * Mathf.Sin(_bulletSpread * Mathf.Deg2Rad) + shootDirection.y * Mathf.Cos(_bulletSpread * Mathf.Deg2Rad);
        rightSpreadBoundary.Normalize();

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)rightSpreadBoundary * 5);

        Vector2 leftSpreadBoundary = shootDirection;
        leftSpreadBoundary.x = shootDirection.x * Mathf.Cos(-_bulletSpread * Mathf.Deg2Rad) - shootDirection.y * Mathf.Sin(-_bulletSpread * Mathf.Deg2Rad);
        leftSpreadBoundary.y = shootDirection.x * Mathf.Sin(-_bulletSpread * Mathf.Deg2Rad) + shootDirection.y * Mathf.Cos(-_bulletSpread * Mathf.Deg2Rad);
        leftSpreadBoundary.Normalize();
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)leftSpreadBoundary * 5);
    }
}
