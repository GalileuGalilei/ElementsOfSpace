using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

/// <summary>
/// Class that describes the world, with its width, height, and the rarity of each element.
/// </summary>
[System.Serializable]
public class WorldDescriptor : ScriptableObject
{
    [System.Serializable]
    public class ElementRarity
    {
        public string ElementSymbol;
        public float rarity;
    }

    public int width, height;
    [SerializeField]
    private SpriteAtlas atlas;
    [SerializeField]
    private List<ElementRarity> elementRarity;
    
    public List<(Tile, float)> TilesPerRarity;

    public WorldDescriptor()
    {
        LoadAndGenerateTiles();
    }

    private void LoadAndGenerateTiles()
    {
        if (atlas == null || elementRarity == null || elementRarity.Count == 0)
        {
            return;
        }
        
        TilesPerRarity = new List<(Tile, float)>();

        foreach (ElementRarity er in elementRarity)
        {
            Sprite sprite = atlas.GetSprite(er.ElementSymbol);
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.name = er.ElementSymbol;
            TilesPerRarity.Add((tile, er.rarity));
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
