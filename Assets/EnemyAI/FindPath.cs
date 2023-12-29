using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using TheKiwiCoder;
using Debug = UnityEngine.Debug;

public class FindPath : ActionNode
{
    public List<Vector2> path = new();
    private static Dictionary<int, Task<List<Vector2>>> pathfindingTasks = new();
    private Stopwatch stopwatch = new();

    protected override void OnStart()
    {
        pathfindingTasks.TryAdd(nodeId, null);

        if (pathfindingTasks[nodeId] != null && pathfindingTasks[nodeId].Status == TaskStatus.Running)
        {
            return;
        }

        stopwatch.Reset();
        stopwatch.Start();

        Vector2 start = new Vector2(context.transform.position.x, context.transform.position.y);
        pathfindingTasks[nodeId] = Task.Run(() =>
        {
            List<Vector2> localPath = new List<Vector2>();

            if (context.pathGoal == Vector2.negativeInfinity)
            {
                throw new Exception("Path goal not set.");
            }

            if (context.AStarHandler == null)
            {
                throw new Exception("AStarHandler not set.");
            }

            if (context.gameObject == null)
            {
                throw new Exception("GameObject not set.");
            }

            localPath.AddRange(context.AStarHandler.GetPath(start, context.pathGoal, context.gameObject));
            return localPath;
        });
        context.spriteRenderer.color = Color.red;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (pathfindingTasks[nodeId].IsCompletedSuccessfully)
        {
            path = pathfindingTasks[nodeId].Result;
            context.path = path;
            context.displayPath.Display(path);
            stopwatch.Stop();
            Debug.Log("Pathfinding task completed in " + stopwatch.ElapsedMilliseconds + "ms");
            context.spriteRenderer.color = Color.green;
            return State.Success;
        }

        if (pathfindingTasks[nodeId].Status == TaskStatus.Running)
        {
            return State.Running;
        }

        if (pathfindingTasks[nodeId].IsFaulted)
        {
            Debug.LogError("Pathfinding task failed: " + pathfindingTasks[nodeId].Exception);
            return State.Failure;
        }

        return State.Success;
    }

    void OnGui()
    {
        GUILayout.Label("Pathfinding task completed in " + stopwatch.ElapsedMilliseconds + "ms");
    }

}
