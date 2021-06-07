using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class InitializePlayerSelectionMenu : MonoBehaviour
{
    public GameObject playerSelectionMenuPrefab;
    public PlayerInput input;
    private void Awake()
    {
        var rootMenu = GameObject.Find("PlayerSelectionMenu");
        if (rootMenu != null)
        {
            var menu = Instantiate(playerSelectionMenuPrefab, rootMenu.transform);
            input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
            menu.GetComponent<PlayerSelectionMenuController>().SetPlayerIndex(input.playerIndex);
            
            //update gui
            var playerSelectionJoin = GameObject.Find(string.Format("PlayerSelectionJoin[{0}]", input.playerIndex));
            menu.transform.position = playerSelectionJoin.transform.position;
            playerSelectionJoin.SetActive(false);

            //if it's player no 1 then allow it to move around the menu
            if (input.playerIndex == 0)
            {
                var readyButton = menu.transform.Find("Ready").GetComponent<Button>();
                var readyButtonNav = readyButton.navigation;

                var nextButton = rootMenu.transform.parent.Find("LevelSelection").transform.Find("Next").GetComponent<Button>();
                var nextButtonNav = nextButton.navigation;

                readyButtonNav.selectOnDown = nextButton;
                nextButtonNav.selectOnUp = readyButton;

                readyButton.navigation = readyButtonNav;
                nextButton.navigation = nextButtonNav;
            }
        }

    }
}
