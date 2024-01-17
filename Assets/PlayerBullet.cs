using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBullet : MonoBehaviour
{
    // Start is called before the first frame update
    

    // Start is called before the first frame update
    public float speed = 50f;
    public GameObject player;

    private static BulletPool _bulletPool;

    public float Damage;
    void Start()
    {
        _bulletPool = BulletPool.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        if (Vector2.Distance(transform.position, player.transform.position) > 30)
        {
            // here deactivate in bullett pool
            SendToPool();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISSSSSSIOOOON with " + collision.collider.tag);
        if (collision != null)
        {
            if (collision.gameObject.tag == "Obstacle")
            {
                // here deactivate in bullett pool
                SendToPool();
            }
        }
    }
}
