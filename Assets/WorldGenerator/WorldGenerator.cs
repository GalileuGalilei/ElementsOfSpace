using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemap;
    [SerializeField]
    WorldDescriptor worldDescriptor;

    private int width, height;
    private List<(Tile, float)> tilesPerRarity;
    private List<Tile> cumulativeTiles;

    void Start()
    {
        if (worldDescriptor == null)
        {
            Debug.LogError("WorldDescriptor is not set");
            return;
        }

        width = worldDescriptor.width;
        height = worldDescriptor.height;
        tilesPerRarity = worldDescriptor.TilesPerRarity;

        PrecomputeTileSelection();
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        if (tilesPerRarity == null || tilesPerRarity.Count == 0)
        {
            Debug.LogError("TilesPerRarity list is empty.");
            return;
        }

        // Set ground level around y = 0
        int groundLevel = 0;

        // Loop over each tile position in the map width and height
        for (int x = 0; x < width; x++)
        {
            // Generate a random height offset for each column to create mountain peaks
            int columnHeight = (int)(groundLevel + Mathf.PerlinNoise(x * 0.1f, 0) * height / 2 - height / 4);

            for (int y = -height / 2; y <= columnHeight; y++)
            {
                // Select a tile based on rarity
                Tile selectedTile = GetTileByRarity();

                // Place the tile on the Tilemap
                tilemap.SetTile(new Vector3Int(x - width / 2, y, 0), selectedTile);
            }
        }
    }

    private void PrecomputeTileSelection()
    {
        cumulativeTiles = new List<Tile>();

        foreach (var tileRarity in tilesPerRarity)
        {
            int count = Mathf.RoundToInt(tileRarity.Item2 * 100); // Scale the rarity to get an integer count
            for (int i = 0; i < count; i++)
            {
                cumulativeTiles.Add(tileRarity.Item1);
            }
        }
    }

    private Tile GetTileByRarity()
    {
        if (cumulativeTiles.Count == 0)
        {
            Debug.LogError("Cumulative tiles list is empty. Did you forget to call PrecomputeTileSelection?");
            return null;
        }

        int randomIndex = Random.Range(0, cumulativeTiles.Count);
        return cumulativeTiles[randomIndex];
    }
}
