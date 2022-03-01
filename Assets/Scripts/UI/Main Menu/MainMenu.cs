using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private bool DEBUG_showAllWeapons = false;

    [SerializeField] private Dropdown weaponSelect;

    private void Start()
    {
        SetWeaponDropdown();
    }

    private void SetWeaponDropdown()
    {
        List<Dropdown.OptionData> weaponOptions = new List<Dropdown.OptionData>();

        foreach (string s in DEBUG_showAllWeapons ? SaveData.instance.availableWeapons.Keys :
                             SaveData.instance.unlockedWeapons.Keys)
        {
            weaponOptions.Add(new Dropdown.OptionData(s));
        }

        weaponSelect.options = weaponOptions;
    }

    public void StartButton()
    {
        string weaponName = weaponSelect.options[weaponSelect.value].text;
        GameObject selectedWeapon = DEBUG_showAllWeapons ? SaveData.instance.availableWeapons[weaponName] :
                                    SaveData.instance.unlockedWeapons[weaponName];
        GameController.instance.LoadLevel(GameController.LEVEL_1_SCENE, selectedWeapon);
    }

    public void QuitButton()
    {
        GameController.instance.QuitGame();
    }
}
