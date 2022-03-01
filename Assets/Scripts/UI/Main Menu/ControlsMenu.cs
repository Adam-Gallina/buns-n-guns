using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    public GameObject normalControlScheme;
    public GameObject oneHandControlScheme;

    public GameObject normalPlayerPrefab;
    public GameObject oneHandPlayerPrefab;

    public Dropdown controlSelector;

    private void Start()
    {
        controlSelector.value = GameController.instance.GetPlayerPrefab() == normalPlayerPrefab ? 0 : 1;
    }

    public void UpdateControlScheme(int value)
    {
        switch (value)
        {
            case 0:
                normalControlScheme.SetActive(true);
                oneHandControlScheme.SetActive(false);
                GameController.instance.SetPlayerPrefab(normalPlayerPrefab);
                break;
            case 1:
                normalControlScheme.SetActive(false);
                oneHandControlScheme.SetActive(true);
                GameController.instance.SetPlayerPrefab(oneHandPlayerPrefab);
                break;
        }
    }
}
