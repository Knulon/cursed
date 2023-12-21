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
    private List<Vector2> path = new();
    private static Task<List<Vector2>> pathfindingTask;
    private Vector2 goal;

    private Stopwatch stopwatch = new Stopwatch();
    public TMP_Text TextField;

    protected override void OnStart()
    {
        TextField = GameObject.Find("Text (TMP)").GetComponent<TMP_Text>();

        Vector2 start = new Vector2(context.transform.position.x, context.transform.position.y);
        goal = new Vector2(4.0f, 4.0f);

        if (pathfindingTask != null && pathfindingTask.Status == TaskStatus.Running)
        {
            return;
        }

        stopwatch.Reset();
        stopwatch.Start();
        pathfindingTask = Task.Run(() =>
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
        if (pathfindingTask.IsCompleted)
        {
            path = pathfindingTask.Result;
            //context.displayPath.Display(path);
            stopwatch.Stop();
            Console.WriteLine("Pathfinding task completed in " + stopwatch.ElapsedMilliseconds + "ms");
            if (TextField != null)
            {
                TextField.text = "Pathfinding task completed in " + stopwatch.ElapsedMilliseconds + "ms";
            }
            context.spriteRenderer.color = Color.green;
            return State.Success;
        }

        if (pathfindingTask.Status == TaskStatus.Running)
        {
            return State.Running;
        }

        if (pathfindingTask.IsFaulted)
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
