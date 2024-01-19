using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Random = UnityEngine.Random;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Unity.Mathematics;
using UnityEngine.UI;

public class PlayerMoveScript : MonoBehaviour
{
    enum Damage
    {
        RUNSAGAINSTWALL,
        ENEMY
    }

    enum Debuffs
    {
        NONE = 0,
        DRUNK = 1,
        LessVisibility = 2,
        ChangedControls = 3,
        NoAttack = 4,
        STRONGDRUNK = 5
    }

    public BulletPool bulletPool;


    // player attributes
    private float lives;

    [SerializeField]
    int MAX_LIVES = 100;

    [SerializeField]
    float rotate = 100f;

    [SerializeField]
    float speed = 1;

    bool hasKey = false;

    // Additional features
    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    GameObject hider;

    [SerializeField]
    float bulletSpawnDistance = 2.5f;


    private Debuffs currentDebuff = Debuffs.NONE;

    [SerializeField]
    float shootCooldown = 0.1f;

    [SerializeField]
    float bulletSpeed = 2;

    [SerializeField]
    float playerDamage = 25;

    [SerializeField]
    GameObject lifes;

    [SerializeField]
    GameObject level;

    [SerializeField]
    GameObject key;

    [SerializeField]
    GameObject winscreen;

    private bool isDrunk = false;
    private float drunkTimer = 0;

    private float shakeTime = 0.001f;

    private Camera cam;
    private float timeSinceLastShoot = 0;
    private Rigidbody2D rigid;

    Dictionary<string, string> stringMap = new Dictionary<string, string>();
    Dictionary<KeyCode, KeyCode> keyCodeMap = new Dictionary<KeyCode, KeyCode>();

    [SerializeField]
    bool invertedControls = false;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private GameObject deathScreen;

    float animationTimer = 0;
    bool animateIn = true;

    private int LEVEL = 0;
    private int MAX_LEVEL = 3;

    int lastLvlKeyCount = 0;

    [SerializeField] GameObject pauseMenu;

    void Awake()
    {
        bulletPool = BulletPool.GetInstance(bulletPrefab);
    }

    void Start()
    {
        setSound();
        updatePlayerPrefs();

        if (bulletPool != null)
        {
            Debug.Log("BULLET POOOOOOL");
        }

        gameManager.nextLevel(1);

        cam = Camera.main;
        lives = MAX_LIVES;
        rigid = GetComponent<Rigidbody2D>();

        initMaps();
    }
    [SerializeField]
    Slider soundSlider;

    [SerializeField]
    Settings settings;

    private void setSound()
    {
        soundSlider.value = PlayerPrefs.GetFloat("_Volume", 1);
        settings.changeVolume();
    }

    public void regenerate()
    {
        lives = math.clamp(lives + 20, 0, MAX_LIVES);
    }

    public void updatePlayerPrefs()
    {
        // sets the rotation speed to the value edited in the settings
        // slider from 0.5 to 3 (default value 1)
        float playerPref = PlayerPrefs.GetFloat("_TurningSensitivity", 1);
        rotate = 100 * playerPref;
        Debug.Log("rotation speed updated to " + playerPref);
    }

    private void initMaps()
    {
        stringMap.Add("Horizontal", "Vertical");
        stringMap.Add("Vertical", "Horizontal");
        keyCodeMap.Add(KeyCode.Space, KeyCode.Return);
        keyCodeMap.Add(KeyCode.Return, KeyCode.Space);
    }
    
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO remove
        if (Input.GetKeyDown(KeyCode.B))
        {
            nextDebuff();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && lives > 0)
        {
            if (pauseMenu.activeSelf)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        centerCam();
        move();
        scale();

        if (currentDebuff != Debuffs.NoAttack)
        {
            shoot();
        }

        showPlayerData();
        if (isDrunk)
        {
            drunkTimer += Time.deltaTime;
            Debug.Log("UPDATE DDRUNK");
        }

    }

    private void showPlayerData()
    {
        lifes.GetComponent<TextMeshProUGUI>().text = "Leben: " + lives;
        level.GetComponent<TextMeshProUGUI>().text = "Level " + ((LEVEL + 1) >= MAX_LEVEL ? MAX_LEVEL : (LEVEL + 1)) + " von " + MAX_LEVEL;
        if (lastLvlKeyCount > 0)
        {
            key.GetComponent<TextMeshProUGUI>().text = lastLvlKeyCount > 0 ? "eingesammelte Schlüssel: " + lastLvlKeyCount : "";
        }
        else
        {
            key.GetComponent<TextMeshProUGUI>().text = hasKey ? "Schlüssel eingesammelt" : "";
        }
    }

    private void nextDebuff()
    {
        currentDebuff += 1;
        switch (currentDebuff)
        {
            case Debuffs.DRUNK:
                Debug.Log("DRUNK activated");
                isDrunk = true;
                break;
            case Debuffs.LessVisibility:
                animateIn = true;
                animationTimer = 0.1f;
                isDrunk = false;
                break;
            case Debuffs.ChangedControls:
                animateIn = false;
                invertedControls = true;
                break;
            case Debuffs.NoAttack:
                // invertedControls = false;
                break;
            case Debuffs.STRONGDRUNK:
                drunkTimer = 0.0001f;
                break;
            case Debuffs.NONE:
                break;
        }

    }

