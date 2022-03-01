using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("Game UI")]
    public LevelMenus menus;
    public Heartbar playerHealthBar;
    public Healthbar bossHealthBar;
    public GameObject minimap;

    [Header("Inventory UI")]
    [SerializeField] private GameObject bossKeyIndicator;
    [SerializeField] private Transform keyCountParent;
    [SerializeField] private Transform potionCountParent;

    private Inventory targetInv;

    private void Start()
    {
        menus.HideMenus();
        playerHealthBar.ShowHealthbar();
        bossHealthBar.HideHealthbar();

        if (SaveData.instance.minimapDisplay == MinimapDisplay.Disabled)
            SetMinimap(false);
    }

    public void SetTargetInventory(Inventory inv)
    {
        targetInv = inv;
    }

    private void Update()
    {
        if (targetInv)
        {
            bossKeyIndicator.SetActive(targetInv.CountItems(Inventory.BossKey) == 1);
            MyDebug.SetFancyText(keyCountParent, targetInv.CountItems(Inventory.BasicKey).ToString());
            MyDebug.SetFancyText(potionCountParent, targetInv.CountItems(Inventory.HealthPotion).ToString());
        }

        if (SaveData.instance.minimapDisplay == MinimapDisplay.Cleared)
        {
            SetMinimap(LevelController.instance.currRoom?.clearedRoom == true);
        }
    }

    private void SetMinimap(bool show)
    {
        if (minimap)
            minimap.SetActive(show);
    }
}
