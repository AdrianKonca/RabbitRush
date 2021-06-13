using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;
    public float Frequency;
    public float FrequencyVariation = 0.2f;
    public float MissChance = 0.2f;
    public int EntityLimit = 10;
    public SpawnerType type;
    public enum SpawnerType { Deadly, Platform };

    private float _nextSpawnIn;
    private float _lastSpawned;
    private Queue<GameObject> _prefabs;

    private void Spawn()
    {
        var newPrefab = Instantiate(Prefab, this.transform);
        var tag = type.ToString();
        newPrefab.tag = tag;
        newPrefab.name = this.name + " " + tag;
        _prefabs.Enqueue(newPrefab);
        if (_prefabs.Count > EntityLimit)
            Destroy(_prefabs.Dequeue());
        _lastSpawned = Time.time;
        _nextSpawnIn = Frequency + Frequency * Random.Range(-FrequencyVariation, FrequencyVariation);
        if (type == SpawnerType.Deadly)
            _nextSpawnIn /= GameController.Instance.enemySpawnRate;
    }
    void Start()
    {
        _prefabs = new Queue<GameObject>();

        Spawn();
    }

    void Update()
    {
        if (Time.time > _lastSpawned + _nextSpawnIn)
        {
            if (Random.value > MissChance)
                Spawn();
            else
                _lastSpawned = Time.time;
        }
    }
}
