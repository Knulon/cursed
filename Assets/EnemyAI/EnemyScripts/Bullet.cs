using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Collider2D _collider;
    private static BulletPool BulletPool;

    public float Damage
    {
        get;
        set;
    } = 1f;

    private float _timeToLive = 5f;

    private void Start()
    {
        if (BulletPool == null)
        {
            BulletPool = BulletPool.GetInstance(gameObject);
        }
    }

    private void Update()
    {
        _timeToLive -= Time.deltaTime;
        if (_timeToLive <= 0)
        {
            BulletPool.AddBullet(this.gameObject);
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 13)
        {

            BulletPool.AddBullet(this.gameObject);
            Debug.Log("Bullet hit obstacle");
        }
    }

    public float GetDamageAndSendToPool()
    {
        BulletPool.AddBullet(this.gameObject);
        return Damage;
    }

    public void ResetTimeToLive()
    {
        _timeToLive = 5f;
    }

    public void SetCollider(Collider2D collider)
    {
        _collider = collider;
        Physics2D.IgnoreCollision(_collider, GetComponent<Collider2D>());
    }
}
