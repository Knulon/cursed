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
    }
}
