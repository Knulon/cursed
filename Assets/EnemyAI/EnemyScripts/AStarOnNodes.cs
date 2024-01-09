using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Unity.Profiling;

public class AStarOnNodes
{

    static readonly ProfilerMarker astarMarker = new ProfilerMarker("AStar");
    static readonly ProfilerMarker heuristicMarker = new ProfilerMarker("Heuristic");
    static readonly ProfilerMarker updatePathMarker = new ProfilerMarker("UpdatePath");

    [SerializeField] public static Dictionary<Pair, HashSet<Pair>> blockedTiles = new();
    public List<Pair> ExploredTiles = new();
    private ContactFilter2D _contactFilter = new();
    private float _resolution;
    public readonly Pair resolutionFraction;
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
        resolutionFraction = new Pair(numerator, denominator);
        _resolution = (float)numerator / denominator;
        Debug.Log("Resolution: " + _resolution);

        CollisionBoxSize = _resolution / 2;

        if (!blockedTiles.ContainsKey(resolutionFraction))
        {
            blockedTiles.Add(resolutionFraction, new HashSet<Pair>());
        }
    }

    public AStarOnNodes(Vector2 gridOffset, int numerator, int denominator, float collisionBoxSize)
    {
        CollisionBoxSize = collisionBoxSize;
        this.gridOffset = gridOffset;
        resolutionFraction = new Pair(numerator, denominator);
        _resolution = (float)numerator / denominator;
        Debug.Log("Resolution: " + _resolution);

        CollisionBoxSize = collisionBoxSize;

        if (!blockedTiles.ContainsKey(resolutionFraction))
        {
            blockedTiles.Add(resolutionFraction, new HashSet<Pair>());
        }
    }


    public class Node
    {
        public Node parent;
        public Pair position;
        public float g;
        public float f;

        public Node(Node parent, Pair position, float g, float h)
        {
            this.parent = parent;
            this.position = position;
            this.g = g;
            this.f = g+h;
        }

        public bool EqualsPosition(Pair other)
        {
            return position.X == other.X && position.Y == other.Y;
        }

    }

    public class Pair {
        public int X;
        public int Y;

        public Pair(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Pair pair &&
                   X == pair.X &&
                   Y == pair.Y;
        }

        // TODO: Combine is pretty slow. Maybe use a different hash function?
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }

    private class MinMaxHeap
    {
        private List<Node> _heap = new();
        private Dictionary<Pair, Node> _nodeDictionary = new();
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
            if (_nodeDictionary.TryGetValue(node.position, out var oldNode))
            {
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
        BuildBlockedTilesHashMap(resolutionFraction, axisSteps);
    }


    public void BuildBlockedTilesHashMap(Pair resolutionFraction, int axisSteps)
    {
        if (blockedTiles[resolutionFraction].Count > 0)
        {
            Debug.Log("Blocked tiles count: " + blockedTiles[resolutionFraction].Count);
            return;
        }
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
                        blockedTiles[resolutionFraction].Add(new Pair(i, j));
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
        astarMarker.Begin();
        //Stopwatch sw = new Stopwatch();
        //sw.Start();
        MinMaxHeap openList = new();
        HashSet<Pair> closedList = new();
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

            if (currentNode.EqualsPosition(GetNearestNode(end)) || DistanceSquared(GetCoordinateOfNode(currentNode), end) < _resolution*_resolution)
            {
                //sw.Stop();
                //Debug.Log("A* finished in " + sw.ElapsedMilliseconds + "ms.");
                astarMarker.End();
                return currentNode;
            }

            if (interruptCounter++ > _maxIterations)
            {
                //sw.Stop();
                //Debug.Log("A* failed in " + sw.ElapsedMilliseconds + "ms.");
                astarMarker.End();
                throw new Exception("A* interrupted. " + _maxIterations + " iterations exceeded.");
            }

            for (int i = 0; i < 8; i++)
            {
                Pair childPosition = new(currentNode.position.X + _directions[i, 0], currentNode.position.Y + _directions[i, 1]);
                if (closedList.Contains(childPosition) || blockedTiles[resolutionFraction].Contains(childPosition))
                {
                    continue;
                }

                // TODO: This should use a pool of nodes to avoid GC.
                // Changing the + 1 to directionPrice[i] will make the algorithm prefer diagonal movement. Unfortunately it has a decent performance impact.
                Node childNode = new(currentNode, childPosition, currentNode.g + 1,
                    GetHeuristic(GetCoordinateOfNode(childPosition), end));
                heuristicMarker.End();

                updatePathMarker.Begin();
                if (openList.UpdateNode(childNode) == -1)
                {
                    openList.Add(childNode);
                }
                updatePathMarker.End();
            }
        }
        //sw.Stop();
        //Debug.Log("A* path not found in " + sw.ElapsedMilliseconds + "ms.");
        astarMarker.End();
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
        return GetCoordinateOfNode(node.position.X, node.position.Y);
    }

    private Vector2 GetCoordinateOfNode(int x, int y)
    {
        return new Vector2(x * _resolution + gridOffset.x, y * _resolution + gridOffset.y);
    }

    public Vector2 GetCoordinateOfNode(Pair node)
    {
        return GetCoordinateOfNode(node.X, node.Y);
    }

    private Pair GetNearestNode(Vector2 position)
    {
        if (position.x < gridOffset.x || position.y < gridOffset.y)
        {
            return new Pair(-1, -1);
        }

        var x = RoundToNearest((position.x - gridOffset.x) / _resolution);
        var y = RoundToNearest((position.y - gridOffset.y) / _resolution);
        return new Pair(x, y);
    }

    private int RoundToNearest(float value)
    {
        if (value - (int)value >= 0.5f)
        {
            return (int)value + 1;
        }
        return (int)value;
    }

    private float GetHeuristic(Vector2 start, Vector2 end)
    {
        heuristicMarker.Begin();
        float dx = Mathf.Abs(start.x - end.x);
        float dy = Mathf.Abs(start.y - end.y);
        return (dx + dy) + (-0.6f * Mathf.Min(dx, dy));
        // return 1 * (dx + dy) + (1.41f - 2 * 1) * Mathf.Min(dx, dy);
    }

    private float DistanceSquared(Vector2 start, Vector2 end)
    {
        float dx = Mathf.Abs(start.x - end.x);
        float dy = Mathf.Abs(start.y - end.y);
        return dx * dx + dy * dy;
    }
}