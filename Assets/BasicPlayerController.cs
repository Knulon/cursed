using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BasicPlayerController : MonoBehaviour
{
    [SerializeField] public float health = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            // TODO: Distinction between enemy and player bullets
            Debug.Log("Player hit by bullet");
            health -= collider.gameObject.GetComponent<Bullet>().GetDamageAndSendToPool();
        }
    }
}
