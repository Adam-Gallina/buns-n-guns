using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelController : MonoBehaviour
{
    public static LevelController instance;
    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerController playerCtr;
    [HideInInspector] public Inventory playerInv;

    [Header("Level")]
    [SerializeField] protected GameObject[] startingInventory;
    protected float startingTime;
    [HideInInspector] public RoomBase currRoom;

    [Header("UI")]
    [HideInInspector] public UI ui;

    [Header("Paused")]
    [HideInInspector] public bool paused = false;
    [HideInInspector] public bool movementPaused = false;

    protected virtual void Awake()
    {
        instance = this;

        Physics2D.IgnoreLayerCollision(GameController.MVMT_COLL_LAYER, GameController.MVMT_COLL_LAYER);
        Physics2D.IgnoreLayerCollision(GameController.MVMT_COLL_LAYER, GameController.ENEMY_LAYER);
        Physics2D.IgnoreLayerCollision(GameController.MVMT_COLL_LAYER, GameController.PLAYER_LAYER);

        ui = GameObject.Find("Canvas").GetComponentInChildren<UI>();
    }

    public virtual void StartLevel(GameObject playerPrefab, GameObject playerGunPrefab)
    {
        foreach (RoomBase room in GameObject.Find("Environment").GetComponentsInChildren<RoomBase>())
        {
            room.AssignSmokeCloud(SmokeScreen.instance.AddSmokeCloud(room.GetRoomMin(), room.GetRoomMax()));
        }

        foreach (Interactive obj in GameObject.Find("Environment").GetComponentsInChildren<Interactive>())
        {
            obj.AssignSmokeCell(SmokeScreen.instance.GetSmokeCellAtPosition(obj.transform.position));
        }

        SpawnPlayer(playerPrefab, playerGunPrefab);
        SetPause(false);

        OnLevelReady();
    }

    // Called when the level is set up and ready to be ran
    protected virtual void OnLevelReady()
    {
        startingTime = Time.time;

        StartStory();
    }

    public virtual void SpawnPlayer(GameObject playerPrefab, GameObject playerGunPrefab)
    {
        SpawnRoom spawn = GameObject.Find("Rooms")?.GetComponentInChildren<SpawnRoom>();

        if (!spawn)
        {
            Debug.LogWarning("Cannot find a Spawn room in the current level, searching for a player instead");
            if (!GameObject.Find("Player"))
            {
                Debug.LogError("Cannot find a player in current level");
                return;
            }

            player = GameObject.Find("Player").transform;  
            playerCtr = player.GetComponent<PlayerController>();
            playerInv = player.GetComponent<Inventory>();
            return;
        }

        player = spawn.SpawnPlayer(playerPrefab, playerGunPrefab).transform;
        playerCtr = player.GetComponent<PlayerController>();
        playerInv = player.GetComponent<Inventory>();
        ui?.SetTargetInventory(playerInv);

        UpdateCurrRoom(spawn);

        foreach (GameObject item in startingInventory)
            playerInv.AddItem(item.GetComponent<InventoryItem>());

        if (ui.playerHealthBar)
        {
            ui.playerHealthBar.AssignEntity(player.GetComponent<PlayerController>(), "Player");
            ui.playerHealthBar.ShowHealthbar();
        }
        else 
        {
            Debug.LogWarning("playerHealthBar is not set");
        }
    }

    public void UpdateCurrRoom(RoomBase newRoom)
    {
        currRoom = newRoom;
    }

    public virtual void PlayerDeath()
    {
        ui.menus.SetDeathMenuVisibility(true);
        player.GetComponent<PlayerController>().SetAnimationState(true);
    }

    public virtual void PlayerWin()
    {
        ui.menus.SetWinMenuVisibility(true);
        player.GetComponent<PlayerController>().SetAnimationState(true);
    }

    #region Pausing
    public void TogglePause()
    {
        SetPause(!paused);
    }
    public void SetPause(bool pauseState, bool updatePauseMenu = true)
    {
        paused = pauseState;
        Time.timeScale = paused ? 0 : 1;
        if (updatePauseMenu)
            ui.menus?.SetPauseMenuVisibility(pauseState);
    }

    public void ToggleMovement()
    {
        SetMovement(!movementPaused);
    }
    public void SetMovement(bool movementState)
    {
        movementPaused = movementState;
    }
    #endregion

    #region Story
    protected virtual void StartStory()
    {
        StartCoroutine(PlayStory());
    }

    protected abstract IEnumerator PlayStory();
    #endregion
}
