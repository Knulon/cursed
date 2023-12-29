using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BasicPlayerController : MonoBehaviour
{
    [SerializeField] public float health = 100f;
    public bool HasKey { get; set; } = false;

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
        Debug.Log("Player collided with " + collider.gameObject.name);
        if (collider.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            // TODO: Distinction between enemy and player bullets
            //Debug.Log("Player hit by bullet");
            health -= collider.gameObject.GetComponent<Bullet>().GetDamageAndSendToPool();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Player triggered " + collider.gameObject.name);
        if (collider.gameObject.name == "Key")
        {
            HasKey = true;
            Destroy(collider.gameObject);
        }
    }
}
