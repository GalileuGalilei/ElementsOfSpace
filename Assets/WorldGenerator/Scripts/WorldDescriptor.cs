using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

/// <summary>
/// Class that describes the world, with terrain layers and their rarity.
/// </summary>
[System.Serializable]
public class WorldDescriptor : ScriptableObject
{
    [System.Serializable]
    public class ElementLayerHeight
    {
        public string ElementSymbol;
        public int layerHeight;
    }

    [System.Serializable]
    public class ElementChunk
    {
        public string ElementSymbol;
        public int chunkSize;
        public int rarity;
    }

    [System.Serializable]
    public class CaveAutomataParams
    {
        public float birthChance;
        public float deathChance;
        public float movingMultiplier;
        public int stepsLimit;
        public int birthLimit;
    }

    public int width;

    [SerializeField]
    private SpriteAtlas atlas;
    [SerializeField]
    private List<ElementLayerHeight> elementLayerHeight;
    [SerializeField]
    private List<ElementChunk> elementChunks;

    [SerializeField]
    public CaveAutomataParams caveAutomataParams;

    public List<(Tile, int)> TilesPerLayerHeight;
    public List<(Tile, int, int)> TileSizeRarityChuncks;

    public WorldDescriptor()
    {
        LoadAndGenerateTiles();
    }

    private void LoadAndGenerateTiles()
    {
        if (atlas == null || elementLayerHeight == null || elementLayerHeight.Count == 0)
        {
            return;
        }

        TilesPerLayerHeight = new List<(Tile, int)>();
        TileSizeRarityChuncks = new List<(Tile, int, int)>();

        foreach (ElementLayerHeight elem in elementLayerHeight)
        {
            Sprite sprite = atlas.GetSprite(elem.ElementSymbol);
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.name = elem.ElementSymbol;
            TilesPerLayerHeight.Add((tile, elem.layerHeight));
        }

        foreach (ElementChunk elem in elementChunks)
        {
            Sprite sprite = atlas.GetSprite(elem.ElementSymbol);
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.name = elem.ElementSymbol;
            TileSizeRarityChuncks.Add((tile, elem.chunkSize, elem.rarity));
        }
    }

    private void OnValidate()
    {
        LoadAndGenerateTiles();
    }

    private void Awake()
    {
        LoadAndGenerateTiles();
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/World Descriptor")]
    public static void CreateWorldDescriptorAsset()
    {
        var asset = ScriptableObject.CreateInstance<WorldDescriptor>();
        AssetDatabase.CreateAsset(asset, "Assets/NewWorldDescriptor.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

#endif
}
