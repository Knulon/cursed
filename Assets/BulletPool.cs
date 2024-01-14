using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class BulletPool : ScriptableObject
{
    [DisallowNull]
    private static BulletPool _instance;

    private Stack<GameObject> _pool;

    private BulletPool()
    {
        _pool = new Stack<GameObject>();
    }

    public GameObject GetBullet(GameObject _bulletPrefab, Vector3 position, float Damage, Collider2D myCollider2D, LayerMask bulletLayer)
    {
        if (_pool.Count == 0)
        {
            GameObject bullet = Instantiate(_bulletPrefab, position, _bulletPrefab.transform.rotation);
            Bullet bulletScriptComponent = bullet.GetComponent<Bullet>();
            bulletScriptComponent.Damage = Damage;
            bulletScriptComponent.SetCollider(myCollider2D);
            bulletScriptComponent.ResetTimeToLive();
            bullet.layer = bulletLayer;
            return bullet;
        }

        // TODO: This fails if the bulletPrefab is not the same as the one in the pool
        GameObject popBullet = _pool.Pop();
        popBullet.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        popBullet.transform.position = position;
        popBullet.transform.rotation = _bulletPrefab.transform.rotation;
        popBullet.layer = bulletLayer;
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

    public static BulletPool GetInstance(GameObject bulletPrefab, Collider2D myCollider2D)
    {
        if (_instance == null)
        {
            _instance = CreateInstance<BulletPool>();
            _instance.PrepareBullets(300, bulletPrefab, Vector3.zero, 0, myCollider2D);
        }
        return _instance;
    }

}