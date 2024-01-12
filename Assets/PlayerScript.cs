using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerMoveScript : MonoBehaviour
{

    public class BulletPool
    {
        private Stack<GameObject> _pool = new Stack<GameObject>();

        public GameObject GetBullet(GameObject _bulletPrefab, Vector3 position, Quaternion rotation,  float Damage)
        {
            if (_pool.Count == 0)
            {
                Debug.Log("new Bullet created!");
                GameObject bullet = Instantiate(_bulletPrefab, position, rotation);
                PlayerBullet bulletScriptComponent = bullet.GetComponent<PlayerBullet>();
                bulletScriptComponent.Damage = Damage;
                return bullet;
            }

            // TODO: This fails if the bulletPrefab is not the same as the one in the pool
            GameObject popBullet = _pool.Pop();
            popBullet.transform.position = position;
            popBullet.transform.rotation = rotation;
            PlayerBullet popBulletScriptComponent = popBullet.GetComponent<PlayerBullet>();
            popBulletScriptComponent.Damage = Damage;
            popBullet.SetActive(true);
            return popBullet;
        }

        public void AddBullet(GameObject bullet)
        {
            bullet.SetActive(false);
            _pool.Push(bullet);
        }

        public int Count()
        {
            return _pool.Count;
        }

        public void PrepareBullets(int i, GameObject _bulletPrefab, Vector3 position)
        {
            for (int j = 0; j < i; j++)
            {
                GameObject bullet = Instantiate(_bulletPrefab, Vector3.zero, _bulletPrefab.transform.rotation);
                bullet.SetActive(false);
                _pool.Push(bullet);
            }
        }
    }

    enum Damage{
        RUNSAGAINSTWALL,
        ENEMY
    }

    enum Debuffs
    {
        NONE = 0,
        Slower = 1,
        LessVisibility = 2,
        ChangedControls = 3,
        NoAttack = 4
    }

    public BulletPool bulletPool = new();


    // player attributes
    private float lives;

    [SerializeField]
    int MAX_LIVES;

    [SerializeField]
    float rotate = 100f;

    [SerializeField]
    float speed = 1;

    bool HasKey = false;


    [SerializeField]
    float forceStrength = 100f;


    // Additional features

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    GameObject hider;


    [SerializeField]
    float bulletSpawnDistance = 2.5f;


    private Debuffs currentDebuff = Debuffs.NONE;



    [SerializeField]
    float shootCooldown = 0.5f;

    [SerializeField]
    float shootSpeed = 1;


    [SerializeField]
    float bulletBaseDamage = 3;

    [SerializeField]
    float playerDamage = 10;

    // funktionert noch nicht!!!!
    [SerializeField]
    float dashStrength = 100f;

    private float dashTimer = -0.2f;



    private Camera cam;
    private float timeSinceLastShoot = 0;
    private Rigidbody2D rigid;

    

    private float isStuck = 0;
    private Vector2 forceVec;

    Dictionary<string, string> stringMap = new Dictionary<string, string>();
    Dictionary<KeyCode, KeyCode> keyCodeMap= new Dictionary<KeyCode, KeyCode>();

    [SerializeField]
    bool invertedControls = false;

    float animationTimer = 0;
    bool animateIn = true;


    bool allowAttack = true;



    void Start()
    {
        cam = Camera.main;
        lives = MAX_LIVES;
        rigid = GetComponent<Rigidbody2D>();

        initMaps();

        bulletPool.PrepareBullets(100, bulletPrefab, new Vector3(0, 0, 0));
    }

    private void initMaps()
    {
        stringMap.Add("Horizontal", "Vertical");
        stringMap.Add("Vertical", "Horizontal");
        keyCodeMap.Add(KeyCode.Space, KeyCode.Return);
        keyCodeMap.Add(KeyCode.Return, KeyCode.Space);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B)) { 
            nextDebuff();
        }

        // hideNonVisibleObjects();
        centerCam();
        centerHider();
        move();
        scale(); // only for tests
        // not yet implemented
        if (currentDebuff != Debuffs.NoAttack)
        {
            shoot();
        }

        
        // showLives();
        // showRoomNumber();
        // ...
    }

    private void nextDebuff()
    {
        currentDebuff += 1;
        switch (currentDebuff)
        {
            case Debuffs.Slower:
                speed = speed * 2 / 3;
                break;
            case Debuffs.LessVisibility:
                animateIn = true;
                animationTimer =  0.1f;
                speed = speed * 3 / 2; 
                break;
            case Debuffs.ChangedControls:
                animateIn = false;
                invertedControls = true;
                break;
            case Debuffs.NoAttack:
                invertedControls = false;
                allowAttack = false;
                break;
            case Debuffs.NONE:
                allowAttack = true;
                break;
        }

    }

    private float getInputAxis(string s)
    { 
        if (!stringMap.ContainsKey(s)){
            return 0;
        }
        if (invertedControls)
        {
            s = stringMap[s];
        }
        return Input.GetAxis(s);
    }

    private bool getInputKey(KeyCode key)
    {
        if (!keyCodeMap.ContainsKey(key))
        {
            return false;
        }
            if (invertedControls)
        {
            key = keyCodeMap[key];
        }
        return Input.GetKeyDown(key);
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("enemy"))
        {
            isStuck = 0.1f;
            forceVec = (transform.position - other.transform.position).normalized * forceStrength;
            rigid.AddForce(forceVec, ForceMode2D.Impulse);
        }

        if (collision.gameObject.name == "Key")
        {
            HasKey = true;

            //teleport Key to nex level
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            // TODO: Distinction between enemy and player bullets
            //Debug.Log("Player hit by bullet");


            // th�s!!
            //  Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            //  damage(Damage.ENEMYNORMAL, bullet.damage);
            //  bullet.GetDamageAndSendToPool();




            // damage(Damage.ENEMYNROMAL);
            // take when merged
            //
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "ExitTrigger")
        {
            Debug.Log("Player has reached the exit.");
            HasKey = false;
            // TODO: Level transition as in: Destroy all enemies, close doors, spawn new enemies, spawn key, etc.
            Destroy(collider.gameObject);
        }
    }

    public float getLives() { return lives; }
    bool damage(Damage type, float dam = 1)
    {
        switch (type)
        {
            case Damage.RUNSAGAINSTWALL:
                lives -= 1;
                break;
            case Damage.ENEMY:
                lives -= bulletBaseDamage+dam;
                break;
            default: return false;
        }
        return true;
    }



    void centerCam()
    {
        if(cam!=null)
            cam.transform.position = transform.position+new Vector3(0,0,-10);
        else
            cam = Camera.main;
    }

    void move()
    {
        // funktionert noch nicht!!!! Kollision mit Gegner!!! -->
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            return;
        }
        else if(dashTimer > -0.1)
        {
            // rigid.totalForce = Vector2.zero;
            rigid.AddForce(-transform.up * dashStrength, ForceMode2D.Impulse);
            dashTimer = -0.1f;
        }
        if (getInputKey(KeyCode.Return))
        {
            rigid.AddForce(transform.up * dashStrength, ForceMode2D.Impulse);
                dashTimer = 0.1f;
            return;
        }
        // <-- funktionert noch nicht!!!!

        if (isStuck > 0)
        {
            isStuck -= Time.deltaTime;
            return;
        }else if(isStuck > -0.1)
        {
            rigid.AddForce(-forceVec, ForceMode2D.Impulse);
            Debug.Log("FORCE ADDED");
            isStuck = -0.1f;
        }
        transform.Rotate(0, 0, Time.deltaTime * -rotate * getInputAxis("Horizontal"));

        float vertical = getInputAxis("Vertical");
        transform.position += transform.up * ((vertical<0)?-0.5f: (vertical > 0) ? 1:0) * Time.deltaTime * speed;
    }

    void shoot()
    {

        if(timeSinceLastShoot < shootCooldown)
        {
            timeSinceLastShoot += Time.deltaTime;
        }else if (getInputKey(KeyCode.Space))
        {
            GameObject bullet = bulletPool.GetBullet(bulletPrefab, transform.position+ transform.up * bulletSpawnDistance, gameObject.transform.rotation, playerDamage);
            PlayerBullet pbscript = bullet.gameObject.GetComponent<PlayerBullet>();
            pbscript.speed = shootSpeed;
            pbscript.player = gameObject;
            timeSinceLastShoot = 0;
        }

    }
    private void scale()
    {
        if (animationTimer > 0)
        {
            float delta = Time.deltaTime;
            animationTimer += animateIn ? delta : -delta ;
        }

        if(animationTimer > 0) { 
            Renderer renderer = hider.GetComponent<Renderer>();
            renderer.material.SetFloat("_animationState", Easing.OutCubic(animationTimer)*4);
            renderer.UpdateGIMaterials();
            DynamicGI.UpdateEnvironment();
            if(animationTimer > 1)
            {
                animationTimer = 1;
            }
        }
    }

    private void centerHider()
    {
        Renderer renderer = hider.GetComponent<Renderer>();


        Vector3 leftTop = hider.transform.position;
        Vector3 center = renderer.bounds.center;
        hider.transform.position = transform.position + (center - leftTop) + new Vector3(0, 0, -0.1f);
    }

   

    
}