using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using Unity.VisualScripting;

public class RoamInLevel : ActionNode
{
    public float chanceToChangeDirection = 0.05f;
    public float minimumTimeInDirection = 0.5f; // Minimum time in seconds to go in a direction before possibly changing direction.
    private float minimumTimeDirectionCounter = 0;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        minimumTimeDirectionCounter -= Time.deltaTime;
        if (minimumTimeDirectionCounter < 0 && Random.Range(0f, 1f) < chanceToChangeDirection)
        {
            Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            context.enemyController.Direction = direction;
            minimumTimeDirectionCounter = minimumTimeInDirection;
        }

        context.enemyController.Move(true);

        return State.Success;
    }
}
