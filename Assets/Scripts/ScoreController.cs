using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public int score = 0;

    private float gameTimer = 0.0f;
    public GameObject ScoreComponent;
    private TextMeshProUGUI scoreText;
    private string timeString;

    private void Update()
    {
        scoreText = ScoreComponent.GetComponent<TextMeshProUGUI>();

        UpdateTimer();

        updateText();
    }

    void UpdateTimer()
    {
        gameTimer += Time.deltaTime;
        // Format the game time into minutes and seconds
        TimeSpan time = TimeSpan.FromSeconds(gameTimer);
        timeString = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }


    void updateText()
    {
        scoreText.text = "Score: " + score + "\nTime: " + timeString;
    }
}
