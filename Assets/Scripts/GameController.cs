using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    #region Constants
    public const int MVMT_COLL_LAYER = 6;
    public const int PLAYER_LAYER = 7;
    public const int WALL_LAYER = 8;
    public const int INCORPOREAL_WALL_LAYER = 9;

    public const int BULLET_LAYER = 10;
    public const int ENEMY_LAYER = 11;
    public const int INTERACTIVE_LAYER = 12;
    public const int PLAYER_INTERACTION_LAYER = 14;

    public const string COLLIDABLE_TAG = "Collidable";
    public const string INCORPOREAL_TAG = "Incorporeal";
    public const string DAMAGEABLE_TAG = "Damageable";

    public const int MAIN_MENU_SCENE = 0;
    public const int LEVEL_1_SCENE = 1;
    #endregion

    #region Game Settings
    public const float hitColDuration = 0.025f;
    #endregion

    public static GameController instance;

    [Header("Debug")]
    public bool DEBUG_spawnEnemies = true;
    public bool DEBUG_spawnBosses = true;
    public bool DEBUG_startWithBossKey = false;
    public GameObject DEBUG_bossKey;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject playerGunPrefab;

    [HideInInspector] public int currLevel = 0;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        currLevel = SceneManager.GetActiveScene().buildIndex;
    }

    private void Start()
    {
        if (currLevel != MAIN_MENU_SCENE)
        {
            StartLevel();
        }
    }

    public void SetPlayerPrefab(GameObject newPrefab)
    {
        playerPrefab = newPrefab;
    }

    public GameObject GetPlayerPrefab()
    {
        return playerPrefab;
    }
    
    private void OnSceneStartLoad()
    {
    }

    private void OnSceneLoaded()
    {
    }
    
    public void ReturnToMenu()
    {
        if (LevelController.instance)
            LevelController.instance.SetPause(false);

        StartCoroutine(LoadScene(MAIN_MENU_SCENE));
    }

    public void LoadLevel(int targetLevel)
    {
        LoadLevel(targetLevel, playerGunPrefab);
    }
    public void LoadLevel(int targetLevel, GameObject playerGun)
    {
        playerGunPrefab = playerGun;

        StartCoroutine(LoadScene(targetLevel, () => StartLevel()));
    }

    private void StartLevel()
    {
        LevelController.instance.StartLevel(playerPrefab, playerGunPrefab);

        if (DEBUG_startWithBossKey)
            LevelController.instance.playerInv.AddItem(DEBUG_bossKey.GetComponent<InventoryItem>());
    }

    private IEnumerator LoadScene(int targetScene)
    {
        OnSceneStartLoad();

        AsyncOperation scene = SceneManager.LoadSceneAsync(targetScene);
        yield return new WaitUntil(() => scene.isDone);

        OnSceneLoaded();

        currLevel = targetScene;
    }
    private IEnumerator LoadScene(int targetScene, UnityAction callback)
    {
        yield return LoadScene(targetScene);

        callback.Invoke();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
