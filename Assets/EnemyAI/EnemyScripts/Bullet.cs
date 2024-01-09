using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Collider2D _collider;

    public float Damage
    {
        get;
        set;
    } = 1f;

    private float _timeToLive = 5f;

    private void Update()
    {
        _timeToLive -= Time.deltaTime;
        if (_timeToLive <= 0)
        {
            EnemyWeaponController.AddBulletToPool(this.gameObject);
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 13)
        {
            EnemyWeaponController.AddBulletToPool(this.gameObject);
            Debug.Log("Bullet hit obstacle");
        }
    }

    public float GetDamageAndSendToPool()
    {
        EnemyWeaponController.AddBulletToPool(this.gameObject);
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
