using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AStarOnNodes
{

    [SerializeField] public static Dictionary<Pair<int, int>, HashSet<Pair<int, int>>> blockedTiles = new();
    public List<Pair<int, int>> ExploredTiles = new();
    private ContactFilter2D _contactFilter = new();
    private float _resolution;
    public readonly int ResolutionIndex;
    public readonly Pair<int, int> resolutionFraction;
    Vector2 gridOffset;
    public float CollisionBoxSize;
    private static readonly int[,] _directions =
    {
        {0, 1}, {0, -1}, {-1, 0}, {1, 0}, {1, 1}, {-1, 1}, {-1, -1}, {1, -1}
    };
    private static float[] directionPrice = { 1, 1, 1, 1, 1.4f, 1.4f, 1.4f, 1.4f };

    private float _maxIterations = 15000;
    private bool _breakLoop = false;

    public AStarOnNodes(Vector2 gridOffset, int numerator, int denominator)
    {
        this.gridOffset = gridOffset;
        resolutionFraction = new Pair<int, int>(numerator, denominator);
        _resolution = (float)numerator / denominator;
        Debug.Log("Resolution: " + _resolution);

        CollisionBoxSize = _resolution / 2;

        if (!blockedTiles.ContainsKey(resolutionFraction))
        {
            blockedTiles.Add(resolutionFraction, new HashSet<Pair<int, int>>());
        }
    }

    public AStarOnNodes(Vector2 gridOffset, int numerator, int denominator, float collisionBoxSize)
    {
        CollisionBoxSize = collisionBoxSize;
        this.gridOffset = gridOffset;
        resolutionFraction = new Pair<int, int>(numerator, denominator);
        _resolution = (float)numerator / denominator;
        Debug.Log("Resolution: " + _resolution);

        CollisionBoxSize = collisionBoxSize;

        if (!blockedTiles.ContainsKey(resolutionFraction))
        {
            blockedTiles.Add(resolutionFraction, new HashSet<Pair<int, int>>());
        }
    }


    public class Node
    {
        public Node parent;
        public Pair<int, int> position;
        public float g;
        public float f;

        public Node(Node parent, Pair<int, int> position, float g, float h)
        {
            this.parent = parent;
            this.position = position;
            this.g = g;
            this.f = g+h;
        }

        public bool EqualsPosition(Pair<int, int> other)
        {
            return position.x == other.x && position.y == other.y;
        }

    }

    public class Pair<T, N>
    {
        public T x;
        public N y;

        public Pair(T x, N y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Pair<T, N> pair &&
                   EqualityComparer<T>.Default.Equals(x, pair.x) &&
                   EqualityComparer<N>.Default.Equals(y, pair.y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }

    private class MinMaxHeap
    {
        private List<Node> _heap = new();
        private Dictionary<Pair<int, int>, Node> _nodeDictionary = new();
        public int Count => _heap.Count;

        public void Add(Node node)
        {
            _heap.Add(node);
            _nodeDictionary.Add(node.position, node);
            HeapifyUp(_heap.Count - 1);
        }

        public Node Peek()
        {
            return _heap[0];
        }

        public Node Pop()
        {
            Node node = _heap[0];
            Swap(0, _heap.Count - 1);
            _heap.RemoveAt(_heap.Count - 1);
            _nodeDictionary.Remove(node.position);
            HeapifyDown(0);
            return node;
        }

        public int UpdateNode(Node node) // -1 if node not found, 0 if node found and not updated, 1 if node found and updated
        {
            if (_nodeDictionary.ContainsKey(node.position))
            {
                Node oldNode = _nodeDictionary[node.position];
                if (oldNode.f > node.f)
                {
                    _nodeDictionary[node.position] = node;
                    int index = _heap.IndexOf(oldNode);
                    _heap[index] = node;
                    HeapifyUp(index);
                    return 1;
                }
                return 0;
            }
            return -1;
        }

        private void HeapifyUp(int index)
        {
            if (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (_heap[parentIndex].f > _heap[index].f)
                {
                    Swap(parentIndex, index);
                    HeapifyUp(parentIndex);
                }
            }
        }

        private void HeapifyDown(int index)
        {
            int leftChildIndex = index * 2 + 1;
            int rightChildIndex = index * 2 + 2;
            int minIndex = index;
            if (leftChildIndex < _heap.Count)
            {
                if (_heap[leftChildIndex].f < _heap[minIndex].f)
                {
                    minIndex = leftChildIndex;
                }
            }

            if (rightChildIndex < _heap.Count)
            {
                if (_heap[rightChildIndex].f < _heap[minIndex].f)
                {
                    minIndex = rightChildIndex;
                }
            }

            if (minIndex != index)
            {
                Swap(minIndex, index);
                HeapifyDown(minIndex);
            }
        }
        private void Swap(int index1, int index2)
        {
            (_heap[index1], _heap[index2]) = (_heap[index2], _heap[index1]);
        }
    }

    public int GetBlockedTilesCount()
    {
        return blockedTiles[resolutionFraction].Count;
    }

    public void BuildBlockedTilesHashMap(int axisSteps)
    {
        BuildBlockedTilesHashMap(ResolutionIndex, axisSteps);
    }


    public void BuildBlockedTilesHashMap(int ResolutionIndexParam, int axisSteps)
    {
        blockedTiles[resolutionFraction].Clear();
        List<Collider2D> overlapColliders = new List<Collider2D>();
        for (int i = 0; i < axisSteps; i++)
        {
            for (int j = 0; j < axisSteps; j++)
            {
                overlapColliders.Clear();
                Vector2 pos = new Vector2(i * _resolution + gridOffset.x, j * _resolution + gridOffset.y);
                Physics2D.OverlapBox(pos, new Vector2(CollisionBoxSize, CollisionBoxSize), 0, _contactFilter,
                    overlapColliders);
                foreach (var collider2D in overlapColliders)
                {
                    if (collider2D.gameObject.tag.Contains("Obstacle"))
                    {
                        blockedTiles[resolutionFraction].Add(new Pair<int, int>(i, j));
                    }
                }
            }
        }
    }

    Node VetAstarParamsAndAstar(Vector2 start, Vector2 end, GameObject me)
    {
        if (GetNearestNode(start).Equals(GetNearestNode(end)))
        {
            Debug.LogError("Start and end positions are the same.");
            return null;
        }

        if (blockedTiles[resolutionFraction].Contains(GetNearestNode(end)))
        {
            Debug.LogError("End position is not walkable.");
            return null;
        }

        if (blockedTiles[resolutionFraction].Contains(GetNearestNode(start)))
        {
            Debug.LogError("Start position is not walkable");
            return null;
        }

        return AstarSearch(start, end, me);
    }

    Node AstarSearch(Vector2 start, Vector2 end, GameObject me)
    {
        MinMaxHeap openList = new();
        HashSet<Pair<int, int>> closedList = new();
        //ExploredTiles.Clear();
        int interruptCounter = 0;

        Node startNode = new Node(null, GetNearestNode(start), 0,  0);
        openList.Add(startNode);

        while (openList.Count > 0 && !_breakLoop)
        {
            Node currentNode = openList.Pop();
            closedList.Add(currentNode.position);

            
            //if (!ExploredTiles.Contains(currentNode.position))
            //{
            //    ExploredTiles.Add(currentNode.position);
            //}
            

            if (currentNode.EqualsPosition(GetNearestNode(end)) || Vector2.Distance(GetCoordinateOfNode(currentNode), end) < _resolution)
            {
                return currentNode;
            }

            if (interruptCounter++ > _maxIterations)
            {
                throw new Exception("A* interrupted. " + _maxIterations + " iterations exceeded.");
            }


            for (int i = 0; i < 8; i++)
            {
                Pair<int, int> childPosition = new(currentNode.position.x + _directions[i, 0], currentNode.position.y + _directions[i, 1]);

                // Changing the + 1 to directionPrice[i] will make the algorithm prefer diagonal movement. Unfortunately it has a decent performance impact.
                Node childNode = new Node(currentNode, childPosition, currentNode.g + 1,
                    GetHeuristic(GetCoordinateOfNode(childPosition), end));

                bool isBlocked = blockedTiles[resolutionFraction].Contains(childPosition);
                if (closedList.Contains(childNode.position) || isBlocked)
                {
                    continue;
                }

                if (openList.UpdateNode(childNode) == -1)
                {
                    openList.Add(childNode);
                }
            }
        }
        return null;
    }

    public Stack<Vector2> GetPath(Vector2 start, Vector2 end, GameObject me)
    {
        Stack<Vector2> path = new Stack<Vector2>();
        Node endNode = VetAstarParamsAndAstar(start, end, me);
        if (endNode == null)
        {
            return path;
        }

        Node currentNode = endNode;
        while (currentNode != null)
        {
            path.Push(GetCoordinateOfNode(currentNode.position));
            currentNode = currentNode.parent;
        }

        return path;
    }

    private Vector2 GetCoordinateOfNode(Node node)
    {
        return GetCoordinateOfNode(node.position.x, node.position.y);
    }

    private Vector2 GetCoordinateOfNode(int x, int y)
    {
        return new Vector2(x * _resolution + gridOffset.x, y * _resolution + gridOffset.y);
    }

    public Vector2 GetCoordinateOfNode(Pair<int, int> node)
    {
        return GetCoordinateOfNode(node.x, node.y);
    }

    private Pair<int, int> GetNearestNode(Vector2 position)
    {
        if (position.x < gridOffset.x || position.y < gridOffset.y)
        {
            return new Pair<int, int>(-1, -1);
        }

        var x = RoundToNearest((position.x - gridOffset.x) / _resolution);
        var y = RoundToNearest((position.y - gridOffset.y) / _resolution);
        return new Pair<int, int>(x, y);
    }

    private int RoundToNearest(float value)
    {
        int rounded = (int)value;
        value -= rounded;
        if (value >= 0.5f)
        {
            return rounded + 1;
        }
        return rounded;
    }

    private float GetHeuristic(Vector2 start, Vector2 end)
    {
        float dx = Mathf.Abs(start.x - end.x);
        float dy = Mathf.Abs(start.y - end.y);
        return (dx + dy) + (-0.6f * Mathf.Min(dx, dy));
        // return 1 * (dx + dy) + (1.41f - 2 * 1) * Mathf.Min(dx, dy);
    }
}