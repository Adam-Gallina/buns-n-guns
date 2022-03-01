using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : LevelController
{
    [SerializeField] private GameObject[] firstClearWeapons;
    [SerializeField] private GameObject[] noPotionsWeapons;


    private int totalPotions;
    private bool noPotions = true;

    private bool noHits = true;

    private void Update()
    {
        if (playerInv && noPotions) 
        {
            int currPotions = playerInv.CountItems(Inventory.HealthPotion);
            if (currPotions < totalPotions)
                noPotions = false;
            totalPotions = currPotions;
        }

        if (playerCtr && playerCtr.GetCurrHealth() < playerCtr.maxHealth)
            noHits = false;
    }

    public override void PlayerWin()
    {
        base.PlayerWin();

        float totalTime = Time.time - startingTime;

        SaveData.instance.totalWins++;
        SaveData.instance.Save();

        string msg = "Epic dub my dude very cool";
        if (SaveData.instance.totalWins == 1)
        {
            msg = GetUnlockMessage("You unlocked", firstClearWeapons);
        }
        else if (noPotions)
        {
            msg = GetUnlockMessage("No potions used! You unlocked", noPotionsWeapons);
        }


        ui.menus.SetWinMenuText(msg, totalTime);
    }

    private string GetUnlockMessage(string baseText, GameObject[] unlockedWeapons)
    {
        string message = baseText.Trim(' ');
        for (int i = 0; i < unlockedWeapons.Length; i++)
        {
            SaveData.instance.UnlockWeapon(unlockedWeapons[i].name);

            if (i == unlockedWeapons.Length - 1 && unlockedWeapons.Length > 1)
            {
                message += " and";
            }

            message += " " + unlockedWeapons[i].name;

            if (unlockedWeapons.Length > 2)
            {
                message += ",";
            }
            
        }

        return message.TrimEnd(',') + "!";
    }

    protected override IEnumerator PlayStory()
    {
        yield return null;
    }
}
