using System.Collections.Generic;
using UnityEngine;

public class AStar_Handler : MonoBehaviour
{
    [SerializeField]
    private int Numerator = 0;
    [SerializeField]
    private int Denominator = 0;
    [SerializeField]
    private Vector2 _gridOffset = new(0, 0);
    [Header("Collision Box Size is usually _resolution / 2")]
    [SerializeField]
    private float _collisionBoxSize = 0.5f;

    private AStarOnNodes _astar;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Init Astar with " + Numerator + "/" + Denominator +  " grid");
        _astar = new AStarOnNodes(_gridOffset,Numerator, Denominator, _collisionBoxSize);
        _astar.BuildBlockedTilesHashMap(new(Numerator,Denominator),3000);
    }

    public Stack<Vector2> GetPath(Vector2 start, Vector2 end, GameObject me)
    {
        return _astar.GetPath(start, end, me);
    }

    public Vector2 GetGridOffset()
    {
        return _gridOffset;
    }

    public int GetNumerator()
    {
        return Numerator;
    }

    public int GetDenominator()
    {
        return Denominator;
    }

    public float GetCollisionBoxSize()
    {
        return _collisionBoxSize;
    }
}
