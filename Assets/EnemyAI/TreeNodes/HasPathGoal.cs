using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class HasPathGoal : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.pathGoal.Equals(Vector2.negativeInfinity))
        {
            return State.Failure;
        }

        return State.Success;
    }
}
