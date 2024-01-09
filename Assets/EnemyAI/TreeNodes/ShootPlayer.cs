using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ShootPlayer : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Vector2 directionToPlayer = context.player.transform.position - context.transform.position;
        context.enemyWeaponController.Shoot(directionToPlayer);

        return State.Success;
    }
}
