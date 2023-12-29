using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Diagnostics;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

public class FinishPathfinding : ActionNode
{
    public List<Vector2> path = new();
    public int TaskId;
    private Dictionary<int, Task<List<Vector2>>> pathfindingTasks;

    protected override void OnStart()
    {
        pathfindingTasks = FindPath.GetPathfindingTasks();

    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (!pathfindingTasks.ContainsKey(TaskId))
        {
            Debug.LogError("TaskId not found: " + TaskId + ". Make sure to have the same TaskId as the corresponding FindTask-Node. And this node needs a succeed decorator :D");
            return State.Failure;
        }

        if (pathfindingTasks[TaskId].IsCompletedSuccessfully)
        {
            path = pathfindingTasks[TaskId].Result;
            context.path = path;
            context.displayPath.Display(path);
            context.spriteRenderer.color = Color.green;
            return State.Success;
        }

        if (pathfindingTasks[TaskId].Status == TaskStatus.Running)
        {
            return State.Running;
        }

        if (pathfindingTasks[TaskId].IsFaulted)
        {
            Debug.LogError("Pathfinding task failed: " + pathfindingTasks[TaskId].Exception);
            return State.Failure;
        }
        return State.Success;
    }
}
