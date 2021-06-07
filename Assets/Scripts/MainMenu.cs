using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject OptionsMenu;
    public GameObject FirstButton;
    public Animator animator;

    private void Start()
    {
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(FirstButton);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MultiplayerSelection");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
