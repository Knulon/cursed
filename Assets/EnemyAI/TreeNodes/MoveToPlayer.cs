using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveToPlayer : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Vector2 directionToPlayer = context.player.transform.position - context.transform.position;
        if (directionToPlayer.magnitude*2 < context.enemyInfoManager.GetAttackPlayerRadius())
        {
            return State.Success;
        }

        context.enemyController.Direction = directionToPlayer;
        context.enemyController.Move();
        return State.Success;
    }
}
