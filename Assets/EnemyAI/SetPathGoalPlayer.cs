using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SetPathGoalPlayer : ActionNode
{
    protected override void OnStart() {
        context.pathGoal = context.player.transform.position;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
