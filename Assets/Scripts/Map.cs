using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum TileType { Grid, Platform }
    public static TileType GetRowInformation(Vector3 position)
    {
        if (Mathf.Round(position.x) == 1 || Mathf.Round(position.x) == 2 || Mathf.Round(position.x) == 9)
        {
            return TileType.Platform;
        }
        else
        {
            return TileType.Grid;
        }
    }
    public static float GetTileHeight()
    {
        return 0f;
    }

    public static bool IsTileEmpty(Vector3 position)
    {
        return Mathf.Round(position.x) == 1 || Mathf.Round(position.x) == 2 || Mathf.Round(position.x) == 9;
    }
    public static bool IsPositionInBounds(Vector3 position)
    {
        return !(position.x < -0.5 || position.x > 11.5 || position.z < -0.5 || position.z > 11.5);
    }

    public Material MapMaterial;

    void Start()
    {
        var road = new Color(0.1f, 0.1f, 0.1f);
        var grass = new Color(0.1f, 0.7f, 0.1f);
        var water = new Color(0.1f, 0.1f, 0.7f);
        var interRoad = new Color(0.4f, 0.4f, 0.4f);
        var gameObject = new GameObject();
        gameObject.transform.parent = transform;
        gameObject.name = "Tiles";
        for (int x = 0; x < 12; x++)
        {
            for (int z = 0; z < 12; z++)
            {
                var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.name = "Tile(" + x + ", " + z + ")";
                plane.transform.parent = gameObject.transform;
                plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                plane.transform.position = new Vector3((float)x, 0f, (float)z);

                Color color;
                if (x == 3 || x == 4 || x == 6 || x == 7)
                    color = road;
                else if (x == 5)
                    color = interRoad;
                else if (x == 1 || x == 9)
                    color = water;
                else
                    color = grass;

                plane.GetComponent<Renderer>().material = MapMaterial;
                plane.GetComponent<Renderer>().material.color = color;
                Destroy(plane.GetComponent<MeshCollider>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
