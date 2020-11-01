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
    public float MissChange = 0.2f;
    public int EntityLimit = 10;
    public SpawnerType type;
    public enum SpawnerType { Deadly, Platform };

    private float _frequency;
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
        _frequency = Frequency + Frequency * Random.Range(-FrequencyVariation, FrequencyVariation);
    }
    void Start()
    {
        _prefabs = new Queue<GameObject>();

        Spawn();
    }

    void Update()
    {
        if (Time.time > _lastSpawned + _frequency)
        {
            if (Random.value > MissChange)
                Spawn();
            else
                _lastSpawned = Time.time;
        }
    }
}
