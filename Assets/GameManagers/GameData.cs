using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerData
{
    public float[] playerPosition;
    public float[] spaceshipPosition;
    public string[] foundElements;
    public string currentPlanet;

    /// <summary>
    /// Converts data from unitys objects to the struct.
    /// </summary>
    public PlayerData(PlayerController playerController, SpaceshipController spaceshipController, PeriodicTable periodicTable, string currentPlanet)
    {
        Vector2 playerPosition = playerController.transform.position;
        this.playerPosition = new float[] { playerPosition.x, playerPosition.y };

        Vector2 spaceshipPosition = spaceshipController.transform.position;
        this.spaceshipPosition = new float[] { spaceshipPosition.x, spaceshipPosition.y };

        foundElements = new string[periodicTable.foundElements.Count];
        periodicTable.foundElements.CopyTo(foundElements);

        this.currentPlanet = currentPlanet;
    }
}

[System.Serializable]
public struct PlanetData
{
    public string seed;
    public string name;
    public List<int[]> destroyedBlocks;

    public PlanetData(string seed, string name)
    {
        this.seed = seed;
        this.name = name;
        destroyedBlocks = new List<int[]>();
    }
}

