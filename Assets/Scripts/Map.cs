using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum TileType { Grid, Platform }
    public static TileType GetRowInformation(Vector3 position)
    {
        if (Mathf.Round(position.x) > 12.5f && Mathf.Round(position.x) < 17.5f)
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
        return 0.1f;
    }

    public static bool IsTileEmpty(Vector3 position)
    {
        return (Mathf.Round(position.x) > 12.5f && Mathf.Round(position.x) < 17.5f); 
    }
    //private List
    public static bool IsPositionInBounds(Vector3 position)
    {
        if (position.x < -0.5 || position.x > 19.5 || position.z < -0.5 || position.z > 16.5)
            return false;

        return true;
    }

    public static bool IsTileFree(Vector3 position)
    {
        var blockers = GameObject.FindGameObjectsWithTag("Blocker");
        int myX = Mathf.RoundToInt(position.x);
        int myZ = Mathf.RoundToInt(position.z);
        for (int i = 0; i < blockers.Length; i++)
        {
            int bX = Mathf.RoundToInt(blockers[i].transform.position.x);
            int bZ = Mathf.RoundToInt(blockers[i].transform.position.z);
            if (bX == myX && bZ == myZ)
                return false;
        }
        return true;
    }

    public Material MapMaterial;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
