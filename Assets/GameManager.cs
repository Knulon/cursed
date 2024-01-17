using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject _player;

    [SerializeField] 
    private GameObject _enemySpawnpointPrefab;

    // private Dictionary<int, GameObject> _playerSpawnpoints;
    // private Dictionary<int, GameObject> _keyLocations;
    private Dictionary<int,List<GameObject>> _enemySpawnpointsPerLevel;

    // Start is called before the first frame update
    void Start()
    {
        // _player = GameObject.Find("Player");
        // _playerSpawnpoints = new();
        // FillDictionaryWithGameObjects(_playerSpawnpoints, "PlayerSpawnpoint");
        _enemySpawnpointsPerLevel = new();
        FillDictionaryWithGameObjectLists("EnemySpawnpoint");
        Debug.Log(_enemySpawnpointsPerLevel);
        // _keyLocations = new();
        // FillDictionaryWithGameObjects(_keyLocations, "KeyLocation");

        // Level Objects are named like "PlayerSpawnpoint_1" or "EnemySpawnpoint_1" or "ExitTrigger_1" with the number being the nextLevelID

    }

    private void FillDictionaryWithGameObjectLists(string tag)
    {
        List<GameObject> gameObjects = new(GameObject.FindGameObjectsWithTag(tag));
        Debug.Log(gameObjects.Count + " viele Elemente mit dme Tag gefunden!");
        foreach (var gameObject in gameObjects)
        {
            var level = Convert.ToInt32(gameObject.name.Split('_')[1]);
            if (_enemySpawnpointsPerLevel.TryGetValue(level, out var value))
            {
                value.Add(gameObject);
            }
            else
            {
                _enemySpawnpointsPerLevel.Add(level, new List<GameObject>() { gameObject });
            }
        }
    }

    private void FillDictionaryWithGameObjects(Dictionary<int, GameObject> dictionary, string tag)
    {
        List<GameObject> gameObjects = new(GameObject.FindGameObjectsWithTag(tag));
        foreach (var gameObject in gameObjects)
        {
            var level = Convert.ToInt32(gameObject.name.Split('_')[1]);
            dictionary.Add(level, gameObject);
        }
    }

    public void nextLevel(int nextLevelID)
    {
        if (nextLevelID > 1)
        {
            ClearLevel();
        }
        // _player.transform.position = _playerSpawnpoints[nextLevelID].transform.position;

        foreach (var enemySpawnpoint in _enemySpawnpointsPerLevel[nextLevelID])
        {
            enemySpawnpoint.SetActive(true);
        }
    }

    private void ClearLevel() // Deletes all enemies and resets all enemy spawners; 
    {
        List<GameObject> enemies = new(GameObject.FindGameObjectsWithTag("enemy"));
        foreach (var enemy in enemies)
        {
            EnemySpawner.ReturnEnemy(enemy);
        }

        List<GameObject> enemySpawners = new(GameObject.FindGameObjectsWithTag("EnemySpawner"));
        foreach (var enemySpawner in enemySpawners)
        {
            enemySpawner.GetComponent<EnemySpawner>().Reset();
            enemySpawner.SetActive(false);
        }
    }
}
