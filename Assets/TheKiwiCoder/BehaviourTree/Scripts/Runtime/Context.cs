using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder {

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context {
        public GameObject gameObject;
        public Transform transform;
        public Animator animator;
        public Rigidbody physics;
        public NavMeshAgent agent;
        public CharacterController characterController;
        public Collider2D collider;
        public SpriteRenderer spriteRenderer;
        public AStar_Handler AStarHandler;
        public EnemyController enemyController;
        public EnemyWeaponController enemyWeaponController;
        public EnemyInfoManager enemyInfoManager;
        public DisplayPath displayPath;
        public GameObject player;
        public PlayerMoveScript playerController;
        public Collider2D playerCollider;
        // TODO: This is where I need to add Components I want to access in my BTs
        // Add other game specific systems here

        public bool isGoalMoving = false;
        public bool isPathValid = false;
        public Vector2 pathGoal = Vector2.negativeInfinity;
        public List<Vector2> path = new();
        public bool isExitGuard = false;
        public bool ExitGuardDecisionMade = false;
        public Dictionary<int, Task<List<Vector2>>> PathfindingTasks;

        public static Context CreateFromGameObject(GameObject gameObject) {
            // Fetch all commonly used components
            Context context = new Context();
            context.gameObject = gameObject;
            context.transform = gameObject.transform;
            context.animator = gameObject.GetComponent<Animator>();
            context.physics = gameObject.GetComponent<Rigidbody>();
            context.agent = gameObject.GetComponent<NavMeshAgent>();
            context.characterController = gameObject.GetComponent<CharacterController>();
            context.collider = gameObject.GetComponent<Collider2D>();
            context.spriteRenderer = context.gameObject.GetComponent<SpriteRenderer>();
            context.AStarHandler = context.gameObject.GetComponent<AStar_Handler>();
            context.enemyController = context.gameObject.GetComponent<EnemyController>();
            context.enemyWeaponController = context.gameObject.GetComponent<EnemyWeaponController>();
            context.enemyInfoManager = context.gameObject.GetComponent<EnemyInfoManager>();
            context.displayPath = context.gameObject.GetComponent<DisplayPath>();
            context.player = GameObject.FindGameObjectWithTag("Player");
            if (context.player == null)
            {
                throw new System.Exception("Player not found, which breaks everything. Pls help.");
            }
            context.playerController = context.player.GetComponent<PlayerMoveScript>();
            context.playerCollider = context.player.GetComponent<Collider2D>();
            context.PathfindingTasks = new Dictionary<int, Task<List<Vector2>>>();

            // Add whatever else you need here...

            return context;
        }
    }
}