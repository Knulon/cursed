using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class SetExitGuardPerm : ActionNode
{
    public float ExitGuardChance = 0.5f;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (!context.ExitGuardDecisionMade)
        {
            if (Random.value < ExitGuardChance)
            {
                context.isExitGuard = true;
                context.ExitGuardDecisionMade = true;
            }
            else
            {
                context.isExitGuard = false;
                context.ExitGuardDecisionMade = true;
            }
            return State.Success;
        }

        if (context.isExitGuard)
        {
            return State.Success;
        }

        return State.Failure;
    }
}
