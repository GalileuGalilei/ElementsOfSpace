using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    float noiseScale = 0.03f;

    private int[] surface;

    private int width;
    private int minHeight, maxHeight;
    private List<(Tile, int)> tilesPerLayerHeight;

    private void CalculateWorldBounds(WorldDescriptor worldDescriptor)
    {
        tilesPerLayerHeight = worldDescriptor.TilesPerLayerHeight;
        width = worldDescriptor.width;
        surface = new int[width];

        foreach (var tile in tilesPerLayerHeight)
        {
            if (tile.Item2 < minHeight)
            {
                minHeight = tile.Item2;
            }

            if (tile.Item2 > maxHeight)
            {
                maxHeight = tile.Item2;
            }
        }
    }

    public void GenerateWorld(WorldDescriptor worldDescriptor, PlanetData planetData)
    {
        Random.InitState(planetData.seed.GetHashCode());

        ResetWorld();
        CalculateWorldBounds(worldDescriptor);
        GenerateWorldLayers(width, tilesPerLayerHeight);
        //GenerateTileChuncks(worldDescriptor.TileSizeRarityChuncks);
        GenerateWorldCaves(worldDescriptor.caveAutomataParams);
        AddPlanetData(planetData);
    }

    /// <summary>
    /// generate the world based on the layers separeted by their heights
    /// </summary>
    private void GenerateWorldLayers(int width, List<(Tile, int)> tilesPerLayerHeight)
    {

        for (int x = -width / 2; x < width / 2; x++)
        {
            float noiseValue = Mathf.PerlinNoise(x * noiseScale, 0);
            int currentHeight = tilesPerLayerHeight[0].Item2;

            for (int i = 0; i < tilesPerLayerHeight.Count; i++)
            {
                int columnHeight = Mathf.Abs((int)((tilesPerLayerHeight[i].Item2 - currentHeight) * noiseValue) + 1);

                for (int y = currentHeight; y < currentHeight + columnHeight; y++)
                {
                    tilemap.SetTile(tilemap.WorldToCell(new Vector3Int(x, y, 0)), tilesPerLayerHeight[i].Item1);
                }
                currentHeight += columnHeight;
            }

            surface[x + width / 2] = currentHeight;
        }
    }

    //generate chunks of tiles, like ores
    private void GenerateTileChuncks(List<(Tile, int, int)> TileSizeRarityChuncks)
    {
        int step = 10;

        //generate chunks bellow surface heights
        for (int x = -width / 2; x < width / 2; x += step)
        {
            for (int y = 0; y < surface[x + width / 2]; y += step)
            {
                foreach (var tile in TileSizeRarityChuncks)
                {
                    if (Random.Range(0, 100) < tile.Item3)
                    {
                        //Automata(tileset, tile)
                    }
                }
            }
        }
    }

    private void GenerateWorldCaves(WorldDescriptor.CaveAutomataParams caveParams)
    {
        CelularAutomata automata = new CelularAutomata(caveParams.birthLimit, caveParams.stepsLimit, caveParams.birthChance, caveParams.deathChance, caveParams.movingMultiplier);
        Vector3Int start = new Vector3Int(0, (maxHeight + minHeight) / 2);
        automata.GenerateVeinStructure(tilemap, null, IsInMap, start);
    }

    //loas which blocks were destroyed in the planet in the last save
    private void AddPlanetData(PlanetData planetData)
    {
        foreach (var block in planetData.destroyedBlocks)
        {
            tilemap.SetTile(new Vector3Int(block[0], block[1], 0), null);
        }
    }

    private bool IsInMap(Vector3Int pos)
    {
        if(pos.x >= width*0.5 || pos.x <= -width*0.5)
        {
            return false;
        }

        if(pos.y < minHeight - 2)
        {
            return false;
        }

        if (pos.y > surface[pos.x + width / 2] - 3)
        {
            return false;
        }

        return true;
    }

    private void ResetWorld()
    {
        tilemap.ClearAllTiles();
    }
}
