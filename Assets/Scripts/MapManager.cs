using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Tile
{
    public enum Types { Grid, Platform, Blocker }
    public Vector2 Position;
    public Tile North;
    public Tile West;
    public Tile South;
    public Tile East;
    public Types Type;
    public Vector3 GetVector3 => new Vector3(Position.x, 0.2f, Position.y);
    public void Nullify()
    {
        if (North != null)
            North.South = null;
        if (South != null)
            South.North = null;
        if (West != null)
            West.East = null;
        if (East != null)
            East.West = null;
    }
    public void RemoveEdge(Tile v)
    {
        if (v == null)
            return;
        if (North == v)
        {
            North.South = null;
            North = null;
        }
        if (South == v)
        {
            South.North = null;
            South = null;
        }
        if (West == v)
        {
            West.East = null;
            West = null;
        }
        if (East == v)
        {
            East.West = null;
            East = null;
        }
    }
    public bool IsConnected(Tile v)
    {
        if (v == null)
            return false;
        if (North == v && North.South == this ||
            South == v && South.North == this ||
            West == v && West.East == this ||
            East == v && East.West == this)
        {
            return true;
        }
        return false;
    }
}

class Line
{
    public Line(Vector3 start, Vector3 end)
    {
        Start = start;
        End = end;
    }

    public Vector3 Start;
    public Vector3 End;
}

public class Move
{
    public Vector3 Start;
    public Vector3 Target;
    public Vector3 Direction;
    public bool IsSuccessful;
    public PlatformInformation Platform;
}
public class MapManager : MonoBehaviour
{
    Tile[,] Map;
    List<Line> Lines;
    public bool IsDebug;
    public Vector3 Offset;
    public int ColumnCount;
    public int RowCount;
    public static MapManager Instance;
    private float PLATFORM_OFFSET = 0.60f;
    private bool IsInBounds(float x, float z) =>
        x >= Offset.x && x <= Offset.x + RowCount && z >= Offset.z && z <= Offset.z + ColumnCount;
    public bool IsInBounds(Vector3 pos) =>
        IsInBounds(pos.x, pos.z);

    private bool CoordinatesInMap(int x, int z) =>
        x >= 0 && x <= RowCount && z >= 0 && z <= ColumnCount;

    private void DrawMap()
    {
        foreach (var line in Lines)
            Debug.DrawLine(line.Start, line.End);
    }

    private void RemoveNode(int x, int z)
    {
        Map[x, z].Nullify();
        Map[x, z] = null;
    }
    private void RemoveEdge(Tile a, Tile b)
    {
        a.RemoveEdge(b);
    }
    private void FillLines()
    {
        for (int x = 0; x < RowCount; x++)
            for (int z = 0; z < ColumnCount; z++)
            {
                if (Map[x, z] != null && Map[x, z].North != null)
                    Lines.Add(new Line(Map[x, z].GetVector3, Map[x, z].North.GetVector3));
                if (Map[x, z] != null && Map[x, z].West != null)
                    Lines.Add(new Line(Map[x, z].GetVector3, Map[x, z].West.GetVector3));
            }
        for (int x = 0; x < RowCount; x++)
            if (Map[x, ColumnCount] != null && Map[x, ColumnCount].North != null)
                Lines.Add(new Line(Map[x, ColumnCount].GetVector3, Map[x, ColumnCount].North.GetVector3));
        for (int z = 0; z < ColumnCount; z++)
            if (Map[RowCount, z] != null && Map[RowCount, z].West != null)
                Lines.Add(new Line(Map[RowCount, z].GetVector3, Map[RowCount, z].West.GetVector3));
    }

