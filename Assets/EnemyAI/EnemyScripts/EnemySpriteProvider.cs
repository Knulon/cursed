using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteProvider : MonoBehaviour
{

    [SerializeField]
    private Sprite _basicEnemyTexture;
    [SerializeField]
    private Sprite _sniperEnemyTexture;
    [SerializeField]
    private Sprite _runnerEnemyTexture;

    public Sprite GetSprite(EnemySpawner.Enemytype enemyType)
    {
        switch (enemyType)
        {
            case EnemySpawner.Enemytype.Normal:
                return _basicEnemyTexture;
            case EnemySpawner.Enemytype.Sniper:
                return _sniperEnemyTexture;
            case EnemySpawner.Enemytype.Runner:
                return _runnerEnemyTexture;
            default:
                return _basicEnemyTexture;
        }
    }

}
