using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfoManager : MonoBehaviour
{
    private GameObject player;
    private Collider2D myCollider;
    private bool playerInSight;
    private bool playerInAttackRange;



    [SerializeField]
    private float _health = 100f;

    private float _maxHealth = 100f;

    [SerializeField]
    public float _detectPlayerRadius = 5f;

    [SerializeField]
    public float _attackPlayerRadius = 1f;

    [SerializeField]
    public float _pathPointReachedRadius = 0.1f;

    [SerializeField]
    private float _playerMovedTooMuchRadius = 0.5f;

    [SerializeField] private GameObject _exitTrigger;

    [SerializeField] private GameObject _healthBar;

    private Vector3 _healthBarTransformScale;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        myCollider = gameObject.GetComponent<Collider2D>();
        _healthBarTransformScale =  _healthBar.transform.localScale;
    }


    public bool SetPlayerInSight(bool value)
    {
        playerInSight = value;
        return playerInSight;
    }

    public float GetDetectPlayerRadius()
    {
        return _detectPlayerRadius;
    }

    public float GetAttackPlayerRadius()
    {
        return _attackPlayerRadius;
    }

    public float GetPathPointReachedRadius()
    {
        return _pathPointReachedRadius;
    }

    public float GetPlayerMovedTooMuchRadius()
    {
        return _playerMovedTooMuchRadius;
    }

    public GameObject GetExitTrigger()
    {
        return _exitTrigger;
    }

    public void SetExitTrigger(GameObject value)
    {
        _exitTrigger = value;
    }

    public void SetHealth(float value)
    {
        _health = value;
        _health = Mathf.Clamp(value, 0, _maxHealth);
        _healthBar.transform.localScale = new Vector3(_health / _maxHealth * _healthBarTransformScale.x, _healthBarTransformScale.y, 1);
        if (_health > _maxHealth-0.5f)
        {
            _healthBar.SetActive(false);
        }
        else
        {
            _healthBar.SetActive(true);
        }
    }

    public void SetMaxHealth(float value)
    {
        _maxHealth = value;
        SetHealth(_health);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with:" + collision.gameObject.name);
        if (collision.gameObject.layer.Equals(11))
        {
            SetHealth(_health - collision.gameObject.GetComponent<Bullet>().GetDamageAndSendToPool());
            if (_health <= 0)
            {
                EnemySpawner.ReturnEnemy(gameObject.transform.parent.gameObject);
            }
        }
    }


    public void OnDrawGizmos()
    {
        if (playerInSight)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(gameObject.transform.position, _detectPlayerRadius);

        if (player != null)
        {
            Gizmos.DrawLine(gameObject.transform.position, player.transform.position);
        }

        if (playerInAttackRange)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.black;
        }
        Gizmos.DrawWireSphere(gameObject.transform.position, _attackPlayerRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, _pathPointReachedRadius);
    }
}
