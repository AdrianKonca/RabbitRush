using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Credits : MonoBehaviour
{
    public GameObject button;
    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(button);
    }
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
