using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CelularAutomata
{
    private int birthLimit;
    private int stepsLimit;
    private float deathChance;
    private float birthChance;

    private int automataCount = 0;
    private int stepsCount = 0;

    public delegate bool IsInMapLimits(Vector3Int pos);

    public CelularAutomata(int birthLimit, int stepsLimits, float birthChance, float deathChance)
    {
        this.birthLimit = birthLimit;
        this.stepsLimit = stepsLimits;
        this.deathChance = deathChance;
        this.birthChance = birthChance;
    }

    public void AddBlocks(Tilemap map, Tile block, IsInMapLimits isInMap, Vector3Int start)
    {
        automataCount = 0;
        stepsCount = 0;
        AutomataRec(map, block, isInMap, start, Vector2.zero);
    }

    private void AutomataRec(Tilemap map, Tile block, IsInMapLimits isInMap, Vector3Int start, Vector2 weights)
    {
        if(!isInMap(start))
        {
            automataCount--;
            return;
        }

        if(map.GetTile(start) != block)
        {
            stepsCount++;
            map.SetTile(start, block);
        }

        //death
        if (stepsCount >= stepsLimit)
        {
            automataCount--;
            return;
        }

        bool die = UnityEngine.Random.Range(0f, 1f) < deathChance;
        if(die && automataCount > 1) 
        {
            automataCount--;
            return;
        }

        //death

        //birth
        bool willMultiply = UnityEngine.Random.Range(0f, 1f) < birthChance;
        if(willMultiply && automataCount < birthLimit)
        {
            automataCount++;
            AutomataRec(map, block, isInMap, start, weights);
        }
        //birth

        //move
        Vector2 dir = UnityEngine.Random.insideUnitCircle;
        weights += dir;
        dir = weights.normalized * 1.5f;
        Vector3 idir = map.CellToWorld(map.WorldToCell((Vector3)dir + start));
        AutomataRec(map, block, isInMap, new Vector3Int((int)idir.x, (int)idir.y), weights);
    }

}
