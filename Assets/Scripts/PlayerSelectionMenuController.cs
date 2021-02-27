using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionMenuController : MonoBehaviour
{
    [SerializeField]
    private int PlayerIndex;
    [SerializeField]
    private GameObject readyPanel;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private Button readyButton;

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled;

    public void SetPlayerIndex(int playerIndex)
    {
        PlayerIndex = playerIndex;
        ignoreInputTime = Time.time + ignoreInputTime;
    }
    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void OnPlayerReady()
    {
        if (!inputEnabled)
            return;
        bool isReady = !PlayerSelectionManager.Instance.GetPlayerReady(PlayerIndex);
        PlayerSelectionManager.Instance.SetPlayerReady(PlayerIndex, isReady);
        if (isReady)
        {
            readyButton.GetComponent<Image>().color = new Color(0, 1, 0);
        }
        else
        {
            readyButton.GetComponent<Image>().color = new Color(1, 0, 0);
        }
    }
}
