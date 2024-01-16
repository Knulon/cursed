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

        if (Vector2.Distance(transform.position, new Vector2(0, 0)) > 30)
        {
            // here deactivate in bullett pool
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.tag == "Obstacle")
            {
                _bulletPool.AddBullet(this.gameObject);
            }
        }
    }
}
