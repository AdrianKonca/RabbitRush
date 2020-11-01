using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum TileType { Grid, Platform }
    public static TileType GetRowInformation(Vector3 position)
    {
        if (Mathf.Round(position.x) == 1 || Mathf.Round(position.x) == 5)
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
        return Mathf.Round(position.x) == 1 || Mathf.Round(position.x) == 5;
    }
    public static bool IsPositionInBounds(Vector3 position)
    {
        return !(position.x < -0.5 || position.x > 9.5 || position.z < -0.5 || position.z > 9.5);
    }

    public GameObject spawner;
    void Start()
    {
        var gameObject = new GameObject();
        gameObject.transform.parent = transform;
        gameObject.name = "Tiles";
        for (int x = 0; x < 10; x++)
        {
            if (!(x == 1 || x == 5))
                for (int z = 0; z < 10; z++)
                {
                    var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    plane.name = "Tile(" + x + ", " + z + ")";
                    plane.transform.parent = gameObject.transform;
                    plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    plane.transform.position = new Vector3((float)x, 0f, (float)z);
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
