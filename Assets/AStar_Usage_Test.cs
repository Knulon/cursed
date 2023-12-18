using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

public class AStar_Usage_Test : MonoBehaviour
{

    private AStarOnNodes astar = new(new Vector2(-10, -10), 1, 2);
    private List<Vector2> path = new();

    private bool breakLoop = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Astar started with " + astar.GetBlockedTilesCount() + " blocked tiles.");
        BuildBlockedTiles();
        Debug.Log("Astar started with " + astar.GetBlockedTilesCount() + " blocked tiles.");
        UpdatePath();
    }

    // Update is called once per frame
    void Update()
    {
        if (breakLoop)
        {
            return;
        }

        UpdatePath();
    }


    void UpdatePath()
    {
        path.Clear();
        Vector2 start = new Vector2(transform.position.x, transform.position.y);
        Vector2 end = new Vector2(4.0f, 4.0f);
        GameObject me = this.gameObject;
        path.AddRange(astar.GetPath(start, end, me));
    }

    private void BuildBlockedTiles()
    {
        astar.BuildBlockedTilesHashMap(3000);
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach (var exploredTile in astar.ExploredTiles)
        {
            Gizmos.DrawCube(astar.GetCoordinateOfNode(exploredTile), new Vector3(astar.CollisionBoxSize, astar.CollisionBoxSize, 0));
        }
        Debug.Log("Explored tiles: " + astar.ExploredTiles.Count);

        //Draw first pathpoint
        Gizmos.color = Color.cyan;
        if (path.Count > 0)
        {
            Gizmos.DrawSphere(path[0], 0.2f);
        }

        Gizmos.color = Color.blue;
        foreach (var pathPoint in path)
        {
            Gizmos.DrawSphere(pathPoint, 0.1f);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector2(4.0f, 4.0f), 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector2(transform.position.x, transform.position.y), 0.1f);


        Gizmos.color = Color.red;
        foreach (var blockedTile in AStarOnNodes.blockedTiles[astar.resolutionFraction])
        {
            //Gizmos.DrawCube(astar.GetCoordinateOfNode(blockedTile), new Vector3(astar.CollisionBoxSize, astar.CollisionBoxSize, 0));
        }
    }
}
