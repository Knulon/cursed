using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walkable_test : MonoBehaviour
{
    [SerializeField]
    public GameObject walkable;
    [SerializeField]
    public GameObject blockedoff;

    [SerializeField] public GameObject pathTile;
    [SerializeField] public GameObject goalTile;

    float testXmin = -5.0f;
    float testXmax = 5.0f;
    float testYmin = -5.0f;
    float testYmax = 5.0f;

    float testResolution = 0.5f;

    private List<GameObject> walkableTiles = new List<GameObject>();
    private List<GameObject> blockedTiles = new List<GameObject>();
    private List<GameObject> pathTiles = new List<GameObject>();
    private List<GameObject> goalTiles = new List<GameObject>();

    AStar astar = new AStar();
    private List<Vector2> path = new List<Vector2>();


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

        foreach (var pathTile in pathTiles)
        {
            Destroy(pathTile);
        }

        foreach (var goalTile in goalTiles)
        {
            Destroy(goalTile);
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

        path = astar.getPath(new Vector2(transform.position.x, transform.position.y), new Vector2(4.0f, 4.0f),this.gameObject);

        foreach (var pathPoint in path)
        {
            pathTiles.Add(Instantiate(pathTile, pathPoint, Quaternion.identity));
        }

        goalTiles.Add(Instantiate(goalTile, new Vector2(4.0f, 4.0f), Quaternion.identity));

    }
}
