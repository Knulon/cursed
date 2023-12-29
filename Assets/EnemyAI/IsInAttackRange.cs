using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IsInAttackRange : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (Vector2.Distance(context.player.transform.position, context.transform.position) <
            context.enemyInfoManager.GetAttackPlayerRadius())
        {
            return State.Success;
        }

        return State.Failure;
    }

}
