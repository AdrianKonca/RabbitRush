using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownScript : MonoBehaviour
{
    public Text text;
    public float startTime = 5f;
    private float timePassed = 0f;
    private CharacterMovement characterController;
    void Start()
    {
        text = GetComponent<Text>();
        text.text = Mathf.CeilToInt(startTime).ToString();
        characterController = FindObjectOfType<CharacterMovement>();
    }
    
    void BeginGame()
    {
        text.gameObject.SetActive(false);
        characterController.SetMovement(true);
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > startTime)
        {
            BeginGame();
            return;
        }
        var timeLeft = Mathf.CeilToInt(startTime - timePassed);
        text.text = timeLeft.ToString();
        

    }
}
