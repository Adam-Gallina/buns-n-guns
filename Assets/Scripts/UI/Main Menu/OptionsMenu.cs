using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Dropdown minimapSelector;
    public Dropdown toneTagSelector;
    public Dropdown characterSelector;

    [Header("Player prefabs selected in character list")]
    public GameObject[] playerPrefabs;

    private void Start()
    {
        minimapSelector.value = (int)SaveData.instance.minimapDisplay;
        toneTagSelector.value = (int)SaveData.instance.toneTags;
        characterSelector.value = (int)SaveData.instance.character;

        UpdateCharacter(characterSelector.value);
    }

    public void UpdateMinimap(int value)
    {
        SaveData.instance.minimapDisplay = (MinimapDisplay)value;
        SaveData.instance.SaveOptions();
    }
    public void UpdateToneTag(int value)
    {
        SaveData.instance.toneTags = (ToneTags)value;
        SaveData.instance.SaveOptions();
    }
    public void UpdateCharacter(int value)
    {
        SaveData.instance.character = (Character)value;
        SaveData.instance.SaveOptions();
        GameController.instance.SetPlayerPrefab(playerPrefabs[value]);
    }
}
