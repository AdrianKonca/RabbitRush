using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;
    public float Frequency;
    public int EntityLimit = 10;
    private float _lastSpawned;
    private Queue<GameObject> _prefabs;

    private void Spawn()
    {
        var newPrefab = Instantiate(Prefab, this.transform);
        _prefabs.Enqueue(newPrefab);
        if (_prefabs.Count > EntityLimit)
            Destroy(_prefabs.Dequeue());
        _lastSpawned = Time.time;
    }
    void Start()
    {
        _prefabs = new Queue<GameObject>();
        Spawn();
    }

    void Update()
    {
        if (Time.time > _lastSpawned + Frequency)
        {
            Spawn();
        }
    }
}
