using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar
{

    class Node
    {
        public Node parent;
        public Vector2 position;
        public float g;
        public float h;
        public float f;

        public Node(Node parent, Vector2 position, float g, float h, float f)
        {
            this.parent = parent;
            this.position = position;
            this.g = g;
            this.h = h;
            this.f = g + h;
        }
        public Node(Node parent, Vector2 position, float g, float h) : this(parent, position, g, h, g + h) { }

    }

    private ContactFilter2D contactFilter = new ContactFilter2D();


    // A* variables
    private SortedList<float, Node> openList = new();
    private List<Node> closedList = new();




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    Node vetAstarParamsAndAstar(Vector2 start, Vector2 end, GameObject me)
    {
        if (start == end)
        {
            Debug.LogError("Start and end positions are the same.");
            return null;
        }
        List<Collider2D> overlapStartCollider = new List<Collider2D>();
        List<Collider2D> overlapEndCollider = new List<Collider2D>();

        Physics2D.OverlapBox(end, new Vector2(.5f, .5f), 0, contactFilter, overlapEndCollider);
        foreach (var collider in overlapEndCollider)
        {
            if (collider && collider.gameObject != me)
            {
                Debug.LogError("End position is not walkable.");
                return null;
            }
        }

        Physics2D.OverlapBox(start, new Vector2(.5f, .5f), 0, contactFilter, overlapStartCollider);
        foreach (var collider in overlapStartCollider)
        {
            if (collider && collider.gameObject != me)
            {
                Debug.LogError("Start position is not walkable");
                return null;
            }
        }

        return astarSearch(start, end);
    }

    Node astarSearch(Vector2 start, Vector2 end)
    {
        Node startNode = new Node(null, start, 0, Vector2.Distance(start, end), 0);

        openList.Add(startNode.f, startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.Values[0];
            openList.RemoveAt(0);
            closedList.Add(currentNode);

            if (currentNode.position == end)
            {
                return currentNode;
            }

            List<Node> children = new List<Node>();
            foreach (var direction in new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right })
            {
                Vector2 childPosition = currentNode.position + direction;
                Node childNode = new Node(currentNode, childPosition, currentNode.g + 1, Vector2.Distance(childPosition, end));
                children.Add(childNode);
            }

            foreach (var child in children)
            {
                if (closedList.Contains(child))
                {
                    continue;
                }

                if (openList.ContainsKey(child.f))
                {
                    if (child.g < openList[child.f].g)
                    {
                        openList.Remove(child.f);
                        openList.Add(child.f, child);
                    }
                }
                else
                {
                    openList.Add(child.f, child);
                }
            }
        }

        return null;
    }

    public List<Vector2> getPath(Vector2 start, Vector2 end, GameObject me)
    {
        List<Vector2> path = new List<Vector2>();
        Node endNode = vetAstarParamsAndAstar(start, end, me);
        if (endNode == null)
        {
            return path;
        }

        Node currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
}
