using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using TheKiwiCoder;
using TMPro;
using Debug = UnityEngine.Debug;

public class FindPath : ActionNode
{
    public List<Vector2> path = new();
    private static Dictionary<int, Task<List<Vector2>>> pathfindingTasks = new();
    private Vector2 goal;

    private Stopwatch stopwatch = new();
    public TMP_Text TextField;

    protected override void OnStart()
    {
        TextField = GameObject.Find("Text (TMP)").GetComponent<TMP_Text>();

        Vector2 start = new Vector2(context.transform.position.x, context.transform.position.y);
        goal = new Vector2(4.0f, 4.0f);

        if (pathfindingTasks[nodeId] != null && pathfindingTasks[nodeId].Status == TaskStatus.Running)
        {
            return;
        }

        stopwatch.Reset();
        stopwatch.Start();
        pathfindingTasks[nodeId] = Task.Run(() =>
        {
            List<Vector2> localPath = new List<Vector2>();
            localPath.AddRange(context.AStarHandler.GetPath(start, goal, context.gameObject));
            return localPath;
        });
        context.spriteRenderer.color = Color.red;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (pathfindingTasks[nodeId].IsCompleted)
        {
            path = pathfindingTasks[nodeId].Result;
            context.displayPath.Display(path);
            stopwatch.Stop();
            Debug.Log("Pathfinding task completed in " + stopwatch.ElapsedMilliseconds + "ms");
            if (TextField != null)
            {
                TextField.text = "Pathfinding task completed in " + stopwatch.ElapsedMilliseconds + "ms";
            }
            context.spriteRenderer.color = Color.green;
            return State.Success;
        }

        if (pathfindingTasks[nodeId].Status == TaskStatus.Running)
        {
            return State.Running;
        }

        if (pathfindingTasks[nodeId].IsFaulted)
        {
            return State.Failure;
        }

        return State.Success;
    }

    void OnGui()
    {
        GUILayout.Label("Pathfinding task completed in " + stopwatch.ElapsedMilliseconds + "ms");
    }

}
