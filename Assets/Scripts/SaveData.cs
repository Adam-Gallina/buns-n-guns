using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MinimapDisplay { Cleared, AlwaysOn, Disabled }
public enum ToneTags { Disabled, Enabled }
public enum Character { Default, Cornelius, Tim }
public class SaveData : MonoBehaviour
{
    #region PlayerPref Keys
    private const string UnlockedWeapons = "Unlocked Weapons";
    private const string TotalWins = "Total Wins";

    private const string MinimapOption = "Options - Minimap";
    private const string TonetagspOption = "Options - Tone Tags";
    private const string CharOption = "Options - Character";
    #endregion

    #region Options
    public MinimapDisplay minimapDisplay;
    public ToneTags toneTags;
    public Character character;
    #endregion

    public static SaveData instance;

    [SerializeField] private GameObject[] weaponPrefabs;
    [HideInInspector] public Dictionary<string, GameObject> availableWeapons;
    [HideInInspector] public Dictionary<string, GameObject> unlockedWeapons;

    // Save Data
    [HideInInspector] public int totalWins;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (weaponPrefabs.Length == 0)
            weaponPrefabs = Resources.LoadAll<GameObject>("Prefabs/Weapons/Player Guns");

        availableWeapons = new Dictionary<string, GameObject>();
        foreach (GameObject g in weaponPrefabs)
            availableWeapons.Add(g.name, g);

        Load();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt))
        {
            if (GameController.instance.currLevel == GameController.MAIN_MENU_SCENE)
            {
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    Debug.Log("Cleared UserPrefs");
                    PlayerPrefs.DeleteAll();
                    GameController.instance.ReturnToMenu();
                }
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    Debug.Log("Unlocked Knife Hand");
                    UnlockWeapon("Knife Hand");
                    GameController.instance.ReturnToMenu();
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Debug.Log("Unlocked Long Sniper and Minigun");
                    UnlockWeapon("Long Sniper");
                    UnlockWeapon("Minigun");
                    GameController.instance.ReturnToMenu();
                }
                if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log("I am so disappointed...Unlocked One Pump Chump");
                    UnlockWeapon("One Pump Chump");
                    GameController.instance.ReturnToMenu();
                }
            }
        }
    }

    public void Load()
    {
        LoadUnlockedWeapons();

        LoadOptions();

        totalWins = PlayerPrefs.GetInt(TotalWins, 0);
    }

    public void Save()
    {
        SaveUnlockedWeapons();

        PlayerPrefs.SetInt(TotalWins, totalWins);
    }

    private void LoadOptions()
    {
        minimapDisplay = (MinimapDisplay)PlayerPrefs.GetInt(MinimapOption, (int)MinimapDisplay.Cleared);
        toneTags = (ToneTags)PlayerPrefs.GetInt(TonetagspOption, (int)ToneTags.Disabled);
        character = (Character)PlayerPrefs.GetInt(CharOption, (int)Character.Default);
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetInt(MinimapOption, (int)minimapDisplay);
        PlayerPrefs.SetInt(TonetagspOption, (int)toneTags);
        PlayerPrefs.SetInt(CharOption, (int)character);
    }

    private void LoadUnlockedWeapons()
    {
        unlockedWeapons = new Dictionary<string, GameObject>();
        string[] savedWeapons = PlayerPrefs.GetString(UnlockedWeapons, weaponPrefabs[0].name).Split(',');

        foreach (string weapon in savedWeapons)
        {
            if (availableWeapons.ContainsKey(weapon))
            {
                unlockedWeapons.Add(weapon, availableWeapons[weapon]);
            }
            else
            {
                Debug.LogWarning("Player has unlocked " + weapon + " but it does not match any loaded weapon prefabs");
            }
        }
    }

    private void SaveUnlockedWeapons()
    {
        string savedWeapons = "";

        foreach (string s in unlockedWeapons.Keys)
            savedWeapons += s + ",";

        savedWeapons = savedWeapons.Trim(',');
        PlayerPrefs.SetString(UnlockedWeapons, savedWeapons);
    }


    // Returns true if the weapon was/is unlocked and false otherwise
    public bool UnlockWeapon(string weapon)
    {
        // Check if weapon is already unlocked
        if (unlockedWeapons.ContainsKey(weapon))
            return true;

        if (availableWeapons.ContainsKey(weapon))
        {
            unlockedWeapons.Add(weapon, availableWeapons[weapon]);
            SaveUnlockedWeapons();
            return true;
        }

        Debug.LogWarning("Trying to unlock non-existant weapon " + weapon);
        return false;
    }

}
