using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class PlayerInSight : ActionNode
{
    bool playerInSight;
    ContactFilter2D contactFilter2D;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Vector2 direction = context.player.transform.position - context.transform.position;
        float angle = Vector2.Angle(direction, context.transform.up);

        // TODO: Check if this is needed and then implement the fov angle check
        //if (angle < 0)
        //{
        //    return State.Failure;
        //}

        float distance = direction.magnitude;
        if (distance > context.enemyInfoManager.GetDetectPlayerRadius())
        {
            context.enemyInfoManager.SetPlayerInSight(false);
            return State.Failure;
        }

        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        context.collider.Raycast(direction, contactFilter2D, hits, distance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle")) // Changed from test if not player to test if obstacle because of bullets triggering this
            {
                context.enemyInfoManager.SetPlayerInSight(false);
                return State.Failure;
            }
        }

        context.enemyInfoManager.SetPlayerInSight(true);
        return State.Success;
    }
}
