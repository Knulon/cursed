using UnityEngine;

public class BulletPool : MonoBehaviour 
{
    public GameObject bulletsPrefab;
    private GameObject[] bullets;
    private int activeObjects = 0;

    BulletPool(GameObject bulletType, int startBullets)
    {
        bulletsPrefab = bulletType;
        this.activeObjects = 0;
        this.bullets = new GameObject[startBullets];
        for (int i = 0; i < this.bullets.Length; i++)
        {
            bullets[i] = Instantiate(bulletsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            bullets[i].SetActive(false);
        }
    }

    void doubleBullets()
    {
        GameObject[] gameObjects = new GameObject[bullets.Length * 2];
        for (int i = 0; i < bullets.Length; i++)
        {
            gameObjects[i] = bullets[i];
        }
        for (int i = bullets.Length; i < gameObjects.Length; i++)
        {
            gameObjects[i] = Instantiate(bulletsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            bullets[i].SetActive(false);
        }
        bullets = gameObjects;
    }

    GameObject getBullet()
    {
        if (activeObjects >= bullets.Length)
            doubleBullets();
        GameObject bullet = bullets[activeObjects++];
        bullet.SetActive(true);
        return bullet;
    }

    void swap(int idx1, int idx2)
    {
        GameObject temp = bullets[idx1];
        bullets[idx1] = bullets[idx2];
        bullets[idx2] = temp;
    }

    void deactivate(GameObject obj)
    {
        int idxSwap = System.Array.IndexOf(bullets, obj);
        swap(idxSwap, activeObjects--);
        obj.SetActive(false);

    }

}
