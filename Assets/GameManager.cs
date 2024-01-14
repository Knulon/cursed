using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private int _level = 0;
    private GameObject _player;
    private Dictionary<int, List<GameObject>> _playerSpawnpoints;
    private Dictionary<int, List<GameObject>> _exitTrigger;
    private Dictionary<int, List<GameObject>> _keyLocations;

    private Dictionary<int,List<GameObject>> _enemySpawnpointsPerLevel;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        _playerSpawnpoints = new();
        FillDictionaryWithGameObjectLists(_playerSpawnpoints, "PlayerSpawnpoint");
        _exitTrigger = new();
        FillDictionaryWithGameObjectLists(_exitTrigger, "ExitTrigger");
        _enemySpawnpointsPerLevel = new();
        FillDictionaryWithGameObjectLists(_enemySpawnpointsPerLevel, "EnemySpawnpoint");
        _keyLocations = new();
        FillDictionaryWithGameObjectLists(_keyLocations, "KeyLocation");
        
        // Level Objects are named like "PlayerSpawnpoint_1" or "EnemySpawnpoint_1" or "ExitTrigger_1" with the number being the level


    }

    private void FillDictionaryWithGameObjectLists(Dictionary<int, List<GameObject>> dictionary, string tag)
    {
        List<GameObject> gameObjects = new(GameObject.FindGameObjectsWithTag(tag));
        foreach (var gameObject in gameObjects)
        {
            var level = Convert.ToInt32(gameObject.name.Split('_')[1]);
            if (dictionary.TryGetValue(level, out var value))
            {
                value.Add(gameObject);
            }
            else
            {
                dictionary.Add(level, new List<GameObject>() { gameObject });
            }
        }
    }

    void nextLevel()
    {
        _level++;
        _player.transform.position = _playerSpawnpoints[_level][Random.Range(0,_playerSpawnpoints.Count)].transform.position;

        foreach (var enemySpawnpoint in _enemySpawnpointsPerLevel[_level])
        {
            enemySpawnpoint.SetActive(true);
        }

        foreach (var exitTrigger in _exitTrigger[_level])
        {
            exitTrigger.SetActive(true);
        }
    }

    private void ClearLevel() // Deletes all enemies and resets all enemy spawners; TODO: Reset player stats
    {
        List<GameObject> enemies = new(GameObject.FindGameObjectsWithTag("enemy"));
        foreach (var enemy in enemies)
        {
            EnemySpawner.ReturnEnemy(enemy);
        }

        foreach (var enemySpawner in _enemySpawnpointsPerLevel[_level])
        {
            enemySpawner.GetComponent<EnemySpawner>().Reset();
            enemySpawner.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
