﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using static UnityEngine.InputSystem.InputAction;

public class GameController : MonoBehaviour
{
    public GameObject PauseObj;
    public GameObject EndMenuObj;
    public Text[] wypisWynikow;
    public Text check;
    public Text carrotText;
    public Text summaryText;
    public static GameController Instance;
    private int carrotCount = 0;
    private int maxCarrotCount = 0;
    private int deathCount = 0;
    private float timeStarted = 0;
    private PlayerInputs playerInputs;
    private bool isPaused = false;
    public Text deathText;
    public Text timeText;
    public AudioClip winSound;
    public AudioClip carrotPickupSound;
    public AudioSource soundEffects;
    public AudioSource music;
    public float startingPitch = 1.0f;
    public float gainedPitch = 1.0f;
    public float elapsedTime = -5.0f;
    public float volumeDown = 0.4f;
    private float pitchPerCarrot;
    [HideInInspector]
    public float enemySpawnRate = 0.8f;
    bool createdFile = false; 
    bool EndGame = false; 

    public enum GameType { Coop, Versus };
    public GameType type = GameType.Coop;

    private void UpdateCarrotText()
    {
        var text = string.Format("{0}/{1}", carrotCount, maxCarrotCount);
        carrotText.text = text;
    }

    private void UpdateTimeText()
    {
        elapsedTime += Time.deltaTime;
        var text = System.String.Format("{0:0.00}s", elapsedTime);
        timeText.text = text;
    }

    private int GetMaxCarrotCount()
    {
        return GameObject.FindGameObjectsWithTag("Carrot").Length;
    }

    private void DisplaySummary()
    {
        string path = Directory.GetCurrentDirectory();
        string fileName;
        if (String.Compare(check.text, "1poziom") == 0)
        {
            fileName = path + "\\" + "Wyniki.txt";
        }
        else if (String.Compare(check.text, "2poziom") == 0)
        {
            fileName = path + "\\" + "Wyniki2.txt";
        }
        else
        {
            fileName = path + "\\" + "inne.txt";
        }
        float totalTime = Time.time - timeStarted - 5;
        string zapis = System.String.Format("Times died: {0} Total time: {1:0.00}s RATING: {2}",
                                        deathCount, totalTime, ScoreSystem.GetGrade(
                                            1, totalTime, deathCount, SceneManager.GetActiveScene().name));
        List<string> linesList = new List<string>();
        if (File.Exists(fileName)){
            string[] lines = File.ReadAllLines(fileName);
            foreach (string item in lines)
            {
                linesList.Add(item);
            }
            File.Delete(fileName);
            bool added = false;
            using (StreamWriter sw = File.CreateText(fileName))
            {
                if (linesList.Count < 10)
                {
                    for (int i = 0; i < linesList.Count; i++)
                    {
                        int start = linesList[i].IndexOf("Times died: ") + "Times died: ".Length;
                        int end = linesList[i].IndexOf(" Total time:");
                        String result = lines[i].Substring(start, end - start);
                        int amountOfDeaths = Int16.Parse(result);//liczba smierci
                        print(amountOfDeaths);

                        start = linesList[i].IndexOf("Total time: ") + "Total time: ".Length;
                        end = linesList[i].IndexOf("s RATING:");
                        result = lines[i].Substring(start, end - start);
                        float time = float.Parse(result);//czas
                        print(time);

                        if (amountOfDeaths >= deathCount && time >= totalTime)
                        {
                            linesList.Insert(i, zapis);
                            added = true;
                            break;
                        }
                    }
                    if(added==false)
                        linesList.Add(zapis);   
                }
                else
                {
                    for (int i = 0; i < linesList.Count; i++)
                    {
                        int start = linesList[i].IndexOf("Times died: ") + "Times died: ".Length;
                        int end = linesList[i].IndexOf(" Total time:");
                        String result = lines[i].Substring(start, end - start);
                        int amountOfDeaths = Int16.Parse(result);//liczba smierci
                        //print(amountOfDeaths);

                        start = linesList[i].IndexOf("Total time: ") + "Total time: ".Length;
                        end = linesList[i].IndexOf("s RATING:");
                        result = lines[i].Substring(start, end - start);
                        float time = float.Parse(result);//czas

                        //print(linesList[i]);
                        //print(time);

                        if (amountOfDeaths >= deathCount && time >= totalTime)
                        {
                            linesList[i] = zapis;
                            break;
                        }
                    }

                }
                foreach (string item in linesList)
                {
                    sw.WriteLine(item.ToString());
                }
            }
        }
        else{
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(zapis);
            }
            linesList.Add(zapis);
        }

        var text = System.String.Format("Times died: {0}\nTotal time: {1:0.00}s\n\nRATING: {2}\n\nPress 'Enter' to go back to main menu.", deathCount, totalTime, ScoreSystem.GetGrade(1, totalTime, deathCount, SceneManager.GetActiveScene().name));
        summaryText.text = text;

        for (int i=0; i < linesList.Count; i++ )
        {
            wypisWynikow[i].gameObject.SetActive(true);
            wypisWynikow[i].text = linesList[i];
        }
        EndGame = true;
        Time.timeScale = 0;
        EndMenuObj.SetActive(true);
    }
    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning("Trying to create another singleton!");
        Instance = this;
        playerInputs = new PlayerInputs();
    }
    void Start()
    {
        maxCarrotCount = GetMaxCarrotCount();
        timeStarted = Time.time;
        music.pitch = startingPitch;
        pitchPerCarrot = gainedPitch / maxCarrotCount;
        UpdateCarrotText();
    }
    void Update()
    {
        UpdateTimeText();

        if (carrotCount == maxCarrotCount)
        {
            if (!summaryText.IsActive())
            {
                AudioManager.Instance.PlaySound(AudioManager.Sounds.Win);
                music.pitch = startingPitch;
                music.volume *= volumeDown;
                DisplaySummary();
                foreach (var cm in FindObjectsOfType<CharacterMovement>())
                    cm.SetMovement(false);
            }
        }
    }
    public void OnActionTriggered(CallbackContext context)
    {
        if (context.action.name == "Look")
            return;
        //TODO: Change to something else or just add UI
        if (context.action.name == playerInputs.Player.Fire.name)
        {
            if (summaryText.IsActive())
            {
                Destroy(GameObject.Find("PlayerSelectionManager"));
                SceneManager.LoadScene("MainMenu");
            }
        }
        else if (context.action.name == playerInputs.Player.BackToMenu.name)
        {
            Destroy(GameObject.Find("PlayerSelectionManager"));
            SceneManager.LoadScene("MainMenu");
        }
        else if (context.action.name == playerInputs.Player.PauseGame.name)
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (isPaused)
        {
            PauseObj.SetActive(false);
            Time.timeScale = 1;
            isPaused = false;
        }
        else if(EndGame == false)
        {
            PauseObj.SetActive(true);
            Time.timeScale = 0;
            isPaused = true;
        }
    }
    public void OnCarrotPickedUp()
    {
        carrotCount += 1;
        UpdateCarrotText();
        AudioManager.Instance.PlaySound(AudioManager.Sounds.CarrotPickup);
        music.pitch += pitchPerCarrot;
    }

    public void OnDeath()
    {
        deathCount += 1;
        var text = string.Format("{0}", deathCount);
        deathText.text = text;
    }
}