    private void RemoveBlockedNodes()
    {
        for (int x = 0; x < RowCount + 1; x++)
            for (int z = 0; z < ColumnCount + 1; z++)
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(Map[x, z].GetVector3 + new Vector3(0, 25f, 0) , -transform.up, 50);
                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject.tag == "Blocker")
                        Map[x, z].Type = Tile.Types.Blocker;
                    if (hit.collider.gameObject.tag == "PlatformTerrain")
                        Map[x, z].Type = Tile.Types.Platform;
                }
            }
    }
    private void RemoveBlockedEdges()
    {
        int SAMPLING_RATE = 3;
        var directions = new List<Vector3>() {
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
        };
        var yOffset = new Vector3(0, 25f, 0);

        for (int x = 0; x < RowCount + 1; x++)
            for (int z = 0; z < ColumnCount + 1; z++)
            {
                for (int p = 1; p <= SAMPLING_RATE; p++)
                {
                    if (Map[x, z] is null)
                        continue;
                    float v = p / (float)SAMPLING_RATE;
                    foreach (var direction in directions)
                    {
                        var hits = Physics.RaycastAll(Map[x, z].GetVector3 + yOffset + direction * v, -transform.up, 50);
                        foreach (var hit in hits)
                            if (hit.collider.gameObject.tag == "Blocker" && IsInBounds(x + (int)direction.x, z + (int)direction.z))
                                RemoveEdge(Map[x, z], Map[x + (int)direction.x, z + (int)direction.z]);
                    }

                }
            }
    }
    private void CreateMap()
    {
        Map = new Tile[RowCount + 1, ColumnCount + 1];
        for (int x = 0; x < RowCount + 1; x++)
            for (int z = 0; z < ColumnCount + 1; z++)
                Map[x,z] = new Tile { Position = new Vector2(x + Offset.x, z + Offset.z)};
    }

    private void CreateEdges()
    {
        for (int x = 0; x < RowCount + 1; x++)
            for (int z = 0; z < ColumnCount + 1; z++)
            {
                if (CoordinatesInMap(x, z - 1))
                    Map[x, z].East = Map[x, z - 1];
                if (CoordinatesInMap(x, z + 1))
                    Map[x, z].West = Map[x, z + 1];
                if (CoordinatesInMap(x - 1, z))
                    Map[x, z].South = Map[x - 1, z];
                if (CoordinatesInMap(x + 1, z))
                    Map[x, z].North = Map[x + 1, z];
            }
    }
    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning("Trying to create another singleton!");
        Instance = this;
    }
    void Start()
    {
        CreateMap();
        CreateEdges();
        RemoveBlockedNodes();
        RemoveBlockedEdges();
        Lines = new List<Line>();
        FillLines();
        Debug.LogWarning(Lines.Count);
    }

    void Update()
    {
        var lu = new Dictionary < Tile.Types, Color>() {
            { Tile.Types.Blocker, new Color(1, 0, 0)},
            { Tile.Types.Platform, new Color(0, 0, 1)},
            { Tile.Types.Grid, new Color(0, 1, 0)}
        };
        if (IsDebug)
        {
            DrawMap();
            for (int x = 0; x < RowCount + 1; x++)
                for (int z = 0; z < ColumnCount + 1; z++)
                {
                    Debug.DrawRay(Map[x, z].GetVector3, Vector3.up * 5, lu[Map[x, z].Type]);
                }
        }
    }

    public Vector2Int GetGridCoords(Vector3 position) =>
        new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));

    public Vector3 GetGridPosition(Vector3 position) =>
        new Vector3(Mathf.Round(position.x), position.y, Mathf.Round(position.z));
    public float GetTileHeight(int x, int z)
    {
        return 0.1f;
    }
    public bool IsTileFree(int x, int z) =>
        Map[x, z] != null;

    //public bool IsMoveLegal(Vector3 start, Vector3 end)
    //{
    //    var startCoords = GetGridCoords(start);
    //    var endCoords = GetGridCoords(end);

    //    if (startCoords)
    //    {

    //    }
    //}
    public PlatformInformation GetNearestPlatform(Vector3 offset)
    {
        var platforms = GameObject.FindGameObjectsWithTag("Platform");
        //retrieve platform objects from map that we are allowed to jump on
        var platformPositions = new List<PlatformInformation>();
        foreach (var platform in platforms)
        {
            var script = platform.transform.GetComponent<PlatformPositioner>();
            var r = script.GetNextObjectPositions(1f / 4f);
            platformPositions.AddRange(r);
        }
        foreach (var position in platformPositions)
        {
            Debug.DrawLine(transform.position, position.FuturePosition);
        }
        platformPositions = platformPositions.OrderBy(pos => Vector3.Distance(transform.position + offset, pos.FuturePosition)).ToList();
        if (platformPositions.Count() == 0)
            return null;

        Debug.DrawLine(transform.position, platformPositions[0].FuturePosition, new Color(255, 0, 0));
        return platformPositions[0];
    }
    private PlatformInformation GetNextPlatform(Vector3 sourcePosition, float movementSpeed)
    {
        var platforms = GameObject.FindGameObjectsWithTag("Platform");
        //retrieve platform objects from map that we are allowed to jump on
        var platformPositions = new List<PlatformInformation>();
        foreach (var platform in platforms)
        {
            var script = platform.transform.GetComponent<PlatformPositioner>();
            var r = script.GetNextObjectPositions(1f / movementSpeed);
            platformPositions.AddRange(r);
        }
        foreach (var position in platformPositions)
        {
            Debug.DrawLine(transform.position, position.FuturePosition);
        }
        platformPositions = platformPositions.OrderBy(pos => Vector3.Distance(sourcePosition, pos.FuturePosition)).ToList();
        if (platformPositions.Count() == 0)
            return null;

        Debug.DrawLine(transform.position, platformPositions[0].FuturePosition, new Color(255, 0, 0));
        return platformPositions[0];
    }

    //if is in bounds 
    public void ProcessNextMove(Move move, Move previousMove, float movementSpeed)
    {
        //if move is out of bounds then move isn't successful;
        if (!IsInBounds(move.Target))
            return;

        //if move is going to happen on platform, calculate next position based on platform
        var start = move.Start - Offset;
        var target = move.Target - Offset;
        var current = Map[(int)start.x, (int)start.z];
        var next = Map[(int)target.x, (int)target.z];
        var currentTileType = current.Type;
        var nextTileType = next.Type;

        //if next tile is blocker or there is no connection then move isn't successful
        if (nextTileType == Tile.Types.Blocker || !current.IsConnected(next))
            return;

        if (currentTileType == Tile.Types.Grid && nextTileType == Tile.Types.Grid)
        {
            //do nothing
        }
        else if (currentTileType == Tile.Types.Grid && nextTileType == Tile.Types.Platform)
        {
            move.Platform = GetNextPlatform(move.Target, movementSpeed);
            if (Vector3.Distance(move.Platform.FuturePosition, move.Target) > PLATFORM_OFFSET)
            {
                move.Platform = null;
                move.Target.y = -0.2f;
            }
            else
            {
                move.Target = move.Platform.FuturePosition;
                move.Direction = move.Target - move.Start;
                move.Direction.y = 0f;
            }
        }
        else if (currentTileType == Tile.Types.Platform && nextTileType == Tile.Types.Grid)
        {
            move.Target = GetGridPosition(move.Target);
            move.Target.y = GetTileHeight((int)move.Target.x, (int)move.Target.z);
            move.Direction = move.Target - move.Start;
            move.Direction.y = 0f;
        }
        else if (currentTileType == Tile.Types.Platform && nextTileType == Tile.Types.Platform)
        {
            move.Platform = GetNextPlatform(move.Target, movementSpeed);
            if (Vector3.Distance(move.Platform.FuturePosition, move.Target) > PLATFORM_OFFSET || move.Platform == previousMove.Platform)
            {
                
                move.Platform = null;
                move.Target = move.Start + move.Direction * 1.25f;
                move.Target.y = -0.2f;
                move.Direction = move.Target - move.Start;
            }
            else
            {
                move.Target = move.Platform.FuturePosition;
                move.Direction = move.Target - move.Start;
                move.Direction.y = 0f;
            }
        }
        move.IsSuccessful = true;
    }
}
