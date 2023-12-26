using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ReadPathFromPrevious : ActionNode
{

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        FindPath findPathNode = leftNeighborNode as FindPath;
        if (findPathNode == null)
        {
            Debug.Log("Left neighbor is not valid");
            return State.Failure;
        }

        if (findPathNode.path.Count != 0)
        {
            Debug.Log("I read the path from the left node which is this long: " + findPathNode.path.Count);
        }
        return State.Success;
    }
}
