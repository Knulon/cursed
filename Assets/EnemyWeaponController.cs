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

    // Start is called before the first frame update
    void Start()
    {
        _bulletsInMagazine = _magazineSize;
        _enemyController = GetComponent<EnemyController>();
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
    }

    public void Shoot(Vector2 shootDirection)
    {
        _bulletsToFire += _fireRate * Time.deltaTime;

        if (_bulletsToFire >= 1 && _bulletsInMagazine != 0)
        {
            _bulletsToFire -= 1;
            _bulletsInMagazine--;

            shootDirection.Normalize();

            GameObject bullet = Instantiate(_bulletPrefab, transform.position, _bulletPrefab.transform.rotation);

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
