using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class FollowPath : ActionNode
{
    protected override void OnStart()
    {
        Debug.Log("FollowPath started.");
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (context.path.Count == 0)
        {
            return State.Failure; // This should never happen.
        }

        Vector2 direction = context.path[0] - (Vector2)context.gameObject.transform.position;
        if (direction.magnitude < context.enemyInfoManager.GetPathPointReachedRadius())
        {
            context.path.RemoveAt(0);
            Debug.Log("Path point reached. Remaining path points: " + context.path.Count);
        }

        context.enemyController.Direction = direction;
        context.enemyController.Move();

        return State.Success;
    }
}
