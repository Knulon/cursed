using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

    private Dictionary<int, List<GameObject>> _enemySpawnpointsPerLevel;
    private Dictionary<int, GameObject> _ExitTriggers;

    // Start is called before the first frame update
    void Start()
    {
        // _player = GameObject.Find("Player");
        // _playerSpawnpoints = new();
        if (_enemySpawnpointsPerLevel == null)
        {
            _enemySpawnpointsPerLevel = FillDictionaryWithGameObjectLists("EnemySpawnpoint");
        }
        Debug.Log(_enemySpawnpointsPerLevel);

        _ExitTriggers = new();
        FillDictionaryWithGameObjects(_ExitTriggers, "ExitTrigger");
        SetExitTriggers();

        // Level Objects are named like "PlayerSpawnpoint_1" or "EnemySpawnpoint_1" or "ExitTrigger_1" with the number being the nextLevelID
        //ClearLevel();
    }

    private Dictionary<int, List<GameObject>> FillDictionaryWithGameObjectLists(string tag)
    {
        Dictionary<int, List<GameObject>> dictionary = new();
        List<GameObject> gameObjects = new(GameObject.FindGameObjectsWithTag(tag));
        Debug.Log(gameObjects.Count + " viele Elemente mit dme Tag gefunden!");
        foreach (var gameObject in gameObjects)
        {
            var level = Convert.ToInt32(Regex.Replace(gameObject.name, "[^0-9]", ""));
            if (dictionary.TryGetValue(level, out var value))
            {
                value.Add(gameObject);
            }
            else
            {
                dictionary.Add(level, new List<GameObject>() { gameObject });
            }
        }
        return dictionary;
    }

    private void FillDictionaryWithGameObjects(Dictionary<int, GameObject> dictionary, string tag)
    {
        List<GameObject> gameObjects = new(GameObject.FindGameObjectsWithTag(tag));
        foreach (var gameObject in gameObjects)
        {
            var level = Convert.ToInt32(Regex.Replace(gameObject.name, "[^0-9]", ""));
            dictionary.Add(level, gameObject);
        }
    }

    private void SetExitTriggers()
    {
        foreach (var level in _enemySpawnpointsPerLevel)
        {
            for (int i = 0; i < level.Value.Count; i++)
            {
                EnemySpawner enemySpawner = level.Value[i].GetComponent<EnemySpawner>();
                enemySpawner.SetExitTrigger(_ExitTriggers[level.Key]);
            }
        }
    }

    public void nextLevel(int nextLevelID)
    {
        if (nextLevelID >= 1)
        {
            ClearLevel();
        }
        // _player.transform.position = _playerSpawnpoints[nextLevelID].transform.position;

        if (_enemySpawnpointsPerLevel == null)
        {
            _enemySpawnpointsPerLevel = FillDictionaryWithGameObjectLists("EnemySpawnpoint");
        }

        foreach (var enemySpawnpoint in _enemySpawnpointsPerLevel[nextLevelID])
        {
            EnemySpawner enemySpawner = enemySpawnpoint.GetComponent<EnemySpawner>();
            enemySpawner.Reset();
            enemySpawner.SetStatusOfSpawner(true);
        }
    }

    void OnDestroy()
    {
        ClearLevel();
    }

    public void ClearLevel() // Deletes all enemies and resets all enemy spawners; 
    {
        List<GameObject> enemies = new(GameObject.FindGameObjectsWithTag("enemy"));
        foreach (var enemy in enemies)
        {
            EnemySpawner.ReturnEnemy(enemy);
        }

        List<GameObject> enemySpawners = new(GameObject.FindGameObjectsWithTag("EnemySpawnpoint"));
        foreach (var spawner in enemySpawners)
        {
            EnemySpawner enemySpawner = spawner.GetComponent<EnemySpawner>();
            enemySpawner.Reset();
            enemySpawner.SetStatusOfSpawner(false);
        }
    }
}
