using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlayerController : MonoBehaviour
{
    [SerializeField] public float health = 100f;
    
    public bool HasKey { get; private set; } = false;

    [SerializeField] public bool Key = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HasKey = Key;
    }



    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.layer.Equals(12))
        {
            health -= collider.gameObject.GetComponent<Bullet>().GetDamageAndSendToPool();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Key")
        {
            Debug.Log("Key has been picked up.");
            HasKey = true;
            Destroy(collider.gameObject);
        }
        if(collider.gameObject.name == "ExitTrigger")
        {
            Debug.Log("Player has reached the exit.");
            HasKey = false;
            // TODO: Level transition as in: Destroy all enemies, close doors, spawn new enemies, spawn key, etc.
            Destroy(collider.gameObject);
        }
    }
}
