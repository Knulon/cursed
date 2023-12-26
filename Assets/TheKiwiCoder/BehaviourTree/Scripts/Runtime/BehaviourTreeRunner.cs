using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public class BehaviourTreeRunner : MonoBehaviour {

        // The main behaviour tree asset
        public BehaviourTree tree;

        // Storage container object to hold game object subsystems
        Context context;

        // Start is called before the first frame update
        void Start() {
            context = CreateBehaviourTreeContext();
            tree = tree.Clone();
            tree.Bind(context);
            tree.nodes[0].nodeId = 0; // set root node id to 0

            // Set left neighbor nodes for data access
            for (int i = 1; i < tree.nodes.Count; i++)
            {
                tree.nodes[i].nodeId = i; // set node id

                if (tree.nodes[i] == null) // null check
                {
                    continue;
                }

                if (!tree.nodes[i].GetType().IsSubclassOf(typeof(ActionNode))) // skip if not an action node
                {
                    continue;
                }

                if (!tree.nodes[i - 1].GetType().IsSubclassOf(typeof(ActionNode))) // set to root node if previous node is not an action node
                {
                    continue;
                }

                tree.nodes[i].leftNeighborNode = tree.nodes[i - 1]; // set left neighbor
            }
        }

        // Update is called once per frame
        void Update() {
            if (tree) {
                tree.Update();
            }
        }

        Context CreateBehaviourTreeContext() {
            return Context.CreateFromGameObject(gameObject);
        }

        private void OnDrawGizmosSelected() {
            if (!tree) {
                return;
            }

            BehaviourTree.Traverse(tree.rootNode, (n) => {
                if (n.drawGizmos) {
                    n.OnDrawGizmos();
                }
            });
        }
    }
}