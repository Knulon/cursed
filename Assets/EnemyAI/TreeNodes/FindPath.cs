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
    public int TaskId;
    private Stopwatch stopwatch = new();
    private Dictionary<int, Task<List<Vector2>>> _pathfindingTasks;


    protected override void OnStart()
    {
        _pathfindingTasks = context.PathfindingTasks;
        _pathfindingTasks.TryAdd(TaskId, null);

        if (_pathfindingTasks[TaskId] != null && _pathfindingTasks[TaskId].Status == TaskStatus.Running)
        {
            return;
        }

        stopwatch.Reset();
        stopwatch.Start();

        Vector2 start = new Vector2(context.transform.position.x, context.transform.position.y);
        _pathfindingTasks[TaskId] = Task.Run(() =>
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
        //context.spriteRenderer.color = Color.red;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (_pathfindingTasks[TaskId].IsCompletedSuccessfully)
        {
            path = _pathfindingTasks[TaskId].Result;
            context.path = path;
            context.displayPath.Display(path);
            stopwatch.Stop();
            Debug.Log("Pathfinding task completed in " + stopwatch.ElapsedMilliseconds + "ms");
            //context.spriteRenderer.color = Color.green;
            return State.Success;
        }

        if (_pathfindingTasks[TaskId].Status == TaskStatus.Running)
        {
            return State.Running;
        }

        if (_pathfindingTasks[TaskId].IsFaulted)
        {
            Debug.LogError("Pathfinding task failed: " + _pathfindingTasks[TaskId].Exception);
            return State.Failure;
        }

        return State.Success;
    }

    void OnGui()
    {
        GUILayout.Label("Pathfinding task completed in " + stopwatch.ElapsedMilliseconds + "ms");
    }
}
