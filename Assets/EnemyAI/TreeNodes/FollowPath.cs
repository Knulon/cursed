using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class FollowPath : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        // TODO: This method could be optimized

        if (context.path.Count == 0)
        {
            //Debug.LogError("Path is empty."); // TODO: This can happen if the player or enemy is in a tile marked as unwalkable because of a low resolution in the A* grid. Fix this. Example: Unstuck the enemy with a node.
            context.isPathValid = false;
            return State.Failure; // This should never happen.
        }

        Vector2 direction = context.path[0] - (Vector2)context.gameObject.transform.position;
        float directionMagnitude = direction.magnitude;
        if (context.path.Count > 1)
        {
            Vector2 directionToSecondPoint = context.path[1] - (Vector2)context.gameObject.transform.position;
            float directionToSecondPointMagnitude = directionToSecondPoint.magnitude;

            if (directionToSecondPointMagnitude < directionMagnitude) // If the second point is closer, look at that instead.
            {
                direction = directionToSecondPoint; // Set direction to the second point.
                directionMagnitude = directionToSecondPointMagnitude; // Set directionMagnitude to the second point's magnitude.
                context.path.RemoveAt(0); // Remove the first point.
            }
        }

        if (directionMagnitude < context.enemyInfoManager.GetPathPointReachedRadius())
        {
            context.path.RemoveAt(0);
        }

        context.enemyController.Direction = direction;
        context.enemyController.Move(true);

        return State.Success;
    }
}
