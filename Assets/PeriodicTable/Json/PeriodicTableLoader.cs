using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public struct Element
{
    public string name;
    public string symbol;
    public int xpos;
    public int ypos;
    public string category;
}

internal struct ElementCollection
{
    public List<Element> elements;
}

public class PeriodicTableLoader
{
    public List<Element> elements = new();
    private const string jsonDataPath = "PeriodicTable";

    static PeriodicTableLoader instance;
    public static PeriodicTableLoader Instance
    {
        get
        {
            instance ??= new PeriodicTableLoader();
            return instance;
        }
    }

    public PeriodicTableLoader()
    {
        LoadData();
    }

    public void LoadData()
    {
        TextAsset json = Resources.Load<TextAsset>(jsonDataPath);
        if (json != null)
        {
            ElementCollection col = JsonConvert.DeserializeObject<ElementCollection>(json.text);
            elements = col.elements;
            Debug.Log("Periodic Table data loaded."); 
        }
    }
}
