using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text carrotText;
    public Text summaryText;
    public AudioClip winSound;
    public AudioClip carrotPickupSound;
    public AudioSource soundEffects;

    private int carrotCount = 0;
    private int maxCarrotCount = 0;
    private int deathCount = 0;
    private float timeStarted = 0;
    private CharacterMovement characterMovement;

    private void UpdateCarrotText()
    {
        var text = System.String.Format("    {0}/{1} Carrots", carrotCount, maxCarrotCount);
        carrotText.text = text;
    }

    private int GetMaxCarrotCount()
    {
        return GameObject.FindGameObjectsWithTag("Carrot").Length;
    }

    private void DisplaySummary()
    {
        float totalTime = Time.time - timeStarted;
        var text = System.String.Format("Times died: {0}\nTotal time: {1:0.00}s\n\nRATING: A+", deathCount, totalTime);

        summaryText.text = text;
        summaryText.gameObject.SetActive(true);

        characterMovement.SetMovement(false);

    }

    void Start()
    {
        characterMovement = FindObjectOfType<CharacterMovement>();
        maxCarrotCount = GetMaxCarrotCount();
        timeStarted = Time.time;
    }

    void Update()
    {
        if (carrotCount == maxCarrotCount)
        {
            if (summaryText.IsActive() && (Input.GetKey("space") || Input.GetKey("enter")))
            {
                SceneManager.LoadScene("MainMenu");
            }
            else if (!summaryText.IsActive())
            {
                soundEffects.PlayOneShot(winSound);
                DisplaySummary();
                characterMovement.SetMovement(false);
            }
        }
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void OnCarrotPickedUp()
    {
        carrotCount += 1;
        UpdateCarrotText();
        soundEffects.PlayOneShot(carrotPickupSound, 0.5f);
    }

    public void OnDeath()
    {
        deathCount += 1;
    }
}
