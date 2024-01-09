using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TheKiwiCoder;
using Vector2 = UnityEngine.Vector2;

public class SetPathGoalExit : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.enemyInfoManager.GetExitTrigger() == null)
        {
            throw new System.Exception("ExitTrigger not set in EnemyInfoManager.");
        }
        Vector2 exitPosition = context.enemyInfoManager.GetExitTrigger().transform.position;

        if (Vector2.Distance(context.pathGoal, exitPosition) < 1)
        {
            return State.Failure; // If the player is close enough, don't generate a new path.
        }

        context.pathGoal = exitPosition;
        return State.Success;
    }
}
