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

public class PeriodicTable
{
    public List<Element> elements = new();
    private const string jsonDataPath = "PeriodicTable";

    static PeriodicTable instance;
    public static PeriodicTable Instance
    {
        get
        {
            instance ??= new PeriodicTable();
            return instance;
        }
    }

    public PeriodicTable()
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