    private float getInputAxis(string s)
    {
        if (!stringMap.ContainsKey(s))
        {
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
        return Input.GetKey(key);
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (collision.gameObject.name == "Key")
        {
            hasKey = true;
            if (LEVEL >= MAX_LEVEL)
            {
                lastLvlKeyCount++;
            }
            //teleport Key to next level
            teleportToLevel(collision.gameObject, LEVEL + 1);
        }

        if (collision.gameObject.name.StartsWith("TELEPORTER"))
        {
            if (LEVEL >= MAX_LEVEL && lastLvlKeyCount == 3)
            {
                Debug.Log("WIN!!!!!");
                winscreen.SetActive(true);
                Time.timeScale = 0.0f;
            }
            else
            {
                Debug.Log("TELEPORT");
                GameObject[] spawner = GameObject.FindGameObjectsWithTag("SPAWN");
                foreach (var spawn in spawner)
                {
                    if (spawn.name.EndsWith("" + (LEVEL + 1)))
                    {
                        transform.position = spawn.transform.position;
                        nextDebuff();
                        LEVEL++;
                        hasKey = false;

                        gameManager.nextLevel(LEVEL+1);

                        break;
                    }
                }
            }

        }

        if (collision.gameObject.name.StartsWith("TELEPORTER_LASTLVL"))
        {
            Debug.Log("TELEPORT_LASTLVL");
            collision.gameObject.SetActive(false);
            LEVEL++;
            nextDebuff();
            hasKey = false;
        }


        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            if (collision.gameObject.layer.Equals(12))
            {
                Debug.Log("Player hit by bullet");

                Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                damage(Damage.ENEMY, bullet.GetDamageAndSendToPool());
            }
        }
    }

    private void teleportToLevel(GameObject obj, int v)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("keypos");
        foreach (GameObject n in gameObjects)
        {
            if (n.name.EndsWith("" + v))
            {
                n.SetActive(false);
                obj.transform.position = n.transform.position;
            }
        }
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public float getLives() { return lives; }
    bool damage(Damage type, float dam = 1)
    {
        switch (type)
        {
            case Damage.RUNSAGAINSTWALL:
                lives -= dam;
                break;
            case Damage.ENEMY:
                lives -= dam;
                break;
            default: return false;
        }

        if (lives <= 0)
        {
            deathScreen.SetActive(true);
            Time.timeScale = 0;
        }
        return true;
    }



    private void centerCam()
    {
        if (cam != null)
            cam.transform.position = transform.position + new Vector3(0, 0, -10);
        else
            cam = Camera.main;
    }

    void move()
    {
        transform.Rotate(0, 0, Time.deltaTime * -rotate * getInputAxis("Horizontal"));

        float vertical = getInputAxis("Vertical");

        Vector3 rotatedMoveDirection = transform.up;
        if (isDrunk && drunkTimer > shakeTime)
        {
            drunkTimer = 0;
            float randomAngle = Random.Range(-100, 100);

            rotatedMoveDirection.x = transform.up.x * Mathf.Cos(randomAngle * Mathf.Deg2Rad) - transform.up.y * Mathf.Sin(randomAngle * Mathf.Deg2Rad);
            rotatedMoveDirection.y = transform.up.x * Mathf.Sin(randomAngle * Mathf.Deg2Rad) + transform.up.y * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
            rotatedMoveDirection.Normalize();
            rotatedMoveDirection *= 3f;
        }
        transform.position += rotatedMoveDirection * ((vertical < 0) ? -0.5f : (vertical > 0) ? 1 : 0) * Time.deltaTime * speed;
    }

    void shoot()
    {

        if (timeSinceLastShoot < shootCooldown)
        {
            timeSinceLastShoot += Time.deltaTime;
        }
        else if (getInputKey(KeyCode.Space))
        {
            // TODO TOBI: what layer mask??
            GameObject bullet = bulletPool.GetBullet(bulletPrefab, transform.position + transform.up * bulletSpawnDistance, playerDamage, null, bulletLayer: 11);
            if (bullet != null)
            {
                PlayerBullet pbscript = bullet.gameObject.GetComponent<PlayerBullet>();
                Vector2 dir = transform.up;
                Vector2 rotatedShootDirection = dir;
                rotatedShootDirection.x = dir.x * Mathf.Cos(-90 * Mathf.Deg2Rad) - dir.y * Mathf.Sin(-90 * Mathf.Deg2Rad);
                rotatedShootDirection.y = dir.x * Mathf.Sin(-90 * Mathf.Deg2Rad) + dir.y * Mathf.Cos(-90 * Mathf.Deg2Rad);
                rotatedShootDirection.Normalize();

                float angle = Mathf.Atan2(rotatedShootDirection.y, rotatedShootDirection.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                bullet.GetComponent<Rigidbody2D>().velocity = (dir * bulletSpeed);

                timeSinceLastShoot = 0;
            }
        }

    }
    private void scale()
    {
        if (animationTimer > 0)
        {
            float delta = Time.deltaTime;
            animationTimer += animateIn ? delta : -delta;
        }

        if (animationTimer > 0)
        {
            Renderer renderer = hider.GetComponent<Renderer>();
            renderer.material.SetFloat("_animationState", Easing.OutCubic(animationTimer) * 4);
            renderer.UpdateGIMaterials();
            DynamicGI.UpdateEnvironment();
            if (animationTimer > 1)
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