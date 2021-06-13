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
    private PlayerInputs playerInputs;

    public Text deathText;
	public Text timeText;
    public AudioClip winSound;
    public AudioClip carrotPickupSound;
    public AudioSource soundEffects;
	public AudioSource music ;
	public float startingPitch = 1.0f ;
	public float gainedPitch = 1.0f ;
	public float elapsedTime = -5.0f ;
	public float volumeDown = 0.4f;
    private float pitchPerCarrot;
    [HideInInspector]
    public float enemySpawnRate = 0.8f;

    public enum GameType { Coop, Versus};
    public GameType type = GameType.Coop;

    private void UpdateCarrotText()
    {
        Debug.Log(enemySpawnRate);
        var text = string.Format("{0}/{1}", carrotCount, maxCarrotCount);
        carrotText.text = text;
    }
	
	private void UpdateTimeText()
	{
		elapsedTime += Time.deltaTime ;
        var text = System.String.Format("{0:0.00}s", elapsedTime);
        timeText.text = text;
	}

    private int GetMaxCarrotCount()
    {
        return GameObject.FindGameObjectsWithTag("Carrot").Length;
    }

    private void DisplaySummary()
    {
        float totalTime = Time.time - timeStarted;
        var text = System.String.Format("Times died: {0}\nTotal time: {1:0.00}s\n\nRATING: {2}\n\nPress 'Enter' to go back to main menu.", deathCount, totalTime, ScoreSystem.GetGrade(1, totalTime, deathCount, SceneManager.GetActiveScene().name));
        summaryText.text = text;
        summaryText.gameObject.SetActive(true);
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
		pitchPerCarrot = gainedPitch / maxCarrotCount ;
        UpdateCarrotText();
    }

    void Update()
    {
		UpdateTimeText() ;
		
        if (carrotCount == maxCarrotCount)
        {
            if (!summaryText.IsActive())
            {
                AudioManager.Instance.PlaySound(AudioManager.Sounds.Win);
				music.pitch = startingPitch ;
				music.volume *= volumeDown ;
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
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
    public void OnCarrotPickedUp()
    {
        carrotCount += 1;
        UpdateCarrotText();
        AudioManager.Instance.PlaySound(AudioManager.Sounds.CarrotPickup);
		music.pitch += pitchPerCarrot ;
    }

    public void OnDeath()
    {
        deathCount += 1;
		var text = string.Format("{0}", deathCount);
        deathText.text = text;
    }
}
