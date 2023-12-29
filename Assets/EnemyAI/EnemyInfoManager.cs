using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfoManager : MonoBehaviour
{
    private GameObject player;
    private Collider2D playerCollider;
    private Collider2D[] colliders;
    private bool playerInSight;
    private bool playerInAttackRange;

    [SerializeField]
    private float detectPlayerRadius = 5f;

    [SerializeField]
    private float attackPlayerRadius = 1f;

    [SerializeField]
    private float pathPointReachedRadius = 0.1f;

    [SerializeField]
    private float playerMovedTooMuchRadius = 0.5f;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<Collider2D>();
        colliders = gameObject.GetComponents<Collider2D>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool SetPlayerInSight(bool value)
    {
        playerInSight = value;
        return playerInSight;
    }

    public float GetDetectPlayerRadius()
    {
        return detectPlayerRadius;
    }

    public float GetAttackPlayerRadius()
    {
        return attackPlayerRadius;
    }

    public float GetPathPointReachedRadius()
    {
        return pathPointReachedRadius;
    }

    public float GetPlayerMovedTooMuchRadius()
    {
        return playerMovedTooMuchRadius;
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

        Gizmos.DrawWireSphere(gameObject.transform.position, detectPlayerRadius);

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
        Gizmos.DrawWireSphere(gameObject.transform.position, attackPlayerRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, pathPointReachedRadius);
    }
}
