using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

/// <summary>
/// Game Manager. The object containing this script is responsible for saving and loading game data, and initializing the game.
/// The object should be persistent through scenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    private const string playerDataPath = "PlayerData.json";
    private const string planetDataPath = "PlanetData.json";
    private PlayerData playerData;
    private PlanetData planetData;
    private static GameManager instance = null;

    [SerializeField]
    PlayerController playerPrefab;
    [SerializeField]
    SpaceshipController spaceshipPrefab;

    /// <summary>
    /// Used to check if there is saved game data and if it should display "load game" button
    /// </summary>
    public bool HasSavedGameData { private set; get; } = false;
    public string CurrentPlanet { private set; get; } = string.Empty;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        TryLoadPlayerData();
    }

    /// <summary>
    /// Saves info that a block was desotroyed in the current planet
    /// </summary>
    public void SavePlanetBlock(Vector2Int block)
    {
        if (CurrentPlanet == string.Empty)
        {
            Debug.LogError("Current planet not set");
            return;
        }

        int[] position = { block.x, block.y };
        planetData.destroyedBlocks.Add( position );
    }

    /// <summary>
    /// Saves player, spaceship position and found elements to a json file at persistent data path
    /// </summary>
    public void SavePlayerData()
    {
        PeriodicTable periodicTable = FindAnyObjectByType<PeriodicTable>();
        PlayerController playerController = FindAnyObjectByType<PlayerController>();
        SpaceshipController spaceshipController = FindAnyObjectByType<SpaceshipController>();
        playerData = new PlayerData(playerController, spaceshipController, periodicTable, CurrentPlanet);

        string json = JsonConvert.SerializeObject(playerData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + playerDataPath, json);
    }

    /// <summary>
    /// Saves current planet data to a json file at persistent data path
    /// </summary>
    public void SavePlanetData()
    {
        string json = JsonConvert.SerializeObject(planetData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + planetDataPath, json);
    }

    /// <summary>
    /// Generates a new world and loads saved data from that planet
    /// </summary>
    public void InitializePlanet(string planetName)
    {
        CurrentPlanet = planetName;
        string planetDataPath = Application.persistentDataPath + "/" + planetName + ".json";

        if (System.IO.File.Exists(planetDataPath))
        {
            string json = System.IO.File.ReadAllText(planetDataPath);
            planetData = JsonConvert.DeserializeObject<PlanetData>(json);
        }
        else
        {
            string seed = Random.Range(0, 1000000).ToString();
            planetData = new PlanetData(planetName, seed);
        }

        SceneManager.LoadScene("PlanetScene");
        WorldGenerator generator = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>();

        if (generator == null)
        {
            Debug.LogError("WorldGenerator not found");
            return;
        }

        WorldDescriptor descriptor = Resources.Load<WorldDescriptor>(planetName);

        if (descriptor == null)
        {
            Debug.LogError("WorldDescriptor not found");
            return;
        }

        generator.GenerateWorld(descriptor, planetData);

        //this should be here?
        InitializePlayerData();
    }

    /// <summary>
    /// Instantiates player and spaceship, and sets their positions to the saved data if there is any
    /// </summary>
    public void InitializePlayerData()
    {
        PlayerController player = Instantiate(playerPrefab);
        SpaceshipController spaceship = Instantiate(spaceshipPrefab);
        PeriodicTable periodicTable = FindAnyObjectByType<PeriodicTable>();

        if (!HasSavedGameData)
        {
            spaceship.SetPlayer(player);
            spaceship.transform.position = new Vector3(0, 300, 0);
            return;
        }

        player.transform.position = new Vector3(playerData.playerPosition[0], playerData.playerPosition[1], 0);
        spaceship.transform.position = new Vector3(playerData.spaceshipPosition[0], playerData.spaceshipPosition[1], 0);

        foreach (string element in playerData.foundElements)
        {
            periodicTable.foundElements.Add(element);
        }
    }

    /// <summary>
    /// Saves player data and current planet data
    /// </summary>
    public void SaveGame()
    {
        SavePlanetData();
        SavePlayerData();
    }

    /// <summary>
    /// Saves game, and loads main menu scene
    /// </summary>
    public void BackToMainMenu()
    {
        SaveGame();
        SceneManager.LoadScene("MainMenu");
    }

    private void TryLoadPlayerData()
    {
        string path = Application.persistentDataPath + "/" + playerDataPath;
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            HasSavedGameData = true;
        }

        HasSavedGameData = false;
    }
}
