using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class GameController : MonoBehaviour
{
    public Text carrotText;
    public Text summaryText;

    public static GameController Instance;
    private int carrotCount = 0;
    private int maxCarrotCount = 0;
    private int deathCount = 0;
    private float timeStarted = 0;
    private CharacterMovement characterMovement;
    private PlayerInputs playerInputs;

    private void UpdateCarrotText()
    {
        var text = string.Format("    {0}/{1} Carrots", carrotCount, maxCarrotCount);
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

    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning("Trying to create another singleton!");
        Instance = this;
        playerInputs = new PlayerInputs();
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
            if (!summaryText.IsActive())
            {
                AudioManager.Instance.PlaySound(AudioManager.Sounds.Win);
                DisplaySummary();
                characterMovement.SetMovement(false);
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
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
    public void OnCarrotPickedUp()
    {
        carrotCount += 1;
        UpdateCarrotText();
        AudioManager.Instance.PlaySound(AudioManager.Sounds.CarrotPickup);
    }

    public void OnDeath()
    {
        deathCount += 1;
    }
}
