using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using Unity.VisualScripting;

public class RoamInLevel : ActionNode
{
    public float chanceToChangeDirection = 0.05f;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (Random.Range(0f, 1f) < chanceToChangeDirection)
        {
            Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            context.enemyController.Direction = direction;
        }

        context.enemyController.Move();

        return State.Success;
    }
}
