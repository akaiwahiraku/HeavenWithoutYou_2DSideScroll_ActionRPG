using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;

    [ContextMenu("Delete save file")]
    public void DeleteSavedData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        GameObject rootObj = gameObject.transform.root.gameObject;
        DontDestroyOnLoad(rootObj);
        Debug.Log("SaveManager: DontDestroyOnLoad applied to " + rootObj.name);
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        saveManagers = FindAllSaveManagers();

        LoadGame();
    }
    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No saved data found!");
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }

        if (PlayerStats.instance != null)
        {
            LoadPlayerStats(PlayerStats.instance);
        }
    }

    public void SaveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        if (PlayerStats.instance != null)
        {
            SavePlayerStats(PlayerStats.instance);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }

    public bool HasSavedData()
    {
        //if (dataHandler.Load() != null)
        //{
        //    return true;
        //}

        //return false;
        GameData data = dataHandler.Load();
        return data != null;
    }

    public void SavePlayerStats(PlayerStats playerStats)
    {
        gameData.playerLevel = playerStats.currentLevel;
        gameData.playerExperience = playerStats.currentExperience;
        gameData.experienceToNextLevel = playerStats.experienceToNextLevel;
        gameData.strength = playerStats.strength.GetValue();
        gameData.agility = playerStats.agility.GetValue();
        gameData.vitality = playerStats.vitality.GetValue();
        gameData.intelligence = playerStats.intelligence.GetValue();
        gameData.maxHealth = playerStats.maxHealth.GetValue();
    }

    public void LoadPlayerStats(PlayerStats playerStats)
    {
        playerStats.currentLevel = gameData.playerLevel;
        playerStats.currentExperience = gameData.playerExperience;
        playerStats.experienceToNextLevel = gameData.experienceToNextLevel;

        // ロード時にAddModifierでステータスを設定
        playerStats.strength.AddModifier(gameData.strength - playerStats.strength.GetValue());
        playerStats.agility.AddModifier(gameData.agility - playerStats.agility.GetValue());
        playerStats.vitality.AddModifier(gameData.vitality - playerStats.vitality.GetValue());
        playerStats.intelligence.AddModifier(gameData.intelligence - playerStats.intelligence.GetValue());
        playerStats.maxHealth.AddModifier(gameData.maxHealth - playerStats.maxHealth.GetValue());
    }
}
