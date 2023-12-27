using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEditor.Experimental.GraphView;

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
        Vector2 direction = context.player.transform.position - context.gameObject.transform.position;
        float angle = Vector2.Angle(direction, context.gameObject.transform.up);

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
            if (hit.collider.gameObject.tag != "Player")
            {
                Debug.Log("Player out of sight.");
                context.enemyInfoManager.SetPlayerInSight(false);
                return State.Failure;
            }
        }

        Debug.Log("Player in sight.");
        context.enemyInfoManager.SetPlayerInSight(true);
        return State.Success;
    }
}