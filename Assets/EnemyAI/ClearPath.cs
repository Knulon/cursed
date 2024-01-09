using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ClearPath : ActionNode
{
    protected override void OnStart() {
        context.path.Clear();
        context.isPathValid = false;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
