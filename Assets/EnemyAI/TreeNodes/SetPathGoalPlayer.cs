using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SetPathGoalPlayer : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Vector2 directionToPlayer = (Vector2)context.player.transform.position - context.pathGoal;

        if (directionToPlayer.magnitude*2f < context.enemyInfoManager.GetDetectPlayerRadius())
        {
            return State.Failure; // If the player is close enough, don't generate a new path.
        }

        context.pathGoal = context.player.transform.position;
        return State.Success;
    }
}
