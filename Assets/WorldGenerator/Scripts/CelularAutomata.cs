using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CelularAutomata
{
    private int birthLimit;
    private int stepsLimit;
    private float deathChance;
    private float birthChance;
    private float movingMultiplier;

    private int automataCount = 0;
    private int stepsCount = 0;

    public delegate bool IsInMapLimits(Vector3Int pos);

    public CelularAutomata(int birthLimit, int stepsLimits, float birthChance, float deathChance, float movingMultiplier)
    {
        this.birthLimit = birthLimit;
        this.stepsLimit = stepsLimits;
        this.deathChance = deathChance;
        this.birthChance = birthChance;
        this.movingMultiplier = movingMultiplier;
    }

    public void GenerateVeinStructure(Tilemap map, Tile block, IsInMapLimits isInMap, Vector3Int start)
    {
        automataCount = 0;
        stepsCount = 0;
        AutomataIterative(map, block, isInMap, start, movingMultiplier);
        SmoothMap(map, block);
    }

    private void AutomataIterative(Tilemap map, Tile block, IsInMapLimits isInMap, Vector3Int start, float movingMultiplier)
    {
        // A queue to store the active automata with their weights
        Queue<(Vector3Int position, Vector2 weights)> activeAutomata = new Queue<(Vector3Int, Vector2)>();
        activeAutomata.Enqueue((start, Vector2.zero));

        automataCount = 1; // Initial automata
        stepsCount = 0;

        while (activeAutomata.Count > 0 || stepsCount < stepsLimit)
        {
            if(automataCount == 0)
            {
                //generate a new automata at random position
                BoundsInt bounds = map.cellBounds;
                Vector3Int randomPos = new Vector3Int(UnityEngine.Random.Range(bounds.xMin, bounds.xMax), UnityEngine.Random.Range(bounds.yMin, bounds.yMax), 0);
                activeAutomata.Enqueue((randomPos, Vector2.zero));
                automataCount++;
            }

            var (currentPos, currentWeights) = activeAutomata.Dequeue();

            // If the current position is out of bounds, terminate this automata
            if (!isInMap(currentPos))
            {
                automataCount--;
                continue;
            }

            // Place a block if not already set
            if (map.GetTile(currentPos) != block)
            {
                stepsCount++;
                map.SetTile(currentPos, block);
            }

            // Check if the automata should die
            if (stepsCount >= stepsLimit || (UnityEngine.Random.Range(0f, 1f) < deathChance && automataCount > 1))
            {
                automataCount--;
                continue;
            }

            // Check if the automata should multiply
            if (UnityEngine.Random.Range(0f, 1f) < birthChance && automataCount < birthLimit)
            {
                automataCount++;
                activeAutomata.Enqueue((currentPos, currentWeights*0.0f));
            }

            // Move to a new position
            Vector2 newWeight = UnityEngine.Random.insideUnitCircle + currentWeights * movingMultiplier;
            Vector3Int newPos = currentPos + (Vector3Int)ManhatanNormalize(newWeight);

            activeAutomata.Enqueue((newPos, newWeight));
        }
    }


    private void SmoothMap(Tilemap map, Tile block)
    {
        BoundsInt bounds = map.cellBounds;
        List<Vector3Int> positions = new();
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                int surroundingWalls = GetSurroundingWallsCount(pos, map, block);
                if (surroundingWalls > 1)
                {
                    positions.Add(pos);
                }
            }
        }

        foreach (var pos in positions)
        {
            map.SetTile(pos, block);
        }
    }

    private Vector2Int ManhatanNormalize(Vector2 dir)
    {
        Vector2Int idir = new Vector2Int();

        if (Mathf.Abs(dir.x) > 0.5)
        {
            idir.x = dir.x > 0 ? 1 : -1;
        }
        else
        {
            idir.x = 0;
        }

        if (Mathf.Abs(dir.y) > 0.5)
        {
            idir.y = dir.y > 0 ? 1 : -1;
        }
        else
        {
            idir.y = 0;
        }

        return idir;
    }

    private int GetSurroundingWallsCount(Vector3Int cell, Tilemap map, Tile block)
    {
        int count = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector3Int neighborPos = cell + new Vector3Int(x, y, 0);

                if (map.GetTile(neighborPos) == block)
                {
                    count++;
                }
            }
        }

        return count;
    }

}
