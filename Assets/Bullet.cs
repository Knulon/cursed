using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D _rigidbody2D;
    ContactFilter2D _contactFilter2D;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _contactFilter2D = new ContactFilter2D();
        _contactFilter2D.SetLayerMask(LayerMask.GetMask("Enemy"));
    }

    // Update is called once per frame
    void Update()
    {
    }
}
