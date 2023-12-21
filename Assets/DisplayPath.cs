using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPath : MonoBehaviour
{
    List<Vector2> path = new();

    public void Display(List<Vector2> path)
    {
        if (path.Count == 0)
        {
            return;
        }
        path.AddRange(path);
    }

    public void OnDrawGizmos()
    {
        //Draw the path
        Gizmos.color = Color.green;
        foreach (var tile in path)
        {
            Gizmos.DrawSphere(tile, 0.1f);
        }
        path.Clear();
    }
}
