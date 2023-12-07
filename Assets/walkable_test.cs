using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walkable_test : MonoBehaviour
{
    [SerializeField]
    public GameObject walkable;
    [SerializeField]
    public GameObject blockedoff;

    float testXmin = -5.0f;
    float testXmax = 5.0f;
    float testYmin = -5.0f;
    float testYmax = 5.0f;

    float testResolution = 0.5f;

    private List<GameObject> walkableTiles = new List<GameObject>();
    private List<GameObject> blockedTiles = new List<GameObject>();

    private List<Collider2D> overlapColliders = new List<Collider2D>();
    private ContactFilter2D contactFilter = new ContactFilter2D();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (var blockedTile in blockedTiles)
        {
            Destroy(blockedTile);
        }

        foreach (var walkableTile in walkableTiles)
        {
            Destroy(walkableTile);
        }


        for (float i = testXmin; i < testXmax; i += testResolution)
        {
            for (float j = testYmin; j < testYmax; j += testResolution)
            {
                Vector2 testPoint = new Vector2(i, j);
                testPoint += new Vector2(transform.position.x, transform.position.y);
                Physics2D.OverlapBox(testPoint, new Vector2(.5f, .5f), 0, contactFilter, overlapColliders);
                bool isWalkable = false;
                foreach (var collider in overlapColliders)
                {
                    if (collider && collider.gameObject != this.gameObject)
                    {
                        isWalkable = true;
                    }
                }
                if (isWalkable)
                {
                    walkableTiles.Add(Instantiate(blockedoff, testPoint, Quaternion.identity));
                }
                else
                {
                    blockedTiles.Add(Instantiate(walkable, testPoint, Quaternion.identity));
                }
            }
        }
    }
}
