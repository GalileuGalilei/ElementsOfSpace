using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Cinemachine;
using UnityEngine.Events;
using System.Collections;
using TreeEditor;
using UnityEngine.Tilemaps;

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
    WorldGenerator worldGeneratorPrefab;
    [SerializeField]
    PlayerController playerPrefab;
    [SerializeField]
    SpaceshipController spaceshipPrefab;

    /// <summary>
    /// Used to check if there is saved game data and if it should display "load game" button
    /// </summary>
    public bool HasSavedGameData { private set; get; } = false;
    public string CurrentPlanet { private set; get; } = string.Empty;
    public Tile CurrentBackgroundTile { private set; get; }
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
        HasSavedGameData = CheckPlayerData();
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
        string path = Application.persistentDataPath + "/" + planetData.name + planetDataPath;
        System.IO.File.WriteAllText(path, json);
    }

    /// <summary>
    /// Clears all previus player and planet data
    /// </summary>
    public void ResetGame()
    {
        ClearPlayerData();
        ClearPlanetsData();
    }

    /// <summary>
    /// Clears player data
    /// </summary>
    public void ClearPlayerData()
    {
        if(HasSavedGameData)
        {
            HasSavedGameData = false;
        }
        else
        {
            return;
        }

        System.IO.File.Delete(Application.persistentDataPath + "/" + playerDataPath);
    }

    /// <summary>
    /// Clears all planets data
    /// </summary>
    public void ClearPlanetsData()
    {
        string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath);
        foreach (string file in files)
        {
            if (file.Contains("PlanetData"))
            {
                System.IO.File.Delete(file);
            }
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

    /// <summary>
    /// Asyncronously loads a new game scene and initializes the planet and player. It there were previus data, deletes it.
    /// </summary>
    /// <param name="planetName"></param>
    public void LoadNewGame(string planetName)
    {
        StartCoroutine(LoadNewGameAsync(planetName));
    }

    /// <summary>
    /// Asyncronously loads a saved game scene and initializes the last planet that player was in.
    /// </summary>
    public void LoadContinueGame()
    {
        StartCoroutine(LoadContinueGameAsync());
    }

    private IEnumerator LoadNewGameAsync(string planetName)
    {
        CurrentPlanet = planetName;
        AsyncOperation sceneLoader = SceneManager.LoadSceneAsync("PlanetScene");

        Debug.Log($"Loading scene planet {planetName}");

        while (!sceneLoader.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        ResetGame();
        InitializePlayer();
        InitializePlanet(planetName);
    }

    private IEnumerator LoadContinueGameAsync()
    {
        AsyncOperation sceneLoader = SceneManager.LoadSceneAsync("PlanetScene");

        while (!sceneLoader.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        InitializePlayer(); //loads current planet name
        InitializePlanet(CurrentPlanet);
    }

    private void InitializePlanet(string planetName)
    {
        string path = Application.persistentDataPath + "/" + planetName + planetDataPath;

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            planetData = JsonConvert.DeserializeObject<PlanetData>(json);
            Debug.Log("Planet data loaded");
        }
        else
        {
            string seed = Random.Range(0, 1000000).ToString();
            planetData = new PlanetData(seed, planetName);
            Debug.Log("New planet data created");
        }

        WorldGenerator generator = Instantiate(worldGeneratorPrefab);

        if (generator == null)
        {
            Debug.LogError("WorldGenerator not found");
        }

        WorldDescriptor descriptor = Resources.Load<WorldDescriptor>(planetName);

        if (descriptor == null)
        {
            Debug.LogError($"WorldDescriptor {planetName} not found");
        }

        CurrentBackgroundTile = descriptor.TileBackground;
        generator.GenerateWorld(descriptor, planetData);
    }

    private void InitializePlayer()
    {
        PlayerController player = Instantiate(playerPrefab);
        SpaceshipController spaceship = Instantiate(spaceshipPrefab);
        PeriodicTable periodicTable = FindAnyObjectByType<PeriodicTable>();
        CinemachineVirtualCamera camera = FindAnyObjectByType<CinemachineVirtualCamera>();

        camera.Follow = player.transform;
        camera.LookAt = player.transform;

        if (!HasSavedGameData)
        {
            spaceship.SetPlayer(player);
            spaceship.transform.position = new Vector3(0, 50, 0);
            return;
        }

        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + playerDataPath);
        playerData = JsonConvert.DeserializeObject<PlayerData>(json);

        player.transform.position = new Vector3(playerData.playerPosition[0], playerData.playerPosition[1], 0);
        spaceship.transform.position = new Vector3(playerData.spaceshipPosition[0], playerData.spaceshipPosition[1], 0);

        //loads last planet player was in
        CurrentPlanet = playerData.currentPlanet;

        foreach (string element in playerData.foundElements)
        {
            periodicTable.foundElements.Add(element);
        }
    }

    /// <summary>
    /// Used only to test if the player data file exists. Used by the main menu to enable or disable the "load game" button
    /// </summary>
    public bool CheckPlayerData()
    {
        string path = Application.persistentDataPath + "/" + playerDataPath;
        if (System.IO.File.Exists(path))
        {
            HasSavedGameData = true;
            Debug.Log("Player data found");
            return true;
        }

        HasSavedGameData = false;
        return false;
    }
}
